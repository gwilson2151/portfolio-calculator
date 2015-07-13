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
			var values = CalculateValues(portfolio, quotes);

			reportBuilder.AppendLine(string.Format("{0} total = {1}", portfolio.Name, values.Values.Sum()));
			foreach (var account in portfolio.Accounts)
			{
				reportBuilder.AppendLine(string.Format("{0} total = {1}", account.Name, account.Positions.Select(p => values[p]).Sum()));
				foreach (var position in account.Positions)
				{
					reportBuilder.AppendLine(string.Format("{0}: {1} x {2} = {3}", position.Security.Symbol, position.Count, quotes[position.Security.Symbol], values[position]));
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
					var price = quotes[position.Security.Symbol];
					var value = price * position.Count;
					results.Add(position, value);
				}
			}

			return results;
		}
	}
}