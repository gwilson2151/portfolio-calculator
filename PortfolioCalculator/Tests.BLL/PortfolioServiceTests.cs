using BLL;

using NUnit.Framework;

namespace Tests.BLL
{
	[TestFixture]
	public class PortfolioServiceTests
	{
		[Test]
		public void When_Updating_Portfolio_With_Transactions_Then_Portfolio_Ends_In_Correct_State()
		{
			var portfolio = TestDataGenerator.GenerateDefaultPortfolio();
			var portfolioService = new PortfolioService(portfolio);

			//generate transactions

			//apply transaction updates
			//portfolioService.UpdateWith(transactions);

			//verify
			Assert.True(false);
		}
	}
}