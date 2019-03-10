using System.Collections.Generic;
using PortfolioSmarts.Domain.Interfaces;

namespace PortfolioSmarts.Domain
{
	public class Account : IDomainEntity
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public Portfolio Portfolio { get; set; }
		public IEnumerable<Transaction> Transactions { get; set; }
		public IEnumerable<Position> Positions { get; set; }

		public Account()
		{
			Transactions = new List<Transaction>();
			Positions = new List<Position>();
		}

		public Account(Portfolio portfolio)
			: this()
		{
			Portfolio = portfolio;
		}
	}
}