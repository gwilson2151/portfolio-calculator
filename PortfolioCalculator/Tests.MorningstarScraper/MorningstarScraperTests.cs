using MorningstarScraper;
using NUnit.Framework;

namespace Tests.MorningstarScraper
{
	[TestFixture]
	public class MorningstarScraperTests
	{
		[Test]
		public void SimpleDemo()
		{
			var scraper = new Scraper();
			var blah = scraper.GetAssetAllocation("XCS");
		}

		[Test]
		public void SameSymbolDifferentExchange()
		{
			var scraper = new Scraper();
			var blah = scraper.GetAssetAllocation("IAU");
		}

		[Test]
		public void NoAssetAllocation()
		{
			var scraper = new Scraper();
			var blah = scraper.GetAssetAllocation("GOOG");
		}
	}
}
