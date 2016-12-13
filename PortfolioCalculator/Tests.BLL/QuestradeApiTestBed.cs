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
				var api = new QuestradeService(tokenManager, new InMemorySecurityRepository(), new InMemoryCategoryRepository());
				portfolio.Accounts = api.GetAccounts();

				foreach (var account in portfolio.Accounts)
				{
					account.Positions = api.GetPositions(account);
					account.Transactions = api.GetTransactions(account, new DateTime(2008, 1, 1), DateTime.Now);
				}
			}

			var quoter = new YahooStockService(new YahooServiceFactory());
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
                var api = new QuestradeService(tokenManager, new InMemorySecurityRepository(), new InMemoryCategoryRepository());

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
                var api = new QuestradeService(tokenManager, new InMemorySecurityRepository(), new InMemoryCategoryRepository());

                var quotes = api.GetQuotes(symbols);
            }
	    }

	    [Test]
	    public void WeightTest()
	    {
            var categoryRepository = new InMemoryCategoryRepository();
	        var category = categoryRepository.GetCategory("Security");
	        var category2 = categoryRepository.GetCategory("Currency");
            var securityRepository = new InMemorySecurityRepository();
	        var security = securityRepository.GetBySymbol("XSB.TO");
	        var security2 = securityRepository.GetBySymbol("MSFT");

            using (var tokenManager = new QuestradeApiTokenManager(new Configuration()))
            {
                var api = new QuestradeService(tokenManager, securityRepository, categoryRepository);

                var weights = api.GetWeights(category, security);
                weights = api.GetWeights(category, security);
                weights = api.GetWeights(category, security2);
                weights = api.GetWeights(category2, security);
                weights = api.GetWeights(category2, security2);
            }
	    }
	}
}