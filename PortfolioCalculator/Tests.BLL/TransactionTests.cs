using System;
using Contracts;
using NUnit.Framework;

namespace Tests.BLL
{
	[TestFixture]
	public class TransactionTests
	{
		[Test]
		public void When_Transaction_Has_Complete_Data_Then_Transaction_Is_Valid()
		{
			var transaction = new Transaction
			{
				Account = new Account { Name = "blah" },
				Date = DateTime.UtcNow,
				Price = 0M,
				Security = new Security { Symbol = "goog" },
				Shares = 10M,
				Type = TransactionType.Buy
			};

			Assert.That(transaction.Valid(), Is.True);
		}

		[Test]
		public void When_Transaction_Missing_Account_Then_Transaction_Is_Not_Valid()
		{
			var transaction = new Transaction
			{
				Account = null,
				Date = DateTime.UtcNow,
				Price = 0M,
				Security = new Security { Symbol = "goog" },
				Shares = 10M,
				Type = TransactionType.Buy
			};

			Assert.That(transaction.Valid(), Is.False);
		}

		[Test]
		public void When_Transaction_Missing_Security_Then_Transaction_Is_Not_Valid()
		{
			var transaction = new Transaction
			{
				Account = new Account { Name = "blah"},
				Date = DateTime.UtcNow,
				Price = 0M,
				Security = null,
				Shares = 10M,
				Type = TransactionType.Buy
			};

			Assert.That(transaction.Valid(), Is.False);
		}

		[Test]
		public void When_Transaction_Missing_Date_Then_Transaction_Is_Not_Valid()
		{
			var transaction = new Transaction
			{
				Account = new Account { Name = "blah" },
				Date = default(DateTime),
				Price = 0M,
				Security = new Security { Symbol = "goog" },
				Shares = 10M,
				Type = TransactionType.Buy
			};

			Assert.That(transaction.Valid(), Is.False);
		}

		[TestCase(0), TestCase(-1010)]
		public void When_Transaction_With_Invalid_Shares_Then_Transaction_Is_Not_Valid(decimal shares)
		{
			var transaction = new Transaction
			{
				Account = new Account { Name = "blah" },
				Date = default(DateTime),
				Price = 0M,
				Security = new Security { Symbol = "goog" },
				Shares = shares,
				Type = TransactionType.Buy
			};

			Assert.That(transaction.Valid(), Is.False);
		}

		[Test]
		public void When_Transaction_With_Invalid_Price_Then_Transaction_Is_Not_Valid()
		{
			var transaction = new Transaction
			{
				Account = new Account { Name = "blah" },
				Date = default(DateTime),
				Price = -12.56M,
				Security = new Security { Symbol = "goog" },
				Shares = 10M,
				Type = TransactionType.Buy
			};

			Assert.That(transaction.Valid(), Is.False);
		}
	}
}