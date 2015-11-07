using System;
using System.Collections.Generic;
using System.Linq;
using BLL;
using Contracts;
using NUnit.Framework;

namespace Tests.BLL
{
	[TestFixture]
	public class PortfolioServiceTests
	{
		[Test]
		public void When_Updating_Empty_Portfolio_With_Transactions_Then_Portfolio_Is_Updated_To_Correct_State()
		{
			// setup
			var portfolio = TestDataGenerator.GenerateEmptyPortfolio();
			var portfolioService = new PortfolioService(portfolio);
			var mandingo = portfolio.Accounts.Single(a => a.Name.Equals("mandingo", StringComparison.InvariantCultureIgnoreCase));
			var took = portfolio.Accounts.Single(a => a.Name.Equals("took", StringComparison.InvariantCultureIgnoreCase));
			Security goog = new Security { Symbol = "goog" };
			Security msft = new Security { Symbol = "msft" };
			var transactionDate = DateTime.UtcNow;

			var transactions = new List<Transaction>();
			transactions.Add(new Transaction
			{
				Account = mandingo,
				Date = transactionDate,
				Price = 15.25M,
				Security = goog,
				Shares = 100M,
				Type = TransactionType.Buy
			});
			transactions.Add(new Transaction()
			{
				Account = took,
				Date = transactionDate.AddMinutes(14),
				Price = 21.54M,
				Security = msft,
				Shares = 50M,
				Type = TransactionType.Sell
			});
			transactions.Add(new Transaction()
			{
				Account = took,
				Date = transactionDate,
				Price = 21.34M,
				Security = msft,
				Shares = 200M,
				Type = TransactionType.Buy
			});

			// execute
			portfolioService.UpdateWith(transactions);

			// verify
			Assert.That(mandingo.Transactions.Count, Is.EqualTo(1));
			Assert.That(mandingo.Positions.Count, Is.EqualTo(1));
			var position = mandingo.Positions.Single();
			Assert.That(position.Security.Symbol, Is.EqualTo(goog.Symbol));
			Assert.That(position.Account, Is.EqualTo(mandingo));
			Assert.That(position.Shares, Is.EqualTo(100M));

			Assert.That(took.Transactions.Count, Is.EqualTo(2));
			Assert.That(took.Positions.Count, Is.EqualTo(1));
			position = took.Positions.Single();
			Assert.That(position.Security.Symbol, Is.EqualTo(msft.Symbol));
			Assert.That(position.Account, Is.EqualTo(took));
			Assert.That(position.Shares, Is.EqualTo(150M));
		}

		[Test]
		public void When_Updating_Portfolio_With_Invalid_Transaction_Then_Exception_Is_Thrown()
		{
			// setup
			var portfolio = TestDataGenerator.GenerateEmptyPortfolio();
			var portfolioService = new PortfolioService(portfolio);
			Security goog = new Security { Symbol = "goog" };
			var transaction = new Transaction
			{
				Account = null,
				Date = DateTime.UtcNow,
				Price = 0M,
				Security = goog,
				Shares = 10M,
				Type = TransactionType.Buy
			};

			// verify
			try
			{
				portfolioService.UpdateWith(new List<Transaction> { transaction });
			}
			catch (Exception ex)
			{
				Assert.That(ex.Message, Is.EqualTo("One or more transactions are invalid."));
			}
		}

		[Test]
		public void When_Updating_Account_With_Sell_Transaction_But_No_Position_Then_Exception_Is_Thrown()
		{
			// setup
			var portfolio = TestDataGenerator.GenerateEmptyPortfolio();
			var mandingo = portfolio.Accounts.Single(a => a.Name.Equals("mandingo", StringComparison.InvariantCultureIgnoreCase));
			var portfolioService = new PortfolioService(portfolio);
			Security goog = new Security { Symbol = "goog" };
			var transaction = new Transaction
			{
				Account = mandingo,
				Date = DateTime.UtcNow,
				Price = 0M,
				Security = goog,
				Shares = 10M,
				Type = TransactionType.Sell
			};

			// verify
			try
			{
				portfolioService.UpdateWith(new List<Transaction> { transaction });
			}
			catch (Exception ex)
			{
				Assert.That(ex.Message, Is.EqualTo("Tried to sell a security the account doesn't have."));
			}
		}

		[Test]
		public void When_Updating_Account_With_Sell_Transaction_With_More_Shares_Than_Position_Then_Exception_Is_Thrown()
		{
			// setup
			Security goog = new Security { Symbol = "goog" };
			var portfolio = TestDataGenerator.GenerateEmptyPortfolio();
			var mandingo = portfolio.Accounts.Single(a => a.Name.Equals("mandingo", StringComparison.InvariantCultureIgnoreCase));
			mandingo.Positions.Add(new Position
			{
				Account = mandingo,
				Shares = 1M,
				Security = goog
			});
			var portfolioService = new PortfolioService(portfolio);
			var transaction = new Transaction
			{
				Account = mandingo,
				Date = DateTime.UtcNow,
				Price = 0M,
				Security = goog,
				Shares = 10M,
				Type = TransactionType.Sell
			};

			// verify
			try
			{
				portfolioService.UpdateWith(new List<Transaction> { transaction });
			}
			catch (Exception ex)
			{
				Assert.That(ex.Message, Is.EqualTo("Tried to sell more shares than the account has."));
			}
		}

		[Test]
		public void When_Updating_Account_With_Sell_Transaction_Then_Account_Has_Shares_Removed()
		{
			// setup
			Security goog = new Security { Symbol = "goog" };
			var portfolio = TestDataGenerator.GenerateEmptyPortfolio();
			var mandingo = portfolio.Accounts.Single(a => a.Name.Equals("mandingo", StringComparison.InvariantCultureIgnoreCase));
			mandingo.Positions.Add(new Position
			{
				Account = mandingo,
				Shares = 100M,
				Security = goog
			});
			var portfolioService = new PortfolioService(portfolio);
			var transaction = new Transaction
			{
				Account = mandingo,
				Date = DateTime.UtcNow,
				Price = 0M,
				Security = goog,
				Shares = 10M,
				Type = TransactionType.Sell
			};

			// verify
			portfolioService.UpdateWith(new List<Transaction> { transaction });

			Assert.That(mandingo.Positions.Single().Shares, Is.EqualTo(90M));
		}

		[Test]
		public void When_Updating_Account_With_Sell_Transaction_Equal_To_Position_Then_Account_Has_Position_Removed()
		{
			// setup
			Security goog = new Security { Symbol = "goog" };
			var portfolio = TestDataGenerator.GenerateEmptyPortfolio();
			var mandingo = portfolio.Accounts.Single(a => a.Name.Equals("mandingo", StringComparison.InvariantCultureIgnoreCase));
			mandingo.Positions.Add(new Position
			{
				Account = mandingo,
				Shares = 100M,
				Security = goog
			});
			var portfolioService = new PortfolioService(portfolio);
			var transaction = new Transaction
			{
				Account = mandingo,
				Date = DateTime.UtcNow,
				Price = 0M,
				Security = goog,
				Shares = 100M,
				Type = TransactionType.Sell
			};

			// verify
			portfolioService.UpdateWith(new List<Transaction> { transaction });

			Assert.That(mandingo.Positions.Count, Is.EqualTo(0));
		}

		[Test]
		public void When_Updating_Account_With_Buy_Transaction_And_No_Position_Then_Account_Gains_Position()
		{
			// setup
			Security goog = new Security { Symbol = "goog" };
			var portfolio = TestDataGenerator.GenerateEmptyPortfolio();
			var mandingo = portfolio.Accounts.Single(a => a.Name.Equals("mandingo", StringComparison.InvariantCultureIgnoreCase));
			var portfolioService = new PortfolioService(portfolio);
			var transaction = new Transaction
			{
				Account = mandingo,
				Date = DateTime.UtcNow,
				Price = 0M,
				Security = goog,
				Shares = 10M,
				Type = TransactionType.Buy
			};

			// execute
			portfolioService.UpdateWith(new List<Transaction> { transaction });

			// verify
			Assert.That(mandingo.Positions.Single().Shares, Is.EqualTo(10M));
		}

		[Test]
		public void When_Updating_Account_With_Buy_Transaction_Then_Position_Is_Added_To()
		{
			// setup
			Security goog = new Security { Symbol = "goog" };
			var portfolio = TestDataGenerator.GenerateEmptyPortfolio();
			var mandingo = portfolio.Accounts.Single(a => a.Name.Equals("mandingo", StringComparison.InvariantCultureIgnoreCase));
			mandingo.Positions.Add(new Position
			{
				Account = mandingo,
				Shares = 100M,
				Security = goog
			});
			var portfolioService = new PortfolioService(portfolio);
			var transaction = new Transaction
			{
				Account = mandingo,
				Date = DateTime.UtcNow,
				Price = 0M,
				Security = goog,
				Shares = 10M,
				Type = TransactionType.Buy
			};

			// verify
			portfolioService.UpdateWith(new List<Transaction> { transaction });

			// verify
			Assert.That(mandingo.Positions.Single().Shares, Is.EqualTo(110M));
		}

		[Test]
		public void When_Updating_Account_With_Transaction_That_Already_Happened_Then_New_Transaction_Is_Ignored()
		{
			var portfolio = TestDataGenerator.GenerateDefaultPortfolio();
			var mandingo = portfolio.Accounts.Single(a => a.Name.Equals("mandingo", StringComparison.InvariantCultureIgnoreCase));
			Security yvr = new Security { Symbol = "yvr" };

			var transactions = new List<Transaction>();
			transactions.AddRange(mandingo.Transactions);
			transactions.Add(new Transaction
			{
				Account = mandingo,
				Date = DateTime.UtcNow,
				Price = 10M,
				Security = yvr,
				Shares = 50M,
				Type = TransactionType.Buy
			});

			var portfolioService = new PortfolioService(portfolio);
			portfolioService.UpdateWith(transactions);

			Assert.That(mandingo.Transactions.Count, Is.EqualTo(3));
			var googPosition = mandingo.Positions.Single(p => p.Security.Symbol.Equals("goog", StringComparison.InvariantCultureIgnoreCase));
			Assert.That(googPosition.Shares, Is.EqualTo(100M));
			var aaplPosition = mandingo.Positions.Single(p => p.Security.Symbol.Equals("aapl", StringComparison.InvariantCultureIgnoreCase));
			Assert.That(aaplPosition.Shares, Is.EqualTo(200M));
			var yvrPosition = mandingo.Positions.Single(p => p.Security.Symbol.Equals("yvr", StringComparison.InvariantCultureIgnoreCase));
			Assert.That(yvrPosition.Shares, Is.EqualTo(50M));
		}

		[Test]
		public void When_Updating_Account_Then_Update_Is_Atomic()
		{
			var portfolio = TestDataGenerator.GenerateDefaultPortfolio();
			var mandingo = portfolio.Accounts.Single(a => a.Name.Equals("mandingo", StringComparison.InvariantCultureIgnoreCase));
			Security yvr = new Security { Symbol = "yvr" };
			Security goog = new Security { Symbol = "goog" };

			var transactions = new List<Transaction>(2)
			{
				new Transaction
				{
					Account = mandingo,
					Date = DateTime.UtcNow,
					Price = 10M,
					Security = yvr,
					Shares = 100M,
					Type = TransactionType.Buy
				},
				new Transaction
				{
					Account = mandingo,
					Date = DateTime.UtcNow,
					Price = 10M,
					Security = goog,
					Shares = 10M,
					Type = TransactionType.Sell
				},
				new Transaction
				{
					Account = mandingo,
					Date = DateTime.UtcNow,
					Price = 10M,
					Security = new Security { Symbol = "not_owned" },
					Shares = 100M,
					Type = TransactionType.Sell
				}
			};

			var portfolioService = new PortfolioService(portfolio);
			Assert.Throws<Exception>(() => portfolioService.UpdateWith(transactions));

			Assert.That(mandingo.Positions.Count, Is.EqualTo(2));
			var googPosition = mandingo.Positions.Single(p => p.Security.Symbol.Equals(goog.Symbol, StringComparison.InvariantCultureIgnoreCase));
			Assert.That(googPosition.Shares, Is.EqualTo(100M));
		}
	}
}
