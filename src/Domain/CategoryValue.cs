using System.Diagnostics;
using PortfolioSmarts.Domain.Interfaces;

namespace PortfolioSmarts.Domain
{
	[DebuggerDisplay("{Name}")]
	public class CategoryValue : IDomainEntity
	{
		public long Id { get; set; }
		public Category Category { get; set; }
		public string Name { get; set; }
	}
}
