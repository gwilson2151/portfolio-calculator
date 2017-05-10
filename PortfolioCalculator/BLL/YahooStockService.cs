using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using BLL.Interfaces;

using Contracts;
using YSQ.core.Historical;
using YSQ.core.Quotes;
using Period = YSQ.core.Historical.Period;

namespace BLL
{
	public class YahooStockService : ISecurityQuoter, ISecurityHistoricalPricer
	{
		private readonly IBuildQuotes _quoteBuilder;
		private readonly IGetHistoricalPrices _priceFinder;

		public YahooStockService(IYahooServiceFactory yahooServiceFactory)
		{
			_quoteBuilder = yahooServiceFactory.GetQuotesService();
			_priceFinder = yahooServiceFactory.GetHistoricalPricesService();
		}

		public IDictionary<Security, decimal> GetQuotes(IEnumerable<Security> securities)
		{
			var symbolMap = securities.ToDictionary(s => s.Symbol);
			var quotes = _quoteBuilder.Quote(symbolMap.Keys.ToArray()).Return(QuoteReturnParameter.Symbol, QuoteReturnParameter.LatestTradePrice);
			return quotes.Where(q => !q.LatestTradePrice.Equals("N/A")).ToDictionary<dynamic, Security, decimal>(key => symbolMap[RemoveYahooSymbolFormat(key.Symbol)], value => decimal.Parse(value.LatestTradePrice, CultureInfo.InvariantCulture));
		}

		public IDictionary<DateTime, IDictionary<string, decimal>> GetHistoricalPrices(IEnumerable<Security> securities, DateTime start, DateTime end, Contracts.Period period)
		{
			var symbols = securities.Select(s => s.Symbol).Distinct().ToList();
			var results = new Dictionary<DateTime, IDictionary<string, decimal>>();

			foreach (var s in symbols)
			{
				var prices = _priceFinder.Get(s, start, end, TranslatePeriod(period));
				foreach (var p in prices)
				{
					IDictionary<string, decimal> datePrices;
					if (!results.TryGetValue(p.Date, out datePrices))
					{
						datePrices = new Dictionary<string, decimal>();
						results[p.Date] = datePrices;
					}

					datePrices[s] = p.Price;
				}
			}

			return results;
		}

		private static Period TranslatePeriod(Contracts.Period period)
		{
			switch (period)
			{
				case Contracts.Period.Daily:
					return Period.Daily;
				case Contracts.Period.Weekly:
					return Period.Weekly;
				case Contracts.Period.Monthly:
				case Contracts.Period.Quarterly:
				case Contracts.Period.Annually:
					return Period.Monthly;
				default:
					throw new ArgumentOutOfRangeException("period", period, string.Format("period out of expected range [{0}]", period));
			}
		}

		private static string RemoveYahooSymbolFormat(string rawSymbol)
		{
			return rawSymbol.Replace("\"", string.Empty);
		}
	}
}