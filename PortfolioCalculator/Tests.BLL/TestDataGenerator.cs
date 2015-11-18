using System;
using System.Collections.Generic;
using System.Linq;
using Contracts;

namespace Tests.BLL
{
	public static class TestDataGenerator
	{
		public static Portfolio GenerateDefaultPortfolio()
		{
			Security goog = new Security {Symbol = "goog"};
			Security msft = new Security {Symbol = "msft"};
			Security aapl = new Security {Symbol = "aapl"};

			Portfolio portfolio = new Portfolio
			{
				Name = "po' boy"
			};

			Account mandingo = new Account
			{
				Name = "mandingo",
				Portfolio = portfolio
			};
			AddPosition(mandingo, goog, 100M, 12.34M);
			AddPosition(mandingo, aapl, 200M, 23.45M);
			portfolio.Accounts.Add(mandingo);

			Account took = new Account
			{
				Name = "took",
				Portfolio = portfolio
			};
			AddPosition(took, msft, 100M, 34.56M);
			portfolio.Accounts.Add(took);

			return portfolio;
		}

		public static Portfolio GenerateEmptyPortfolio()
		{
			Portfolio portfolio = new Portfolio
			{
				Name = "po' boy"
			};
			Account mandingo = new Account
			{
				Name = "mandingo",
				Portfolio = portfolio
			};
			portfolio.Accounts.Add(mandingo);

			Account took = new Account
			{
				Name = "took",
				Portfolio = portfolio
			};
			portfolio.Accounts.Add(took);

			return portfolio;
		}

		public static Portfolio GenerateFundbotPortfolio()
		{
			Security xfn = new Security { Symbol = "XFN.TO" };
			Security agg = new Security { Symbol = "AGG" };
			Security xiu = new Security { Symbol = "XIU.TO" };
			Security cpd = new Security { Symbol = "CPD.TO" };
			Security efa = new Security { Symbol = "EFA" };

			Portfolio portfolio = new Portfolio
			{
				Name = Guid.NewGuid().ToString()
			};
			Account account = new Account
			{
				Name = portfolio.Name,
				Portfolio = portfolio
			};
			portfolio.Accounts.Add(account);

			AddPosition(account, xfn, 100M, 29.97M);
			AddPosition(account, agg, 27M, 1000.69M);
			AddPosition(account, xiu, 100M, 21.1M);
			AddPosition(account, cpd, 500M, 16.55M);
			AddPosition(account, efa, 100M, 68.24M);

			return portfolio;
		}

		public static IEnumerable<Category> GenerateFundbotCategories()
		{
			var region = new Category { Name = "Region" };
			var assetClass = new Category { Name = "Asset Class" };
			var currency = new Category { Name = "Currency" };

			region.Values.Add(new CategoryValue { Category = region, Name = "CAD"});
			region.Values.Add(new CategoryValue { Category = region, Name = "USD" });
			region.Values.Add(new CategoryValue { Category = region, Name = "INTL" });

			assetClass.Values.Add(new CategoryValue { Category = assetClass, Name = "Equity" });
			assetClass.Values.Add(new CategoryValue { Category = assetClass, Name = "Bonds" });
			assetClass.Values.Add(new CategoryValue { Category = assetClass, Name = "Preferred" });

			currency.Values.Add(new CategoryValue { Category = currency, Name = "CAD" });
			currency.Values.Add(new CategoryValue { Category = currency, Name = "USD" });

			return new List<Category> { region, assetClass, currency };
		}

		public static IEnumerable<CategoryWeight> GenerateFundbotWeights(IEnumerable<Category> fundbotCategories)
		{
			Security xfn = new Security { Symbol = "XFN.TO" };
			Security agg = new Security { Symbol = "AGG" };
			Security xiu = new Security { Symbol = "XIU.TO" };
			Security cpd = new Security { Symbol = "CPD.TO" };
			Security efa = new Security { Symbol = "EFA" };

			var categories = fundbotCategories.ToList();

			var regionCad = categories.Single(c => c.Name.Equals("Region")).Values.Single(v => v.Name.Equals("CAD"));
			var regionUsd = categories.Single(c => c.Name.Equals("Region")).Values.Single(v => v.Name.Equals("USD"));
			var regionIntl = categories.Single(c => c.Name.Equals("Region")).Values.Single(v => v.Name.Equals("INTL"));

			var currencyCad = categories.Single(c => c.Name.Equals("Currency")).Values.Single(v => v.Name.Equals("CAD"));
			var currencyUsd = categories.Single(c => c.Name.Equals("Currency")).Values.Single(v => v.Name.Equals("USD"));

			var assetClassEquity = categories.Single(c => c.Name.Equals("Asset Class")).Values.Single(v => v.Name.Equals("Equity"));
			var assetClassBonds = categories.Single(c => c.Name.Equals("Asset Class")).Values.Single(v => v.Name.Equals("Bonds"));
			var assetClassPreferred = categories.Single(c => c.Name.Equals("Asset Class")).Values.Single(v => v.Name.Equals("Preferred"));

			var weights = new List<CategoryWeight>
			{
				new CategoryWeight { Security = xfn, Value = currencyCad, Weight = 100M },
				new CategoryWeight { Security = xfn, Value = regionCad, Weight = 100M },
				new CategoryWeight { Security = xfn, Value = assetClassEquity, Weight = 100M },
				new CategoryWeight { Security = agg, Value = currencyUsd, Weight = 100M },
				new CategoryWeight { Security = agg, Value = regionUsd, Weight = 100M },
				new CategoryWeight { Security = agg, Value = assetClassBonds, Weight = 100M },
				new CategoryWeight { Security = xiu, Value = currencyCad, Weight = 100M },
				new CategoryWeight { Security = xiu, Value = regionCad, Weight = 100M },
				new CategoryWeight { Security = xiu, Value = assetClassEquity, Weight = 100M },
				new CategoryWeight { Security = cpd, Value = currencyCad, Weight = 100M },
				new CategoryWeight { Security = cpd, Value = regionCad, Weight = 100M },
				new CategoryWeight { Security = cpd, Value = assetClassPreferred, Weight = 100M },
				new CategoryWeight { Security = efa, Value = currencyUsd, Weight = 100M },
				new CategoryWeight { Security = efa, Value = regionIntl, Weight = 100M },
				new CategoryWeight { Security = efa, Value = assetClassEquity, Weight = 100M }
			};

			return weights;
		}

		private static void AddPosition(Account account, Security security, decimal shares, decimal price)
		{
			account.Positions.Add(new Position
			{
				Account = account,
				Shares = shares,
				Security = security
			});
			account.Transactions.Add(new Transaction
			{
				Account = account,
				Date = DateTime.UtcNow,
				Price = price,
				Security = security,
				Shares = shares,
				Type = TransactionType.Buy
			});
		}
	}
}
