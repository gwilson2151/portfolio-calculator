using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BLL.Interfaces;

using Contracts;
using Contracts.Interfaces;

namespace BLL
{
	public class StringValueReporter
	{
		private readonly ISecurityQuoter _quoter;

		public StringValueReporter(ISecurityQuoter quoter)
		{
			_quoter = quoter;
		}

		public string GetReport(Portfolio portfolio)
		{
			var reportBuilder = new StringBuilder();
			var quotes = GetQuotes(portfolio);
			var valuesDict = CalculateValues(portfolio, quotes);

			reportBuilder.AppendLine(string.Format("{0} total = {1}", portfolio.Name, valuesDict.Values.Where(v => v > 0M).Sum()));
			foreach (var account in portfolio.Accounts)
			{
				reportBuilder.AppendLine(string.Format("{0} total = {1}", account.Name, account.Positions.Select(p => valuesDict[p]).Where(v => v > 0M).Sum()));
				foreach (var position in account.Positions)
				{
					var value = valuesDict[position];
					if (value < 0M)
						reportBuilder.AppendLine(string.Format("{0}: {1} x {2} = {3}", position.Security.Symbol, position.Shares, "quote not found", "unknown"));
					else
						reportBuilder.AppendLine(string.Format("{0}: {1} x {2} = {3}", position.Security.Symbol, position.Shares, quotes[position.Security.Symbol], value));
				}
			}

			return reportBuilder.ToString();
		}

		private IDictionary<string, decimal> GetQuotes(Portfolio portfolio)
		{
			var securities = new List<Security>();
			foreach (var account in portfolio.Accounts)
			{
				securities.AddRange(account.Positions.Select(p => p.Security));
			}
			return _quoter.GetQuotes(securities.Distinct());
		}

		private static IDictionary<IDomainEntity, decimal> CalculateValues(Portfolio portfolio, IDictionary<string, decimal> quotes)
		{
			var results = new Dictionary<IDomainEntity, decimal>();

			foreach (var account in portfolio.Accounts)
			{
				foreach (var position in account.Positions)
				{
					try
					{
						var price = quotes[position.Security.Symbol];
						var value = price * position.Shares;
						results.Add(position, value);
					}
					catch (KeyNotFoundException)
					{
						results.Add(position, -1M);
					}
				}
			}

			return results;
		}
	}
}