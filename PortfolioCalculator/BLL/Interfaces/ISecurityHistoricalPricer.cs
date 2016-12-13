using System;
using System.Collections.Generic;
using Contracts;

namespace BLL.Interfaces
{
	public interface ISecurityHistoricalPricer
	{
		IDictionary<DateTime, IDictionary<string, decimal>> GetHistoricalPrices(IEnumerable<Security> securities, DateTime start, DateTime end, Contracts.Period period); 
	}
}