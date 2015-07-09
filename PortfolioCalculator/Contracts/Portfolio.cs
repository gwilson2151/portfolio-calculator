using System.Collections.Generic;
using Contracts.Interfaces;

namespace Contracts
{
	public class Portfolio : IDomainEntity
    {
		public int Id { get; set; }
		public string Name { get; set; }
		public List<Account> Accounts { get; set; }
    }
}
