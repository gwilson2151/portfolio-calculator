using System.Collections.Generic;

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
				Name = "po' boy",
				Accounts = new List<Account>()
			};

			Account mandingo = new Account
			{
				Name = "mandingo",
				Portfolio = portfolio,
				Positions = new List<Position>(),
				Transactions = new List<Transaction>()
			};
			mandingo.Positions.Add(new Position
			{
				Account = mandingo,
				Shares = 100M,
				Security = goog
			});
			mandingo.Positions.Add(new Position
			{
				Account = mandingo,
				Shares = 200M,
				Security = aapl
			});
			portfolio.Accounts.Add(mandingo);

			Account took = new Account
			{
				Name = "took",
				Portfolio = portfolio,
				Positions = new List<Position>(),
				Transactions = new List<Transaction>()
			};
			took.Positions.Add(new Position
			{
				Account = took,
				Shares = 100M,
				Security = msft
			});
			portfolio.Accounts.Add(took);

			return portfolio;
		}

		public static Portfolio GenerateEmptyPortfolio() {
			Portfolio portfolio = new Portfolio
			{
				Name = "po' boy",
				Accounts = new List<Account>()
			};
			Account mandingo = new Account
			{
				Name = "mandingo",
				Portfolio = portfolio,
				Positions = new List<Position>(),
				Transactions = new List<Transaction>()
			};
			portfolio.Accounts.Add(mandingo);

			Account took = new Account
			{
				Name = "took",
				Portfolio = portfolio,
				Positions = new List<Position>(),
				Transactions = new List<Transaction>()
			};
			portfolio.Accounts.Add(took);

			return portfolio;
		}
	}
}