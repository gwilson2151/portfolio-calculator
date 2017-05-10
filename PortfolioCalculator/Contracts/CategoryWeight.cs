using System.Diagnostics;
using Contracts.Interfaces;

namespace Contracts
{
	[DebuggerDisplay("{Value.Name, Weight}")]
	public class CategoryWeight : IDomainEntity
	{
		public long Id { get; set; }
		public CategoryValue Value { get; set; }
		public decimal Weight { get; set; }
		public Security Security { get; set; }
	}
}