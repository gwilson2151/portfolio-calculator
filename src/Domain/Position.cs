using System.Diagnostics;
using Contracts.Interfaces;

namespace Contracts
{
	[DebuggerDisplay("Security={Security.Symbol} Shares={Shares}")]
	public class Position : IDomainEntity
	{
		public long Id { get; set; }
		public Account Account { get; set; }
		public Security Security { get; set; }
		public decimal Shares { get; set; }
	}
}