using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.Interfaces;
using Contracts;

namespace BLL
{
	public class StringHistoricalReporter
	{
		private readonly ISecurityHistoricalPricer _historicalPricer;

		public StringHistoricalReporter(ISecurityHistoricalPricer historicalPricer)
		{
			_historicalPricer = historicalPricer;
		}


		public string GetReport(Portfolio portfolio, DateTime start, DateTime end, Period period)
		{
			var reportBuilder = new StringBuilder();
			var prices = GetPrices(portfolio, start, end, period);

			reportBuilder.AppendLine("not fully implemented");
			return reportBuilder.ToString();
		}

		public IDictionary<DateTime, IDictionary<string, decimal>> GetPrices(Portfolio portfolio, DateTime start, DateTime end, Period period)
		{
			var securities = new List<Security>();
			foreach (var account in portfolio.Accounts)
			{
				securities.AddRange(account.Positions.Select(p => p.Security));
			}
			return _historicalPricer.GetHistoricalPrices(securities.Distinct(), start, end, period);
		}
	}
}
