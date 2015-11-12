using System.Collections.Generic;
using Contracts.Interfaces;

namespace Contracts
{
	public class Account : IDomainEntity
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public Portfolio Portfolio { get; set; }
		public List<Transaction> Transactions { get; set; }
		public List<Position> Positions { get; set; }

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