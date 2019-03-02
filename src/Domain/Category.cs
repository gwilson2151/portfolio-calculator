using System.Collections.Generic;
using System.Diagnostics;
using Contracts.Interfaces;

namespace Contracts
{
	[DebuggerDisplay("{Name}")]
	public class Category : IDomainEntity
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public IList<CategoryValue> Values { get; set; }

		public Category()
		{
			Values = new List<CategoryValue>();
		}
	}
}