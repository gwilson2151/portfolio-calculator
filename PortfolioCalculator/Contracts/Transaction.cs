using System;

namespace Contracts
{
	public class Transaction
	{
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public Security Security { get; set; }
		public decimal Price { get; set; }
	}
}