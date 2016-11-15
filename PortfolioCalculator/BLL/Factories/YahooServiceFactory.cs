using BLL.Interfaces;
using YSQ.core.Historical;
using YSQ.core.Quotes;

namespace BLL.Factories
{
	public class YahooServiceFactory : IYahooServiceFactory
	{
		public IBuildQuotes GetQuotesService()
		{
			return new QuoteService();
		}

		public IGetHistoricalPrices GetHistoricalPricesService()
		{
			return new HistoricalPriceService();
		}
	}
}