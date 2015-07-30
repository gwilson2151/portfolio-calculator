using System.Collections.Generic;
using Contracts.Interfaces;

namespace Contracts
{
	public class Portfolio : IDomainEntity
    {
		public long Id { get; set; }
		public string Name { get; set; }
		public IList<Account> Accounts { get; set; }
    }
}
