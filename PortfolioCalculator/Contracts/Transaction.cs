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
	}
}