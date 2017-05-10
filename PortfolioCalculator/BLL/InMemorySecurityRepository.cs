using System;
using System.Collections.Generic;

using BLL.Interfaces;

using Contracts;

namespace BLL
{
	public class InMemorySecurityRepository : ISecurityRepository
	{
		private readonly Dictionary<string, Security> _securityCache = new Dictionary<string, Security>(StringComparer.InvariantCultureIgnoreCase);

		public Security Add(Security security)
		{
			_securityCache[security.Symbol] = security;
			return security;
		}

		public Security GetOrCreate(string exchange, string symbol)
		{
			var cacheKey = GetCacheKey(exchange, symbol);
			if (!_securityCache.ContainsKey(cacheKey))
				_securityCache[cacheKey] = new Security
				{
					Symbol = symbol,
					Exchange = exchange
				};
			return _securityCache[cacheKey];
		}

		//private string GetCacheKey(Security security)
		//{
		//	return GetCacheKey(security.Exchange, security.Symbol);
		//}

		private static string GetCacheKey(string exchange, string symbol)
		{
			return string.Format("{0}:{1}", exchange, symbol);
		}
	}
}