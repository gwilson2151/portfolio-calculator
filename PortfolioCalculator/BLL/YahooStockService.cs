using System.Collections.Generic;

using BLL.Interfaces;

using Contracts;

using YSQ.core.Quotes;

namespace BLL
{
	public class YahooStockService : ISecurityQuoter
	{
		private readonly IBuildQuotes _quoteBuilder;

		public YahooStockService(IBuildQuotes quoteBuilder)
		{
			_quoteBuilder = quoteBuilder;
		}

		public IDictionary<string, decimal> GetQuotes(IEnumerable<Security> security)
		{
			throw new System.NotImplementedException();
		}
	}
}