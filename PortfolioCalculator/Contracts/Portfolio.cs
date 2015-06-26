using System.Collections.Generic;

namespace Contracts
{
    public class Portfolio
    {
		public int Id { get; set; }
		public string Name { get; set; }
		public IEnumerable<Account> Accounts { get; set; }
    }
}
