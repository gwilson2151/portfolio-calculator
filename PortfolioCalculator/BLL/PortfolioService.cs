using System;
using System.Collections.Generic;
using System.Linq;
using BLL.Interfaces;

using Contracts;

namespace BLL
{
	public class PortfolioService : IPortfolioService
	{
		private readonly Portfolio _portfolio;

		private class AccountUpdate
		{
			public List<Transaction> Transactions { get; private set; }
			public List<Position> Positions { get; private set; }

			public AccountUpdate(IEnumerable<Transaction> transactions, IEnumerable<Position> positions)
			{
				Positions = positions.ToList();
				Transactions = transactions.ToList();
			}
		}

		public PortfolioService(Portfolio portfolio)
		{
			_portfolio = portfolio;
		}

		// This method already feels like it needs to be broken up...
		// Also needs to have atomicity added.
		public void UpdateWith(IEnumerable<Transaction> transactions)
		{
			var transactionList = transactions.ToList();

			if (transactionList.Any(t => !t.Valid()))
				throw new Exception("One or more transactions are invalid.");

			var updates =  new Dictionary<Account, AccountUpdate>();

			foreach (var transactionAccount in transactionList.Select(t => t.Account).Distinct())
			{
				var account = _portfolio.Accounts.Single(a => a.Name.Equals(transactionAccount.Name, StringComparison.InvariantCultureIgnoreCase));
				var transactionsForAccount = transactionList.Where(t => t.Account.Name.Equals(account.Name, StringComparison.InvariantCultureIgnoreCase))
														 .OrderBy(t => t.Date);

				var update = CalculateAccountUpdate(transactionsForAccount, account);
				updates.Add(transactionAccount, update);
			}

			foreach (var kvPair in updates)
			{
				kvPair.Key.Positions = kvPair.Value.Positions;
				kvPair.Key.Transactions = kvPair.Value.Transactions;
			}
		}

		private static AccountUpdate CalculateAccountUpdate(IOrderedEnumerable<Transaction> transactionsForAccount, Account account)
		{
			var positions = ClonePositions(account.Positions);
			var transactions = new List<Transaction>(account.Transactions);

			foreach (var transaction in transactionsForAccount)
			{
				var position = positions.SingleOrDefault(p => p.Security.Symbol.Equals(transaction.Security.Symbol, StringComparison.InvariantCultureIgnoreCase));

				// this if/else should live in account
				if (position == null)
				{
					if (transaction.Type == TransactionType.Sell)
						throw new Exception("Tried to sell a security the account doesn't have.");
					else
					{
						positions.Add(new Position
						{
							Account = account,
							Shares = transaction.Shares,
							Security = transaction.Security
						});
					}
				}
				else
				{
					if (transaction.Type == TransactionType.Sell)
					{
						if (position.Shares < transaction.Shares)
							throw new Exception("Tried to sell more shares than the account has.");
						position.Shares -= transaction.Shares;

						if (position.Shares == 0M)
							positions.Remove(position);
					}
					else
					{
						position.Shares += transaction.Shares;
					}
				}

				transaction.Account = account;
				transactions.Add(transaction);
			}

			return new AccountUpdate(transactions, positions);
		}

		private static List<Position> ClonePositions(List<Position> positions)
		{
			return positions.Select(position => new Position
			{
				Id = position.Id,
				Account = position.Account,
				Security = position.Security,
				Shares = position.Shares
			}).ToList();
		}
	}
}