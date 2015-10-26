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