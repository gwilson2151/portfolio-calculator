using System.Collections.Generic;
using System.Diagnostics;
using PortfolioSmarts.Domain.Interfaces;

namespace PortfolioSmarts.Domain
{
	[DebuggerDisplay("{Name}")]
	public class Category : IDomainEntity
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public ICollection<CategoryValue> Values { get; set; }

		public Category()
		{
			Values = new List<CategoryValue>();
		}
	}
}