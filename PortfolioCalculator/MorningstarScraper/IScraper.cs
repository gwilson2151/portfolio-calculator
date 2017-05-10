using System.Collections.Generic;

namespace MorningstarScraper
{
	public interface IScraper
	{
		IDictionary<string, decimal> GetAssetAllocation(string ticker);
	}
}