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

		public IDictionary<string, decimal> GetQuotes(IEnumerable<Security> security)
		{
			var quotes = _quoteBuilder.Quote(security.Select(s => s.Symbol).ToArray()).Return(QuoteReturnParameter.Symbol, QuoteReturnParameter.Bid);
			return quotes.ToDictionary<dynamic, string, decimal>(key => RemoveYahooSymbolFormat(key.Symbol), value => decimal.Parse(value.Bid, CultureInfo.InvariantCulture));
		}

		private static string RemoveYahooSymbolFormat(string rawSymbol)
		{
			return rawSymbol.Replace("\"", string.Empty);
		}
	}
}