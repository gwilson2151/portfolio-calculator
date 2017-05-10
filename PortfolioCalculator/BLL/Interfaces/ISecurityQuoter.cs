using System.Collections.Generic;

using Contracts;

namespace BLL.Interfaces
{
	public interface ISecurityQuoter
	{
		IDictionary<Security, decimal> GetQuotes(IEnumerable<Security> securities);
	}
}