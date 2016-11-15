using YSQ.core.Historical;
using YSQ.core.Quotes;

namespace BLL.Interfaces
{
	public interface IYahooServiceFactory
	{
		IBuildQuotes GetQuotesService();
		IGetHistoricalPrices GetHistoricalPricesService();
	}
}