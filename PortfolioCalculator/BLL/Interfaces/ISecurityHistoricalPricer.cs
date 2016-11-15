using System;
using System.Collections.Generic;
using Contracts;

namespace BLL.Interfaces
{
	public interface ISecurityHistoricalPricer
	{
		IDictionary<DateTime, IDictionary<string, decimal>> GetHistoricalPrices(IEnumerable<DateTime> dates, IEnumerable<Security> securities); 
	}
}