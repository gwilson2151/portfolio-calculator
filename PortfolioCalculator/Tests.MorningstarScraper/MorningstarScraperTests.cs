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
	}
}
