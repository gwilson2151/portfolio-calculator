using System;
using System.Dynamic;
using System.Linq;

using BLL;
using BLL.Interfaces;

using Contracts;

using Moq;
using NUnit.Framework;

using YSQ.core.Quotes;

namespace Tests.BLL
{
	[TestFixture]
	public class YahooStockServiceTests
	{
		private Mock<IBuildQuotes> _ysqMock;
		private Mock<IFindQuotes> _ysqfMock;
		private Mock<IQuoteServiceFactory> _qsfMock;

		[SetUp]
		public void SetUp()
		{
			_ysqMock = new Mock<IBuildQuotes>();
			_ysqfMock = new Mock<IFindQuotes>();
			_qsfMock = new Mock<IQuoteServiceFactory>();

			_qsfMock.Setup(m => m.GetYahooStockQuotesService()).Returns(_ysqMock.Object);
		}

		[Test]
		public void When_YSQ_Can_Find_Quote_Then_Service_Returns_Quote()
		{
			// setup
			dynamic quote = new ExpandoObject();
			quote.Symbol = "\"GOOG\"";
			quote.LatestTradePrice = "657.73";

			_ysqMock.Setup(m => m.Quote(It.IsAny<string[]>())).Returns(_ysqfMock.Object);
			_ysqfMock.Setup(m => m.Return(It.IsAny<QuoteReturnParameter[]>())).Returns(new [] {quote});

			// execute
			var service = new YahooStockService(_qsfMock.Object);
			var quotes = service.GetQuotes(new [] {new Security {Symbol = "GOOG"}});

			// verify
			Assert.That(quotes.Count, Is.EqualTo(1));
			var verify = quotes.Single();
			Assert.That(verify.Key, Is.EqualTo("GOOG"));
			Assert.That(verify.Value, Is.EqualTo(657.73M));
		}

		[Test]
		public void When_YSQ_Can_Not_Find_Quote_Then_Service_Discards_Quote()
		{
			// setup
			dynamic googQuote = new ExpandoObject();
			googQuote.Symbol = "\"GOOG\"";
			googQuote.LatestTradePrice = "657.73";
			dynamic bjtQuote = new ExpandoObject();
			bjtQuote.Symbol = "\"BJT\"";
			bjtQuote.LatestTradePrice = "N/A";

			_ysqMock.Setup(m => m.Quote(It.IsAny<string[]>())).Returns(_ysqfMock.Object);
			_ysqfMock.Setup(m => m.Return(It.IsAny<QuoteReturnParameter[]>())).Returns(new[] { googQuote, bjtQuote });

			// execute
			var service = new YahooStockService(_qsfMock.Object);
			var quotes = service.GetQuotes(new[] { new Security { Symbol = "GOOG" }, new Security { Symbol = "BJT"} });

			// verify
			Assert.That(quotes.Count, Is.EqualTo(1));
			var verify = quotes.Single();
			Assert.That(verify.Key, Is.EqualTo("GOOG"));
			Assert.That(verify.Value, Is.EqualTo(657.73M));
		}
	}
}