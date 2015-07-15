using System;
using System.Collections.Generic;

using Moq;
using NUnit.Framework;

using BLL;
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
		public void When_StringValueReporter_Given_Valid_Portfolio_Then_Report_Is_Written_To_String()
		{
			// setup
			var portfolio = GenerateDefaultPortfolio();
			_quoterMock.Setup(m => m.GetQuotes(It.IsAny<IEnumerable<Security>>())).Returns(new Dictionary<string, decimal>
			{
				{"goog", new decimal(18.25)},
				{"msft", new decimal(15)},
				{"aapl", new decimal(9.36)},
			});

			// execute
			StringValueReporter reporter = new StringValueReporter(_quoterMock.Object);
			var result = reporter.GetReport(portfolio);

			// validate
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Not.Empty);

			const string expected = @"po' boy total = 5197.00
mandingo total = 3697.00
goog: 100 x 18.25 = 1825.00
aapl: 200 x 9.36 = 1872.00
took total = 1500
msft: 100 x 15 = 1500
";
			Assert.That(result, Is.EqualTo(expected));
		}

		[Test, Ignore]
		public void When_SecurityQuoter_Cannot_Find_Quote_Then_Report_Generation_Error_Message_Is_Written_To_String()
		{
			throw new NotImplementedException();
		}

		private static Portfolio GenerateDefaultPortfolio()
		{
			Security goog = new Security {Symbol = "goog"};
			Security msft = new Security {Symbol = "msft"};
			Security aapl = new Security {Symbol = "aapl"};

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
