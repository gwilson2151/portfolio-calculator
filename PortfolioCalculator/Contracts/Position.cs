using Contracts.Interfaces;

namespace Contracts
{
	public class Position : IDomainEntity
	{
		public long Id { get; set; }
		public Account Account { get; set; }
		public Security Security { get; set; }
		public decimal Shares { get; set; }
	}
}