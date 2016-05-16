using System;
using System.Collections.Generic;
using System.Linq;
using BLL.Exceptions;
using BLL.Interfaces;

using Contracts;

using QuestradeAPI;
using OrderSide = Questrade.BusinessObjects.Entities.OrderSide;
using SecurityType = Questrade.BusinessObjects.Entities.SecurityType;
using SymbolData = Questrade.BusinessObjects.Entities.SymbolData;
using Level1DataItem = Questrade.BusinessObjects.Entities.Level1DataItem;

namespace BLL
{
    public class QuestradeService : ISecurityQuoter, ISecurityCategoriser
	{
		private readonly IQuestradeApiTokenManager _tokenManager;
        private readonly ISecurityRepository _securityRepo;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDictionary<string, SymbolData> _symbolCache = new Dictionary<string, SymbolData>();

        public QuestradeService(IQuestradeApiTokenManager tokenManager, ISecurityRepository securityRepository, ICategoryRepository categoryRepository)
		{
			_tokenManager = tokenManager;
			_securityRepo = securityRepository;
            _categoryRepository = categoryRepository;
		}

		public List<Account> GetAccounts()
		{
			var response = GetAccountsResponse.GetAccounts(_tokenManager.GetAuthToken());
			return response.Accounts.Select(a => new Account { Name = a.m_number }).ToList();
		}

		public List<Position> GetPositions(Account account)
		{
			var response = GetPositionsResponse.GetPositions(_tokenManager.GetAuthToken(), account.Name);
			return response.Positions.Select(qp => new Position
			{
				Account = account,
				Security = _securityRepo.GetBySymbol(qp.m_symbol),
				Shares = Convert.ToDecimal(qp.m_openQuantity)
			}).ToList();
		}

		public List<Transaction> GetTransactions(Account account, DateTime startDate, DateTime endDate)
		{
			var response = GetExecutionsResponse.GetExecutions(_tokenManager.GetAuthToken(), account.Name, startDate, endDate);
			return response.Executions.Select(qe => new Transaction
			{
				Account = account,
				Date = qe.m_timestamp,
				Price = Convert.ToDecimal(qe.m_price),
				Security = _securityRepo.GetBySymbol(qe.m_symbol),
				Shares = Convert.ToDecimal(qe.m_quantity),
				Type = ConvertQuestradeSideToTransactionType(qe.m_side)
			}).ToList();
		}

        public IEnumerable<SymbolData> GetSymbols(IEnumerable<Security> securities)
        {
            if (securities.Any(s => !_symbolCache.ContainsKey(s.Symbol)))
            {
                var symbolNames = securities.Where(s => !_symbolCache.ContainsKey(s.Symbol)).Select(s => s.Symbol).ToList();
                var response = GetSymbolsResponse.GetSymbols(_tokenManager.GetAuthToken(), new List<ulong>(), symbolNames);
                response.Symbols.ForEach(s => _symbolCache[s.m_symbol.ToUpper()] = s);
            }

            return securities.Select<Security, SymbolData>(s => _symbolCache[s.Symbol]);
        }

        // Interface Implementations
	    public IDictionary<string, decimal> GetQuotes(IEnumerable<Security> securities)
	    {
	        var symbolData = GetSymbols(securities);
            var symbolIds = symbolData.Select<SymbolData, ulong>(qsd => qsd.m_symbolId).ToList();

	        var response = GetQuoteResponse.GetQuote(_tokenManager.GetAuthToken(), symbolIds);
	        return response.Quotes.ToDictionary<Level1DataItem, string, decimal>(key => key.m_symbol, value => Convert.ToDecimal(value.m_lastTradePrice));
	    }

        // this is going to get nuts...
        public IList<CategoryWeight> GetWeights(Category category, Security security)
        {
            // this exception block sucks. TODO - Replace with something sane.
            try
            {
                var weights = _categoryRepository.GetWeights(category, security).ToList();
                if (!weights.Any(w => w.Value.Category == category))
                    return GetWeightDataFromQuestrade(category, security);
                return weights.Where(w => w.Value.Category == category).ToList();
            }
            catch (SecurityNotFoundException)
            {
                return GetWeightDataFromQuestrade(category, security);
            }
        }

        private IList<CategoryWeight> GetWeightDataFromQuestrade(Category category, Security security)
        {
            var symbolData = GetSymbols(new[] { security }).Single();
            var values = _categoryRepository.GetValues(category);
            var list = new List<CategoryWeight>();

            if (category.Name.Equals("security", StringComparison.InvariantCultureIgnoreCase))
            {
                string categoryValueName;
                switch (symbolData.m_securityType)
                {
                    case SecurityType.Stock:
                        categoryValueName = "Stock";
                        break;
                    case SecurityType.Bond:
                        categoryValueName = "Bond";
                        break;
                    default:
                        throw new Exception(string.Format("Questrade security type not supported: {0}", symbolData.m_securityType));
                }

                var value = values.SingleOrDefault(v => v.Name.Equals(categoryValueName));
                if (value == null)
                {
                    value = new CategoryValue { Category = category, Name = categoryValueName };
                    _categoryRepository.AddValues(category, new[] { value });
                }
                list.Add(new CategoryWeight { Security = security, Weight = 100M, Value = value });
            }
            else if (category.Name.Equals("currency", StringComparison.InvariantCultureIgnoreCase))
            {
                var value = values.SingleOrDefault(v => v.Name.Equals(symbolData.m_currency));
                if (value == null)
                {
                    value = new CategoryValue { Category = category, Name = symbolData.m_currency };
                    _categoryRepository.AddValues(category, new[] { value });
                }
                list.Add(new CategoryWeight { Security = security, Weight = 100M, Value = value });
            }
            else
            {
                throw new Exception(string.Format("Category not supported: {0}", category.Name));
            }

            if (!list.Any())
                return new List<CategoryWeight>();

            _categoryRepository.AddWeights(category, security, list);
            return list;
        }

        // End Interface Implementations

		private TransactionType ConvertQuestradeSideToTransactionType(OrderSide side)
		{
			switch (side)
			{
				case OrderSide.Buy:
					return TransactionType.Buy;
				case OrderSide.Sell:
					return TransactionType.Sell;
				case OrderSide.BTC:
				case OrderSide.BTO:
				case OrderSide.Count:
				case OrderSide.Cov:
				case OrderSide.Short:
				case OrderSide.STC:
				case OrderSide.STO:
				case OrderSide.Undefined:
				default:
					throw new Exception(string.Format("Unsupported Questrade order type: {0}", side.ToString()));
			}
		}
	}
}