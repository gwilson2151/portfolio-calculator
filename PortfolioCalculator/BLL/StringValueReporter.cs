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
			var values = CalculateValues(portfolio);

			reportBuilder.AppendLine(string.Format("{0} total = {1}", portfolio.Name, values.Values.Sum()));
			foreach (var account in portfolio.Accounts)
			{
				reportBuilder.AppendLine(string.Format("{0} total = {1}", account.Name, account.Positions.Select(p => values[p]).Sum()));
				foreach (var position in account.Positions)
				{
					reportBuilder.AppendLine(string.Format("{0}: {1} x {2} = {3}", position.Security.Name, position.Count, _quoter.GetQuote(position.Security), values[position]));
				}
			}

			return reportBuilder.ToString();
		}

		private IDictionary<IDomainEntity, decimal> CalculateValues(Portfolio portfolio)
		{
			var results = new Dictionary<IDomainEntity, decimal>();

			foreach (var account in portfolio.Accounts)
			{
				foreach (var position in account.Positions)
				{
					var price = _quoter.GetQuote(position.Security);
					var value = price * position.Count;
					results.Add(position, value);
				}
			}

			return results;
		}
	}
}