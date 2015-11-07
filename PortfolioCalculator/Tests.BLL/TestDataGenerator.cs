using System;
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
			mandingo.Transactions.Add(new Transaction
			{
				Account = mandingo,
				Date = DateTime.UtcNow,
				Price = 12.34M,
				Security = goog,
				Shares = 100M,
				Type = TransactionType.Buy
			});
			mandingo.Positions.Add(new Position
			{
				Account = mandingo,
				Shares = 200M,
				Security = aapl
			});
			mandingo.Transactions.Add(new Transaction
			{
				Account = mandingo,
				Date = DateTime.UtcNow,
				Price = 23.45M,
				Security = aapl,
				Shares = 200M,
				Type = TransactionType.Buy
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
			took.Transactions.Add(new Transaction
			{
				Account = took,
				Date = DateTime.UtcNow,
				Price = 34.56M,
				Security = msft,
				Shares = 100M,
				Type = TransactionType.Buy
			});
			portfolio.Accounts.Add(took);

			return portfolio;
		}

		public static Portfolio GenerateEmptyPortfolio()
		{
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
