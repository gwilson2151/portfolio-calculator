using System;
using Contracts;
using NUnit.Framework;

namespace Tests.BLL
{
	[TestFixture]
	public class TransactionTests
	{
		[Test]
		public void When_Transaction_Missing_Account_Then_Transaction_Is_Not_Valid()
		{
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
			Security goog = new Security { Symbol = "goog" };
			var transaction = new Transaction
			{
				Account = new Account { Name = "blah" },
				Date = default(DateTime),
				Price = 0M,
				Security = goog,
				Shares = 10M,
				Type = TransactionType.Buy
			};

			Assert.That(transaction.Valid(), Is.False);
		}

		[TestCase(0), TestCase(-1010)]
		public void When_Transaction_With_Invalid_Shares_Then_Transaction_Is_Not_Valid(decimal shares)
		{
			Security goog = new Security { Symbol = "goog" };
			var transaction = new Transaction
			{
				Account = new Account { Name = "blah" },
				Date = default(DateTime),
				Price = 0M,
				Security = goog,
				Shares = shares,
				Type = TransactionType.Buy
			};

			Assert.That(transaction.Valid(), Is.False);
		}
	}
}