using System;

using BLL;
using BLL.Factories;

using Contracts;

using NUnit.Framework;

namespace Tests.BLL
{
	[TestFixture, Explicit]
	public class QuestradeApiTestBed
	{
		[Test]
		public void Test()
		{
			var portfolio = new Portfolio
			{
				Name = "Test Portfolio"
			};
			using (var tokenManager = new QuestradeApiTokenManager(new Configuration()))
			{
				var api = new QuestradeService(tokenManager, new InMemorySecurityRepository());
				var accounts = api.GetAccounts();

				foreach (var account in accounts)
				{
					account.Positions = api.GetPositions(account);
				}

				portfolio.Accounts = accounts;
			}

			var quoter = new YahooStockService(new QuoteServiceFactory());
			var reporter = new StringValueReporter(quoter);
			var report = reporter.GetReport(portfolio);

			Console.Out.Write(report);
		}
	}
}