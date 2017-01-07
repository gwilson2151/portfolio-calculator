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
			var portfolio = TestDataGenerator.GenerateDefaultPortfolio();
			_quoterMock.Setup(m => m.GetQuotes(It.IsAny<IEnumerable<Security>>())).Returns(new Dictionary<Security, decimal>
			{
				{new Security { Symbol = "goog"}, 18.25M},
				{new Security { Symbol = "msft"}, 15M},
				{new Security { Symbol = "aapl"}, 9.36M},
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

		[Test]
		public void When_SecurityQuoter_Cannot_Find_Quote_Then_Report_Generation_Error_Message_Is_Written_To_String()
		{
			// setup
			var portfolio = TestDataGenerator.GenerateDefaultPortfolio();
			_quoterMock.Setup(m => m.GetQuotes(It.IsAny<IEnumerable<Security>>())).Returns(new Dictionary<Security, decimal>
			{
				{new Security { Symbol = "goog"}, new decimal(18.25)},
				{new Security { Symbol = "msft"}, new decimal(15)},
			});

			// execute
			StringValueReporter reporter = new StringValueReporter(_quoterMock.Object);
			var result = reporter.GetReport(portfolio);

			// validate
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Not.Empty);

			const string expected = @"po' boy total = 3325.00
mandingo total = 1825.00
goog: 100 x 18.25 = 1825.00
aapl: 200 x quote not found = unknown
took total = 1500
msft: 100 x 15 = 1500
";
			Assert.That(result, Is.EqualTo(expected));
		}
	}
}
