using System.Collections.Generic;

using Contracts;

namespace BLL.Interfaces
{
	public interface IPortfolioService
	{
		void UpdateWith(IEnumerable<Transaction> transactions);
	}
}