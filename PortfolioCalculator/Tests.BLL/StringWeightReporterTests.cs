using System.Collections.Generic;
using System.Linq;
using BLL;
using BLL.Interfaces;
using Contracts;
using Moq;
using NUnit.Framework;

namespace Tests.BLL
{
	[TestFixture]
	public class StringWeightReporterTests
	{
		private Mock<ISecurityQuoter> _quoterMock;

		[SetUp]
		public void SetUp()
		{
			_quoterMock = new Mock<ISecurityQuoter>();
		}

		[Test]
		public void When_StringWeightReporter_Given_Fundbot_Portfolio_Then_Report_Is_Written_To_String()
		{
			// setup
			var portfolio = TestDataGenerator.GenerateFundbotPortfolio();
			var categories = TestDataGenerator.GenerateFundbotCategories().ToList();
			var weights = TestDataGenerator.GenerateFundbotWeights(categories).ToList();

			_quoterMock.Setup(m => m.GetQuotes(It.IsAny<IEnumerable<Security>>())).Returns(new Dictionary<Security, decimal>
			{
				{new Security { Symbol = "XFN.TO"}, 29.97M},
				{new Security { Symbol = "AGG"}, 1000.69M},
				{new Security { Symbol = "XIU.TO"}, 21.1M},
				{new Security { Symbol = "CPD.TO"}, 16.55M},
				{new Security { Symbol = "EFA"}, 68.24M},
			});

			// execute
			StringWeightReporter reporter = new StringWeightReporter(_quoterMock.Object);
			var result = reporter.GetReport(portfolio, categories, weights);

			// validate
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Not.Empty);

			const string expected = @"Portfolio: {0}

Region
USD: 57.2%
CAD: 28.3%
INTL: 14.5%

Asset Class
Bonds: 57.2%
Equity: 25.3%
Preferred: 17.5%

Currency
USD: 71.7%
CAD: 28.3%
";
			Assert.That(result, Is.EqualTo(string.Format(expected, portfolio.Name)));
		}
	}
}