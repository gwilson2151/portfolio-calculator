using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.Interfaces;
using Contracts;
using Contracts.Interfaces;

namespace BLL
{
	public class StringWeightReporter
	{
		private readonly ISecurityQuoter _quoter;

		public StringWeightReporter(ISecurityQuoter quoter)
		{
			_quoter = quoter;
		}

		public string GetReport(Portfolio portfolio, IEnumerable<Category> categories, IEnumerable<CategoryWeight> weights)
		{
			var reportBuilder = new StringBuilder();
			var quotes = GetQuotes(portfolio);
			var valuesDict = CalculateValues(portfolio, quotes);
			var weightsList = weights.ToList();

			var total = valuesDict.Values.Where(v => v > 0M).Sum();

			reportBuilder.AppendLine(string.Format("Portfolio: {0}", portfolio.Name));
			foreach (var category in categories)
			{
				Dictionary<string, decimal> calculations = new Dictionary<string, decimal>();

				foreach (var kvpair in valuesDict)
				{
					var weight = weightsList.Single(w => w.Security.Symbol.Equals(kvpair.Key) && w.Value.Category == category);
					if (calculations.ContainsKey(weight.Value.Name))
						calculations[weight.Value.Name] += kvpair.Value;
					else
						calculations.Add(weight.Value.Name, kvpair.Value);
				}

				reportBuilder.AppendLine(string.Format("\r\n{0}", category.Name));
				foreach (var kvpair in calculations.OrderByDescending(kv => kv.Value))
				{
					reportBuilder.AppendLine(String.Format("{0}: {1:N1}%", kvpair.Key, kvpair.Value / total * 100M));
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

		private static IDictionary<string, decimal> CalculateValues(Portfolio portfolio, IDictionary<string, decimal> quotes)
		{
			var results = new Dictionary<string, decimal>();

			foreach (var account in portfolio.Accounts)
			{
				foreach (var position in account.Positions)
				{
					try
					{
						var price = quotes[position.Security.Symbol];
						var value = price * position.Shares;
						if (results.ContainsKey(position.Security.Symbol))
							results[position.Security.Symbol] += value;
						else
							results.Add(position.Security.Symbol, value);
					}
					catch (KeyNotFoundException)
					{
						results.Add(position.Security.Symbol, -1M);
					}
				}
			}

			return results;
		}
	}
}