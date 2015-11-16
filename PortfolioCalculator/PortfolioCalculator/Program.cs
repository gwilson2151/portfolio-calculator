using System;
using System.Collections.Generic;
using System.Configuration;
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
		private const string FundbotPortfolioName = "b6cc9e27-77fb-4a75-a92f-992347ef3f08";

		private enum ErrorCodes
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
				Console.Write(@"report-fundbot - import buys.csv from fundbot and print a current value report");
				return Exit(0);
			}

			if (args[0].ToLower(CultureInfo.InvariantCulture).Equals("report-fundbot"))
			{
				return Exit(QuickFundbotReportOperation(FundbotPortfolioName));
			}

			return Exit(DefaultOperation());
		}

		private static ErrorCodes QuickFundbotReportOperation(string portfolioName)
		{
			var dataDir = Path.GetFullPath(Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["DataDirectoryLocation"]));

			if (!Directory.Exists(dataDir))
			{
				Console.Error.WriteLine("Data directory at {0} does not exist.", dataDir);
				return ErrorCodes.DirectoryMissing;
			}

			var fundbotBuysFile = Path.Combine(dataDir, "buys.csv");
			if (!File.Exists(fundbotBuysFile))
			{
				Console.Error.WriteLine("Fundbot file at {0} does not exist.", fundbotBuysFile);
				return ErrorCodes.FileMissing;
			}

			var factory = new DataImporterFactory();
			var fundBotImporter = factory.GetFundbotBuysFileTransactions(fundbotBuysFile);
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

			var quoter = new YahooStockService(new QuoteServiceFactory());
			var reporter = new StringValueReporter(quoter);
			var report = reporter.GetReport(portfolio);

			Console.Write(report);

			return ErrorCodes.NoError;
		}

		private static ErrorCodes ImportFundbotOperation(string portfolioName)
		{
			var dataDir = Path.GetFullPath(Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["DataDirectoryLocation"]));

			if (!Directory.Exists(dataDir))
			{
				Console.Error.WriteLine("Data directory at {0} does not exist.", dataDir);
				return ErrorCodes.DirectoryMissing;
			}

			var fundbotBuysFile = Path.Combine(dataDir, "buys.csv");
			if (!File.Exists(fundbotBuysFile))
			{
				Console.Error.WriteLine("Fundbot file at {0} does not exist.", fundbotBuysFile);
				return ErrorCodes.FileMissing;
			}

			var factory = new DataImporterFactory();
			var fundBotImporter = factory.GetFundbotBuysFileTransactions(fundbotBuysFile);
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

			var quoter = new YahooStockService(new QuoteServiceFactory());
			var reporter = new StringValueReporter(quoter);
			var report = reporter.GetReport(portfolio);

			Console.Write(report);

			//foreach (var transaction in transactions)
			//{
			//	// need to see if security already exists
			//	// need to see if transaction already exists
			//	// key = symbol & date & shares
			//}

			return ErrorCodes.NoError;
		}

		private static ErrorCodes DefaultOperation()
		{
			var dataDir = Path.GetFullPath(Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["DataDirectoryLocation"]));

			if (!Directory.Exists(dataDir))
			{
				Console.Error.WriteLine("Data directory at {0} does not exist.", dataDir);
				return ErrorCodes.DirectoryMissing;
			}

			var portfolioFile = Path.Combine(dataDir, "portfolio.js");
			if (!File.Exists(portfolioFile))
			{
				Console.Error.WriteLine("Portfolio file at {0} does not exist.", portfolioFile);
				return ErrorCodes.FileMissing;
			}

			var portfolioFileContents = File.ReadAllText(portfolioFile, Encoding.UTF8);
			var portfolio = JsonConvert.DeserializeObject<Portfolio>(portfolioFileContents);

			var categoriesFile = Path.Combine(dataDir, "categories.js");
			if (!File.Exists(categoriesFile))
			{
				Console.Error.WriteLine("Categories file at {0} does not exist.", categoriesFile);
				return ErrorCodes.FileMissing;
			}

			var categoryFileContents = File.ReadAllText(categoriesFile, Encoding.UTF8);
			var categories = JsonConvert.DeserializeObject<IEnumerable<Category>>(categoryFileContents);

			var quoter = new YahooStockService(new QuoteServiceFactory());
			var reporter = new StringValueReporter(quoter);
			var report = reporter.GetReport(portfolio);

			Console.Write(report);

			return ErrorCodes.NoError;
		}

		private static int Exit(ErrorCodes code)
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
