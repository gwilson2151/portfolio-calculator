using System.Collections.Generic;

using Contracts;

namespace BLL.Interfaces
{
	public interface ISecurityQuoter
	{
		IDictionary<string, decimal> GetQuotes(IEnumerable<Security> securities);
	}
}