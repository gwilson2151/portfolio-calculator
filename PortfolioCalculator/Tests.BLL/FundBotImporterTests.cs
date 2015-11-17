using System;
using System.IO;
using System.Linq;
using DataGatherer;
using LumenWorks.Framework.IO.Csv;
using NUnit.Framework;

namespace Tests.BLL
{
	[TestFixture]
	public class FundBotImporterTests
	{
		
		[Test]
		public void When_FundBotImporter_Given_Reader_Then_Returns_Transactions()
		{
			const string test = "XFN.TO,06/05/2014,100,29.97,CAD,CAD,Equity";
			var csvReader = new CsvReader(new StringReader(test), false);
			var fundBotImporter = new FundBotImporter(csvReader);

			var transactions = fundBotImporter.GetTransactions().ToList();

			Assert.That(transactions.Count, Is.EqualTo(1));
			var transaction = transactions.Single();
			Assert.That(transaction.Security.Symbol, Is.EqualTo("XFN.TO"));
			Assert.That(transaction.Price, Is.EqualTo(29.97M));
			Assert.That(transaction.Shares, Is.EqualTo(100M));
			Assert.That(transaction.Date, Is.EqualTo(new DateTime(2014, 5, 6)));
		}

		[Test]
		public void When_FundBotImporter_Given_Reader_Then_Returns_Categories()
		{
			const string test = @"ZDV.TO,CAD,CAD,Equity
XSB.TO,CAD,CAD,Bonds
XSU.TO,CAD,USD,Equity
XIN.TO,CAD,INTL,Equity";
			var csvReader = new CsvReader(new StringReader(test), false);
			var fundBotImporter = new FundBotImporter(csvReader);

			var categories = fundBotImporter.GetCategories().ToList();

			Assert.That(categories.Count, Is.EqualTo(3));
			var region = categories.Single(c => c.Name.Equals("region", StringComparison.InvariantCultureIgnoreCase));
			var assetClass = categories.Single(c => c.Name.Equals("assetclass", StringComparison.InvariantCultureIgnoreCase));
			var currency = categories.Single(c => c.Name.Equals("currency", StringComparison.InvariantCultureIgnoreCase));
			Assert.That(region.Values.Count, Is.EqualTo(3));
			Assert.That(assetClass.Values.Count, Is.EqualTo(2));
			Assert.That(currency.Values.Count, Is.EqualTo(1));
		}
	}
}