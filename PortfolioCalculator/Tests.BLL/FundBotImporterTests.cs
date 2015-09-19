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

			Assert.That(transactions.Count(), Is.EqualTo(1));
			var transaction = transactions.Single();
			Assert.That(transaction.Security.Symbol, Is.EqualTo("XFN.TO"));
			Assert.That(transaction.Price, Is.EqualTo(29.97M));
			Assert.That(transaction.Shares, Is.EqualTo(100M));
			Assert.That(transaction.Date, Is.EqualTo(new DateTime(2014, 5, 6)));
		}
	}
}