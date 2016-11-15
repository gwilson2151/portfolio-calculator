using System;
using System.Linq;

using NUnit.Framework;

using YSQ.core.Quotes;
using YSQ.core.Historical;

namespace Tests.BLL
{
	[TestFixture, Explicit]
	public class YsqTestBed
	{
		[Test]
		public void Test()
		{
			var service = new QuoteService();
			var symbols = new[] { "GOOG", "XCS.TO"};
			var quotes = service.Quote(symbols).Return(QuoteReturnParameter.Symbol, QuoteReturnParameter.LatestTradePrice);
		}

		[Test]
		public void BadSymbolTest()
		{
			var service = new QuoteService();
			var symbols = new[] { "GOOG", "BJT" };
			var quotes = service.Quote(symbols).Return(QuoteReturnParameter.Symbol, QuoteReturnParameter.LatestTradePrice);
		}

		[Test]
		public void HistoricalTest()
		{
			var service = new HistoricalPriceService();
			var endDate = DateTime.Today.AddDays(-1);
			var startDate = endDate.AddDays(-49);

			var prices = service.Get("GOOG", startDate, endDate, Period.Weekly).ToList();

		}
	}
}