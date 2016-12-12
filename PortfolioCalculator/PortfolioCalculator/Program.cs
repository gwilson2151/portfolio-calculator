using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using BLL;
using BLL.Factories;

using Contracts;
using DataGatherer.Factories;
using DAL.SQLite;
using Newtonsoft.Json;

namespace PortfolioCalculator
{
	class Program
	{
		private const string QuestradePortfolioName = "Questrade_4f4cbfa2-5e74-4039-9dfb-834497f64a73";
		private const string FundbotPortfolioName = "Fundbot_b6cc9e27-77fb-4a75-a92f-992347ef3f08";

		private static readonly Configuration Configuration = new Configuration();

		private enum ErrorCode
		{
			NoError,
			DirectoryMissing,
			FileMissing
		};

		static int Main(string[] args)
		{
			//EF6Test();

			if (args[0].ToLower(CultureInfo.InvariantCulture).Equals("help"))
			{
				Console.Write(@"questrade-value-report - get positions from Questrade API and print current value report. Make sure qapikey is populated with a valid refresh token from Questrade API page.
	1. https://login.questrade.com//APIAccess/UserApps.aspx
	2. Register a personal app if you haven't done so already.
	3. Click 'generate new token' link and copy the token into the 'qapikey' file in the data directory.
fundbot-value-report - import buys.csv from fundbot and print a current value report
fundbot-weight-report - import buys.csv from fundbot and print a report of how the securities are weighted according to categories.csv");
				return Exit(0);
			}

			if (args[0].ToLower(CultureInfo.InvariantCulture).Equals("questrade-value-report"))
			{
				return Exit(QuickQuestradeValueReportOperation(QuestradePortfolioName));
			}

            if (args[0].ToLower(CultureInfo.InvariantCulture).Equals("questrade-weight-report"))
            {
                return Exit(QuickQuestradeWeightReportOperation(QuestradePortfolioName));
            }

			if (args[0].ToLower(CultureInfo.InvariantCulture).Equals("questrade-month-report"))
			{
				return Exit(QuestradeMonthReportOperation(QuestradePortfolioName));
			}

			if (args[0].ToLower(CultureInfo.InvariantCulture).Equals("fundbot-value-report"))
			{
				return Exit(QuickFundbotValueReportOperation(FundbotPortfolioName));
			}

			if (args[0].ToLower(CultureInfo.InvariantCulture).Equals("fundbot-weight-report"))
			{
				return Exit(QuickFundbotWeightReportOperation(FundbotPortfolioName));
			}

			return Exit(DefaultOperation());
		}

		private static ErrorCode QuickQuestradeValueReportOperation(string portfolioName)
		{
			var portfolio = new Portfolio
			{
				Name = portfolioName
			};
			using (var tokenManager = new QuestradeApiTokenManager(Configuration))
			{
				var api = new QuestradeService(tokenManager, new InMemorySecurityRepository(), new InMemoryCategoryRepository());
				portfolio.Accounts = api.GetAccounts();

				foreach (var account in portfolio.Accounts)
				{
					account.Positions = api.GetPositions(account);
					account.Transactions = api.GetTransactions(account, new DateTime(2008, 1, 1), DateTime.Now);
                }

                var reporter = new StringValueReporter(api);
                var report = reporter.GetReport(portfolio);

                Console.Write(report);
			}

			return ErrorCode.NoError;
		}

	    private static ErrorCode QuickQuestradeWeightReportOperation(string portfolioName)
	    {
            var portfolio = new Portfolio
            {
                Name = portfolioName
            };
            var categoryRepository = new InMemoryCategoryRepository();
	        var securityCategory = categoryRepository.GetCategory("Security");
	        var currencyCategory = categoryRepository.GetCategory("Currency");
	        using (var tokenManager = new QuestradeApiTokenManager(Configuration))
	        {
                var api = new QuestradeService(tokenManager, new InMemorySecurityRepository(), categoryRepository);
                portfolio.Accounts = api.GetAccounts();
	            var weights = new List<CategoryWeight>();

	            foreach (var account in portfolio.Accounts)
	            {
                    account.Positions = api.GetPositions(account);
	                foreach (var position in account.Positions)
	                {
                        weights.AddRange(api.GetWeights(securityCategory, position.Security));
                        weights.AddRange(api.GetWeights(currencyCategory, position.Security));
	                }
	            }

                var reporter = new StringWeightReporter(api);
	            var report = reporter.GetReport(portfolio, new[] {securityCategory, currencyCategory}, weights.Distinct());

                Console.Write(report);
	        }

	        return ErrorCode.NoError;
	    }

		private static ErrorCode QuestradeMonthReportOperation(string portfolioName)
		{
			var portfolio = new Portfolio
			{
				Name = portfolioName
			};
			using (var tokenManager = new QuestradeApiTokenManager(Configuration))
			{
				var api = new QuestradeService(tokenManager, new InMemorySecurityRepository(), new InMemoryCategoryRepository());
				portfolio.Accounts = api.GetAccounts();

				foreach (var account in portfolio.Accounts)
				{
					account.Positions = api.GetPositions(account);
				}

				var service = new YahooStockService(new YahooServiceFactory());
				var reporter = new StringHistoricalReporter(service);
				var startDate = GetStartOfMarketWeek(DateTime.Now);
				var report = reporter.GetReport(portfolio, startDate.AddMonths(-1), startDate, Period.Weekly);

				Console.Write(report);
			}

			return ErrorCode.NoError;
		}

		private static DateTime GetStartOfMarketWeek(DateTime now)
		{
			if (now.DayOfWeek == DayOfWeek.Sunday)
				return now.AddDays(-6);
			return now.AddDays(-((int) now.DayOfWeek - 1));
		}

		private static ErrorCode QuickFundbotValueReportOperation(string portfolioName)
		{
			var dataDir = Path.GetFullPath(Environment.ExpandEnvironmentVariables(Configuration.DataDirectoryPath));

			if (!Directory.Exists(dataDir))
			{
				Console.Error.WriteLine("Data directory at {0} does not exist.", dataDir);
				return ErrorCode.DirectoryMissing;
			}

			var fundbotBuysFile = Path.Combine(dataDir, "buys.csv");
			if (!File.Exists(fundbotBuysFile))
			{
				Console.Error.WriteLine("Fundbot file at {0} does not exist.", fundbotBuysFile);
				return ErrorCode.FileMissing;
			}

			var factory = new DataImporterFactory();
			var fundBotImporter = factory.GetFundbotTransactions(fundbotBuysFile);
			var transactions = fundBotImporter.GetTransactions();

			var account = new Account
			{
				Name = portfolioName
			};
			var portfolio = new Portfolio
			{
				Name = portfolioName,
				Accounts = new List<Account> { account }
			};
			account.Portfolio = portfolio;

			foreach (var transaction in transactions)
			{
				transaction.Account = account;
			}

			var portfolioService = new PortfolioService(portfolio);
			portfolioService.UpdateWith(transactions);

			var quoter = new YahooStockService(new YahooServiceFactory());
			var reporter = new StringValueReporter(quoter);
			var report = reporter.GetReport(portfolio);

			Console.Write(report);

			return ErrorCode.NoError;
		}

		private static ErrorCode QuickFundbotWeightReportOperation(string portfolioName)
		{
			var dataDir = Path.GetFullPath(Environment.ExpandEnvironmentVariables(Configuration.DataDirectoryPath));

			if (!Directory.Exists(dataDir))
			{
				Console.Error.WriteLine("Data directory at {0} does not exist.", dataDir);
				return ErrorCode.DirectoryMissing;
			}

			var fundbotBuysFile = Path.Combine(dataDir, "buys.csv");
			if (!File.Exists(fundbotBuysFile))
			{
				Console.Error.WriteLine("Fundbot file at {0} does not exist.", fundbotBuysFile);
				return ErrorCode.FileMissing;
			}

			var fundbotCategoriesFile = Path.Combine(dataDir, "categories.csv");
			if (!File.Exists(fundbotCategoriesFile))
			{
				Console.Error.WriteLine("Fundbot file at {0} does not exist.", fundbotCategoriesFile);
				return ErrorCode.FileMissing;
			}

			var factory = new DataImporterFactory();
			var transactionReader = factory.GetFundbotTransactions(fundbotBuysFile);
			var transactions = transactionReader.GetTransactions();

			var account = new Account
			{
				Name = portfolioName
			};
			var portfolio = new Portfolio
			{
				Name = portfolioName,
				Accounts = new List<Account> { account }
			};
			account.Portfolio = portfolio;

			foreach (var transaction in transactions)
			{
				transaction.Account = account;
			}

			var portfolioService = new PortfolioService(portfolio);
			portfolioService.UpdateWith(transactions);

			var categoryReader = factory.GetFundbotCategories(fundbotCategoriesFile);
			IEnumerable<Category> categories;
			IEnumerable<CategoryWeight> weights;
			categoryReader.GetCategoriesAndWeights(out categories, out weights);

			var quoter = new YahooStockService(new YahooServiceFactory());
			StringWeightReporter reporter = new StringWeightReporter(quoter);
			var report = reporter.GetReport(portfolio, categories, weights);

			Console.Write(report);

			return ErrorCode.NoError;
		}

		private static ErrorCode ImportFundbotOperation(string portfolioName)
		{
			var dataDir = Path.GetFullPath(Environment.ExpandEnvironmentVariables(Configuration.DataDirectoryPath));

			if (!Directory.Exists(dataDir))
			{
				Console.Error.WriteLine("Data directory at {0} does not exist.", dataDir);
				return ErrorCode.DirectoryMissing;
			}

			var fundbotBuysFile = Path.Combine(dataDir, "buys.csv");
			if (!File.Exists(fundbotBuysFile))
			{
				Console.Error.WriteLine("Fundbot file at {0} does not exist.", fundbotBuysFile);
				return ErrorCode.FileMissing;
			}

			var factory = new DataImporterFactory();
			var fundBotImporter = factory.GetFundbotTransactions(fundbotBuysFile);
			var transactions = fundBotImporter.GetTransactions();

			// which portfolio? account?
			var account = new Account
			{
				Name = portfolioName
			};
			var portfolio = new Portfolio
			{
				Name = portfolioName,
				Accounts = new List<Account> { account }
			};
			account.Portfolio = portfolio;

			foreach (var transaction in transactions)
			{
				transaction.Account = account;
			}

			var portfolioService = new PortfolioService(portfolio);
			portfolioService.UpdateWith(transactions);

			var quoter = new YahooStockService(new YahooServiceFactory());
			var reporter = new StringValueReporter(quoter);
			var report = reporter.GetReport(portfolio);

			Console.Write(report);

			//foreach (var transaction in transactions)
			//{
			//	// need to see if security already exists
			//	// need to see if transaction already exists
			//	// key = symbol & date & shares
			//}

			return ErrorCode.NoError;
		}

		private static ErrorCode DefaultOperation()
		{
			var dataDir = Path.GetFullPath(Environment.ExpandEnvironmentVariables(Configuration.DataDirectoryPath));

			if (!Directory.Exists(dataDir))
			{
				Console.Error.WriteLine("Data directory at {0} does not exist.", dataDir);
				return ErrorCode.DirectoryMissing;
			}

			var portfolioFile = Path.Combine(dataDir, "portfolio.js");
			if (!File.Exists(portfolioFile))
			{
				Console.Error.WriteLine("Portfolio file at {0} does not exist.", portfolioFile);
				return ErrorCode.FileMissing;
			}

			var portfolioFileContents = File.ReadAllText(portfolioFile, Encoding.UTF8);
			var portfolio = JsonConvert.DeserializeObject<Portfolio>(portfolioFileContents);

			var categoriesFile = Path.Combine(dataDir, "categories.js");
			if (!File.Exists(categoriesFile))
			{
				Console.Error.WriteLine("Categories file at {0} does not exist.", categoriesFile);
				return ErrorCode.FileMissing;
			}

			var categoryFileContents = File.ReadAllText(categoriesFile, Encoding.UTF8);
			var categories = JsonConvert.DeserializeObject<IEnumerable<Category>>(categoryFileContents);

			var quoter = new YahooStockService(new YahooServiceFactory());
			var reporter = new StringValueReporter(quoter);
			var report = reporter.GetReport(portfolio);

			Console.Write(report);

			return ErrorCode.NoError;
		}

		private static int Exit(ErrorCode code)
		{
			Console.WriteLine(@"Press any key to exit");
			Console.ReadKey(true);
			return (int)code;
		}

		private static void EF6Test()
		{
			var db = new PortfolioContext();
			var first = db.Portfolios.Include("Accounts").First();
			var accounts = first.Accounts;

			//var next = from p in db.Portfolios.Include("Accounts") select p;
		}
	}
}
