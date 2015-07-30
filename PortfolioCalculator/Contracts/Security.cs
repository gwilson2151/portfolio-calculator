using Contracts.Interfaces;

namespace Contracts
{
	public class Security : IDomainEntity
	{
		public long Id { get; set; }
		public string Symbol { get; set; }
	}
}