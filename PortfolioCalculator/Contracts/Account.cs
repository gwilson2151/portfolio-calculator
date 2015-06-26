using System.Collections.Generic;

namespace Contracts
{
	public class Account
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public Portfolio Portfolio { get; set; }
		public IEnumerable<Transaction> Transactions { get; set; }
		public IEnumerable<Position> Positions { get; set; }
	}
}