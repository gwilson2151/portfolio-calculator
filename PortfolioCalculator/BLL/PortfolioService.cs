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

		public PortfolioService(Portfolio portfolio)
		{
			_portfolio = portfolio;
		}

		// This method already feels like it needs to be broken up...
		// Also needs to have atomicity added.
		public void UpdateWith(IEnumerable<Transaction> transactions)
		{
			if (transactions.Any(t => !t.Valid()))
				throw new Exception("One or more transactions are invalid.");

			foreach (var transactionAccount in transactions.Select(t => t.Account).Distinct())
			{
				var account = _portfolio.Accounts.Single(a => a.Name.Equals(transactionAccount.Name, StringComparison.InvariantCultureIgnoreCase));
				var transactionsForAccount = transactions.Where(t => t.Account.Name.Equals(account.Name, StringComparison.InvariantCultureIgnoreCase))
														 .OrderBy(t => t.Date);

				foreach (var transaction in transactionsForAccount)
				{
					var position = account.Positions.SingleOrDefault(p => p.Security.Symbol.Equals(transaction.Security.Symbol));

					if (position == null)
					{
						if (transaction.Type == TransactionType.Sell)
							throw new Exception("Tried to sell a security the account doesn't have.");
						else
						{
							account.Positions.Add(new Position
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
								account.Positions.Remove(position);
						}
						else
						{
							position.Shares += transaction.Shares;
						}
					}

					transaction.Account = account;
					account.Transactions.Add(transaction);
				}
			}
		}
	}
}