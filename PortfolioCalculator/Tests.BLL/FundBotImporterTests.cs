using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Contracts;
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

			IEnumerable<Category> categories;
			IEnumerable<CategoryWeight> categoryWeights;
			fundBotImporter.GetCategoriesAndWeights(out categories, out categoryWeights);

			var categoryList = categories.ToList();
			var weightList = categoryWeights.ToList();

			Assert.That(categoryList.Count(), Is.EqualTo(3));
			var region = categoryList.Single(c => c.Name.Equals("Region", StringComparison.InvariantCultureIgnoreCase));
			var assetClass = categoryList.Single(c => c.Name.Equals("Asset Class", StringComparison.InvariantCultureIgnoreCase));
			var currency = categoryList.Single(c => c.Name.Equals("Currency", StringComparison.InvariantCultureIgnoreCase));
			Assert.That(region.Values.Count, Is.EqualTo(3));
			Assert.That(assetClass.Values.Count, Is.EqualTo(2));
			Assert.That(currency.Values.Count, Is.EqualTo(1));

			Assert.That(weightList.Count(), Is.EqualTo(12));
			Assert.That(weightList.Count(w => w.Security.Symbol.Equals("ZDV.TO", StringComparison.InvariantCultureIgnoreCase)), Is.EqualTo(3));
			Assert.That(weightList.Count(w => w.Security.Symbol.Equals("XSB.TO", StringComparison.InvariantCultureIgnoreCase)), Is.EqualTo(3));
			Assert.That(weightList.Count(w => w.Security.Symbol.Equals("XSU.TO", StringComparison.InvariantCultureIgnoreCase)), Is.EqualTo(3));
			Assert.That(weightList.Count(w => w.Security.Symbol.Equals("XIN.TO", StringComparison.InvariantCultureIgnoreCase)), Is.EqualTo(3));

			Assert.That(weightList.Count(w => w.Security.Symbol.Equals("ZDV.TO", StringComparison.InvariantCultureIgnoreCase) && 
				w.Value.Category.Name.Equals("Region", StringComparison.InvariantCultureIgnoreCase) && 
				w.Value.Name.Equals("CAD", StringComparison.InvariantCultureIgnoreCase)), Is.EqualTo(1));
			Assert.That(weightList.Count(w => w.Security.Symbol.Equals("ZDV.TO", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Category.Name.Equals("Currency", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Name.Equals("CAD", StringComparison.InvariantCultureIgnoreCase)), Is.EqualTo(1));
			Assert.That(weightList.Count(w => w.Security.Symbol.Equals("ZDV.TO", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Category.Name.Equals("Asset Class", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Name.Equals("Equity", StringComparison.InvariantCultureIgnoreCase)), Is.EqualTo(1));

			Assert.That(weightList.Count(w => w.Security.Symbol.Equals("XSB.TO", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Category.Name.Equals("Region", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Name.Equals("CAD", StringComparison.InvariantCultureIgnoreCase)), Is.EqualTo(1));
			Assert.That(weightList.Count(w => w.Security.Symbol.Equals("XSB.TO", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Category.Name.Equals("Currency", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Name.Equals("CAD", StringComparison.InvariantCultureIgnoreCase)), Is.EqualTo(1));
			Assert.That(weightList.Count(w => w.Security.Symbol.Equals("XSB.TO", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Category.Name.Equals("Asset Class", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Name.Equals("Bonds", StringComparison.InvariantCultureIgnoreCase)), Is.EqualTo(1));

			Assert.That(weightList.Count(w => w.Security.Symbol.Equals("XSU.TO", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Category.Name.Equals("Region", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Name.Equals("USD", StringComparison.InvariantCultureIgnoreCase)), Is.EqualTo(1));
			Assert.That(weightList.Count(w => w.Security.Symbol.Equals("XSU.TO", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Category.Name.Equals("Currency", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Name.Equals("CAD", StringComparison.InvariantCultureIgnoreCase)), Is.EqualTo(1));
			Assert.That(weightList.Count(w => w.Security.Symbol.Equals("XSU.TO", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Category.Name.Equals("Asset Class", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Name.Equals("Equity", StringComparison.InvariantCultureIgnoreCase)), Is.EqualTo(1));

			Assert.That(weightList.Count(w => w.Security.Symbol.Equals("XIN.TO", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Category.Name.Equals("Region", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Name.Equals("INTL", StringComparison.InvariantCultureIgnoreCase)), Is.EqualTo(1));
			Assert.That(weightList.Count(w => w.Security.Symbol.Equals("XIN.TO", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Category.Name.Equals("Currency", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Name.Equals("CAD", StringComparison.InvariantCultureIgnoreCase)), Is.EqualTo(1));
			Assert.That(weightList.Count(w => w.Security.Symbol.Equals("XIN.TO", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Category.Name.Equals("Asset Class", StringComparison.InvariantCultureIgnoreCase) &&
				w.Value.Name.Equals("Equity", StringComparison.InvariantCultureIgnoreCase)), Is.EqualTo(1));
		}
	}
}