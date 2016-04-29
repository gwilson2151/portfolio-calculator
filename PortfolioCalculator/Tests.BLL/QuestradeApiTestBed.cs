using System;
using System.Collections.Generic;
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
				portfolio.Accounts = api.GetAccounts();

				foreach (var account in portfolio.Accounts)
				{
					account.Positions = api.GetPositions(account);
					account.Transactions = api.GetTransactions(account, new DateTime(2008, 1, 1), DateTime.Now);
				}
			}

			var quoter = new YahooStockService(new QuoteServiceFactory());
			var reporter = new StringValueReporter(quoter);
			var report = reporter.GetReport(portfolio);

			Console.Out.Write(report);
		}

        [Test]
        public void SymbolTest()
        {
            var symbols = new List<Security>
            {
                new Security {Symbol = "XCS.TO"},
                new Security {Symbol = "XIU.TO"},
                new Security {Symbol = "GOOG"},
            };
            using (var tokenManager = new QuestradeApiTokenManager(new Configuration()))
            {
                var api = new QuestradeService(tokenManager, new InMemorySecurityRepository());

                var quotes = api.GetSymbols(symbols);
            }
        }

	    [Test]
	    public void QuoteTest()
	    {
            var symbols = new List<Security>
                {
                    new Security {Symbol = "XCS.TO"},
                    new Security {Symbol = "XIU.TO"},
                    new Security {Symbol = "GOOG"},
                };
            using (var tokenManager = new QuestradeApiTokenManager(new Configuration()))
            {
                var api = new QuestradeService(tokenManager, new InMemorySecurityRepository());

                var quotes = api.GetQuotes(symbols);
            }
	    }
	}
}