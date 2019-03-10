using System.Diagnostics;
using PortfolioSmarts.Domain.Interfaces;

namespace PortfolioSmarts.Domain
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