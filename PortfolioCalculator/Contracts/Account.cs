using System.Collections.Generic;
using Contracts.Interfaces;

namespace Contracts
{
	public class Account : IDomainEntity
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public Portfolio Portfolio { get; set; }
		public IList<Transaction> Transactions { get; set; }
		public IList<Position> Positions { get; set; }
	}
}