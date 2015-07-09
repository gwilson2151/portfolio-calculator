using System.Collections.Generic;

using BLL;

using Moq;
using NUnit.Framework;

using BLL.Interfaces;
using Contracts;

namespace Tests.BLL
{
	[TestFixture]
    public class StringValueReporterTests
	{
		private Mock<ISecurityQuoter> _quoterMock;

		[SetUp]
		public void SetUp()
		{
			_quoterMock = new Mock<ISecurityQuoter>();
		}

		[Test]
		public void Test()
		{
			// setup
			var portfolio = GenerateDefaultPortfolio();
			_quoterMock.Setup(m => m.GetQuote(It.Is<Security>(s => s.Name == "goog"))).Returns(new decimal(18.25));
			_quoterMock.Setup(m => m.GetQuote(It.Is<Security>(s => s.Name == "msft"))).Returns(new decimal(15));
			_quoterMock.Setup(m => m.GetQuote(It.Is<Security>(s => s.Name == "aapl"))).Returns(new decimal(9.36));

			// execute
			StringValueReporter reporter = new StringValueReporter(_quoterMock.Object);
			var result = reporter.GetReport(portfolio);

			// validate
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Not.Empty);
		}

		private static Portfolio GenerateDefaultPortfolio()
		{
			Security goog = new Security {Name = "goog"};
			Security msft = new Security {Name = "msft"};
			Security aapl = new Security {Name = "aapl"};

			Portfolio portfolio = new Portfolio
			{
				Name = "po' boy",
				Accounts = new List<Account>()
			};

			Account mandingo = new Account
			{
				Name = "mandingo",
				Portfolio = portfolio,
				Positions = new List<Position>()
			};
			mandingo.Positions.Add(new Position
			{
				Account = mandingo,
				Count = 100,
				Security = goog
			});
			mandingo.Positions.Add(new Position
			{
				Account = mandingo,
				Count = 200,
				Security = aapl
			});
			portfolio.Accounts.Add(mandingo);

			Account took = new Account
			{
				Name = "took",
				Portfolio = portfolio,
				Positions = new List<Position>()
			};
			took.Positions.Add(new Position
			{
				Account = took,
				Count = 100,
				Security = msft
			});
			portfolio.Accounts.Add(took);

			return portfolio;
		}
	}
}
