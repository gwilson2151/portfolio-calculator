using Contracts;

using NUnit.Framework;

using YSQ.core.Quotes;

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
	}
}