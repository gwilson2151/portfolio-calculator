using System.Diagnostics;
using Contracts.Interfaces;

namespace Contracts
{
	[DebuggerDisplay("{Name}")]
	public class CategoryValue : IDomainEntity
	{
		public long Id { get; set; }
		public Category Category { get; set; }
		public string Name { get; set; }
	}
}