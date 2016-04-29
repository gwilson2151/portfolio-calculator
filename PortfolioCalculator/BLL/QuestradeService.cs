using System;
using System.Collections.Generic;
using System.Linq;

using BLL.Interfaces;

using Contracts;

using QuestradeAPI;
using OrderSide = Questrade.BusinessObjects.Entities.OrderSide;
using SymbolData = Questrade.BusinessObjects.Entities.SymbolData;
using Level1DataItem = Questrade.BusinessObjects.Entities.Level1DataItem;

namespace BLL
{
    public class QuestradeService : ISecurityQuoter
	{
		private readonly IQuestradeApiTokenManager _tokenManager;
		private readonly InMemorySecurityRepository _securityRepo;
        private readonly IDictionary<string, SymbolData> _symbolCache = new Dictionary<string, SymbolData>();

		public QuestradeService(IQuestradeApiTokenManager tokenManager, InMemorySecurityRepository securityRepository)
		{
			_tokenManager = tokenManager;
			_securityRepo = securityRepository;
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

	    public IDictionary<string, decimal> GetQuotes(IEnumerable<Security> securities)
	    {
	        var symbolData = GetSymbols(securities);
            var symbolIds = symbolData.Select<SymbolData, ulong>(qsd => qsd.m_symbolId).ToList();

	        var response = GetQuoteResponse.GetQuote(_tokenManager.GetAuthToken(), symbolIds);
	        return response.Quotes.ToDictionary<Level1DataItem, string, decimal>(key => key.m_symbol, value => Convert.ToDecimal(value.m_lastTradePrice));
	    }

        public IEnumerable<SymbolData> GetSymbols(IEnumerable<Security> securities)
        {
            var securitiesToFetch = securities.Where(s => !_symbolCache.ContainsKey(s.Symbol));

            if (securitiesToFetch.Any())
            {
                var symbolNames = securitiesToFetch.Select(s => s.Symbol).ToList();
                var response = GetSymbolsResponse.GetSymbols(_tokenManager.GetAuthToken(), new List<ulong>(), symbolNames);
                response.Symbols.ForEach(s => _symbolCache[s.m_symbol.ToUpper()] = s);
            }

            return securities.Select<Security, SymbolData>(s => _symbolCache[s.Symbol]);
        }

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