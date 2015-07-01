using Contracts.Interfaces;

namespace Contracts
{
	public class CategoryValue : IDomainEntity
	{
		public int Id { get; set; }
		public Category Category { get; set; }
		public string Name { get; set; }
	}
}