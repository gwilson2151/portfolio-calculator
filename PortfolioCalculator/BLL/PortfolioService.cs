using System.Collections.Generic;

using BLL.Interfaces;

using Contracts;

namespace BLL
{
	public class PortfolioService : IPortfolioService
	{
		private readonly Portfolio _portfolio;

		public PortfolioService(Portfolio portfolio)
		{
			_portfolio = portfolio;
		}

		public void UpdateWith(IEnumerable<Transaction> transactions)
		{
			throw new System.NotImplementedException();
		}
	}
}