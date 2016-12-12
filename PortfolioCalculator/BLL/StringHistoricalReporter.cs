using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.Interfaces;
using Contracts;
using Contracts.Interfaces;

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
			var values = GetValues(portfolio, prices);
			var columnOrder = values.Keys.OrderBy(d => d.Date);

			// print header
			reportBuilder.Append("\t\t");
			foreach (var date in columnOrder)
			{
				reportBuilder.AppendFormat("| {0} ", date.ToString("d"));
			}
			reportBuilder.AppendLine();
			reportBuilder.Append("----------------");
			for (var i = 0; i < columnOrder.Count(); ++i)
				reportBuilder.Append("-------------");
			reportBuilder.AppendLine();
			
			foreach (var account in portfolio.Accounts)
			{
				reportBuilder.AppendLine(string.Format("{0,-16}", account.Name));
				foreach (var position in account.Positions)
				{
					reportBuilder.AppendFormat("{0,-16}", position.Security.Symbol);
					foreach (var date in columnOrder)
					{
						var rowValues = values[date];
						var value = rowValues[position];
						if (value < 0M)
							reportBuilder.AppendFormat("| {0,10} ", " !*v*! ");
						else
							reportBuilder.AppendFormat("| {0,10} ", value.ToString("##.00"));
					}
					reportBuilder.AppendLine();
				}
			}

			return reportBuilder.ToString();
		}

		private static IDictionary<DateTime, IDictionary<IDomainEntity, decimal>> GetValues(Portfolio portfolio, IDictionary<DateTime, IDictionary<string, decimal>> priceDates)
		{
			var results = new Dictionary<DateTime, IDictionary<IDomainEntity, decimal>>();

			foreach (var date in priceDates.Keys)
			{
				var values = new Dictionary<IDomainEntity, decimal>();
				results[date] = values;
				var prices = priceDates[date];

				foreach (var account in portfolio.Accounts)
				{
					foreach (var position in account.Positions)
					{
						try
						{
							var price = prices[position.Security.Symbol];
							var value = price*position.Shares;
							values.Add(position, value);
						}
						catch (KeyNotFoundException)
						{
							values.Add(position, -1M);
						}
					}
				}
			}

			return results;
		}

		private IDictionary<DateTime, IDictionary<string, decimal>> GetPrices(Portfolio portfolio, DateTime start, DateTime end, Period period)
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
