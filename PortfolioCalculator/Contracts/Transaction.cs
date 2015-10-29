using System;
using Contracts.Interfaces;

namespace Contracts
{
	public class Transaction : IDomainEntity
	{
		public long Id { get; set; }
		public Account Account { get; set; }
		public DateTime Date { get; set; }
		public Security Security { get; set; }
		public decimal Price { get; set; }
		public decimal Shares { get; set; }
		public TransactionType Type { get; set; }

		public bool Valid() {
			if (Account != null && Date != default(DateTime) && Security != null && Shares > 0M)
				return true;
			return false;
		}
	}
}