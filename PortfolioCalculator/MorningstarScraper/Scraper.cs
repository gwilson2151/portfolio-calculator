using System;
using System.Collections.Generic;
using AngleSharp;

namespace MorningstarScraper
{
	public class Scraper
	{
		private const string AssetAllocationUrl = "http://portfolios.morningstar.com/fund/summary?t={0}";

		public IDictionary<string, decimal> GetAssetAllocation(string ticker)
		{
			var results = new Dictionary<string, decimal>();
			var config = Configuration.Default.WithDefaultLoader();
			var address = string.Format(AssetAllocationUrl, ticker);
			var document = BrowsingContext.New(config).OpenAsync(address).Result;
			const string rowSelector = "table#asset_allocation_tab tbody tr";
			var rows = document.QuerySelectorAll(rowSelector);

			foreach (var row in rows)
			{
				if (!string.IsNullOrWhiteSpace(row.ClassName) && row.ClassName.ToLower() == "hr")
					continue;

				var assetLabel = row.Children[1].TextContent;
				var assetAllocation = row.Children[2].TextContent;
				decimal value;
				var success = decimal.TryParse(assetAllocation, out value);
				if (!success)
					Console.WriteLine("MorningstarScraper - [{0}] wouldn't parse to decimal.", assetAllocation);
				results.Add(assetLabel, value);
			}

			return results;
		}
	}
}
