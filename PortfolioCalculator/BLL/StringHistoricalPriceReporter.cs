using BLL.Interfaces;
using Contracts;

namespace BLL
{
	public class StringHistoricalPriceReporter
	{
		private readonly ISecurityHistoricalPricer _pricer;

		public enum HistoryLength
		{
			Week, // days
			Month, // weeks
			Quarter, // weeks
			Half, // months
			Year // months
		}

		public StringHistoricalPriceReporter(ISecurityHistoricalPricer pricer)
		{
			_pricer = pricer;
		}

		public void Build(Portfolio portfolio, HistoryLength howFarPast = HistoryLength.Month)
		{
			
		}

		public string GetReport()
		{
			return "Not implemented";
		}
	}
}
