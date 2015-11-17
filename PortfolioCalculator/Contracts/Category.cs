using System.Collections.Generic;

using Contracts.Interfaces;

namespace Contracts
{
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