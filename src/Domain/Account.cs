using System.Collections.Generic;
using PortfolioSmarts.Domain.Interfaces;

namespace PortfolioSmarts.Domain
{
	public class Account : IDomainEntity
	{
		public long Id { get; set; }
		public string ExternalId { get; set; }
		public string Name { get; set; }
		public Portfolio Portfolio { get; set; }
		public IEnumerable<Transaction> Transactions { get; set; }
		public IEnumerable<Position> Positions { get; set; }
		public IEnumerable<Balance> Balances { get; set; }

		public Account()
		{
			Transactions = new List<Transaction>();
			Positions = new List<Position>();
			Balances = new List<Balance>();
		}

		public Account(Portfolio portfolio)
			: this()
		{
			Portfolio = portfolio;
		}

		public Account(Account account)
		{
			Id = account.Id;
			ExternalId = account.ExternalId;
			Name = account.Name;
			Transactions = new List<Transaction>(account.Transactions);
			Positions = new List<Position>(account.Positions);
			Balances = new List<Balance>(account.Balances);
			Portfolio = Portfolio;
		}
	}
}
