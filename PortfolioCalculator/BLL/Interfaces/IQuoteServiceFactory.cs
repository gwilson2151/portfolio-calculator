using YSQ.core.Quotes;

namespace BLL.Interfaces
{
	public interface IQuoteServiceFactory
	{
		IBuildQuotes GetYahooStockQuotesService();
	}
}