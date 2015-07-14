using BLL.Interfaces;

using YSQ.core.Quotes;

namespace BLL.Factories
{
	public class QuoteServiceFactory : IQuoteServiceFactory
	{
		public IBuildQuotes GetYahooStockQuotesService()
		{
			return new QuoteService();
		}
	}
}