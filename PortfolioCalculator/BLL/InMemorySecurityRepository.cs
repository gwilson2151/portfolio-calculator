using System;
using System.Collections.Generic;

using BLL.Interfaces;

using Contracts;

namespace BLL
{
	public class InMemorySecurityRepository : ISecurityRepository
	{
		private readonly Dictionary<string, Security> _securityCache = new Dictionary<string, Security>(StringComparer.InvariantCultureIgnoreCase);

		public Security GetBySymbol(string symbol)
		{
			if (!_securityCache.ContainsKey(symbol))
				_securityCache[symbol] = new Security
				{
					Symbol = symbol
				};

			return _securityCache[symbol];
		}
	}
}