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
		private readonly IDictionary<string, SymbolData> _symbolCacheByName = new Dictionary<string, SymbolData>();
		private readonly IDictionary<ulong, SymbolData> _symbolCacheById = new Dictionary<ulong, SymbolData>();

		private readonly IDictionary<string, string> _exchangeMapping;

		public QuestradeService(IQuestradeApiTokenManager tokenManager, ISecurityRepository securityRepository, ICategoryRepository categoryRepository)
		{
			_tokenManager = tokenManager;
			_securityRepo = securityRepository;
			_categoryRepository = categoryRepository;

			_exchangeMapping = InitializeExchangeMapping();
		}

		private static IDictionary<string, string> InitializeExchangeMapping()
		{
			return new Dictionary<string, string>
			{
				{ "TSX", "TO"}
			};
		}

		public List<Account> GetAccounts()
		{
			var response = GetAccountsResponse.GetAccounts(_tokenManager.GetAuthToken());
			return response.Accounts.Select(a => new Account { Name = a.m_number }).ToList();
		}

		public List<Position> GetPositions(Account account)
		{
			var response = GetPositionsResponse.GetPositions(_tokenManager.GetAuthToken(), account.Name);
			var symbolIds = response.Positions.Select(p => p.m_symbolId);
			var symbolData = GetSymbolsById(symbolIds).ToDictionary<SymbolData, ulong>(s => s.m_symbolId);

			return response.Positions.Select(qp => new Position
			{
				Account = account,
				Security = _securityRepo.GetOrCreate(symbolData[qp.m_symbolId].m_listingExchange, StripExchangeFromSymbol(qp.m_symbol)),
				Shares = Convert.ToDecimal(qp.m_openQuantity)
			}).ToList();
		}

		public List<Transaction> GetTransactions(Account account, DateTime startDate, DateTime endDate)
		{
			var response = GetExecutionsResponse.GetExecutions(_tokenManager.GetAuthToken(), account.Name, startDate, endDate);
			var symbolIds = response.Executions.Select(p => p.m_symbolId).Distinct();
			var symbolData = GetSymbolsById(symbolIds).ToDictionary<SymbolData, ulong>(s => s.m_symbolId);

			return response.Executions.Select(qe => new Transaction
			{
				Account = account,
				Date = qe.m_timestamp,
				Price = Convert.ToDecimal(qe.m_price),
				Security = _securityRepo.GetOrCreate(symbolData[qe.m_symbolId].m_listingExchange, StripExchangeFromSymbol(qe.m_symbol)),
				Shares = Convert.ToDecimal(qe.m_quantity),
				Type = ConvertQuestradeSideToTransactionType(qe.m_side)
			}).ToList();
		}

		public IEnumerable<SymbolData> GetSymbols(IEnumerable<Security> securities)
		{
			var notFound = securities.Where(s => !_symbolCacheByName.ContainsKey(BuildSymbolFromSecurity(s))).Select(BuildSymbolFromSecurity).ToList();
			if (notFound.Any())
			{
				var response = GetSymbolsResponse.GetSymbols(_tokenManager.GetAuthToken(), new List<ulong>(), notFound);
				//response.Symbols.ForEach(s => _symbolCacheByName[s.m_symbol.ToUpper()] = s);
				AddToCache(response.Symbols);
			}

			return securities.Select<Security, SymbolData>(s => _symbolCacheByName[BuildSymbolFromSecurity(s)]);
		}

		// Interface Implementations
		public IDictionary<Security, decimal> GetQuotes(IEnumerable<Security> securities)
		{
			var symbolData = GetSymbols(securities);
			var symbolIds = symbolData.Select<SymbolData, ulong>(qsd => qsd.m_symbolId).ToList();
			var securityMap = securities.ToDictionary(BuildSymbolFromSecurity);

			var response = GetQuoteResponse.GetQuote(_tokenManager.GetAuthToken(), symbolIds);
			return response.Quotes.Where(q => !double.IsInfinity(q.m_lastTradePrice) && !double.IsNaN(q.m_lastTradePrice))
				.ToDictionary<Level1DataItem, Security, decimal>(key => securityMap[key.m_symbol], value => Convert.ToDecimal(value.m_lastTradePrice));
		}

		// this is going to get nuts...
		public IList<CategoryWeight> GetWeights(Category category, Security security)
		{
			var weights = _categoryRepository.GetWeights(category, security).ToList();
			if (weights.Any(w => w.Value.Category == category))
				return weights.Where(w => w.Value.Category == category).ToList();
			return GetWeightDataFromQuestrade(category, security);
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
				throw new CategoryNotSupportedException(string.Format("Category not supported: {0}", category.Name));
			}

			if (!list.Any())
				return new List<CategoryWeight>();

			_categoryRepository.AddWeights(category, security, list);
			return list;
		}

		// End Interface Implementations


		private IEnumerable<SymbolData> GetSymbolsById(IEnumerable<ulong> symbolIds)
		{
			var notFound = symbolIds.Where(s => !_symbolCacheById.ContainsKey(s)).ToList();
			if (notFound.Any())
			{
				var response = GetSymbolsResponse.GetSymbols(_tokenManager.GetAuthToken(), notFound, new List<string>());
				AddToCache(response.Symbols);
			}
			return symbolIds.Select<ulong, SymbolData>(id => _symbolCacheById[id]);
		}

		private void AddToCache(IEnumerable<SymbolData> symbolData)
		{
			//symbolData.ForEach(s => _symbolCacheByName[s.m_symbol.ToUpper()] = s);
			foreach (var data in symbolData)
			{
				_symbolCacheByName[data.m_symbol.ToUpper()] = data;
				_symbolCacheById[data.m_symbolId] = data;
			}
		}

		private string BuildSymbolFromSecurity(Security security)
		{
			return string.Format("{0}.{1}", security.Symbol, _exchangeMapping[security.Exchange]);
		}

		private string StripExchangeFromSymbol(string symbol)
		{
			return symbol.Split('.')[0];
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