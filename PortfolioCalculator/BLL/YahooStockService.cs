using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using BLL.Interfaces;

using Contracts;

using YSQ.core.Quotes;

namespace BLL
{
	public class YahooStockService : ISecurityQuoter
	{
		private readonly IBuildQuotes _quoteBuilder;

		public YahooStockService(IQuoteServiceFactory quoteServiceFactory)
		{
			_quoteBuilder = quoteServiceFactory.GetYahooStockQuotesService();
		}

		public IDictionary<string, decimal> GetQuotes(IEnumerable<Security> securities)
		{
			var symbols = securities.Select(s => s.Symbol).Distinct().ToArray();
			var quotes = _quoteBuilder.Quote(symbols).Return(QuoteReturnParameter.Symbol, QuoteReturnParameter.LatestTradePrice);
			return quotes.Where(q => !q.LatestTradePrice.Equals("N/A")).ToDictionary<dynamic, string, decimal>(key => RemoveYahooSymbolFormat(key.Symbol), value => decimal.Parse(value.LatestTradePrice, CultureInfo.InvariantCulture));
		}

		private static string RemoveYahooSymbolFormat(string rawSymbol)
		{
			return rawSymbol.Replace("\"", string.Empty);
		}
	}
}