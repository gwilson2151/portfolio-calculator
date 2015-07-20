using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BLL;
using BLL.Factories;
using BLL.Interfaces;

using Contracts;
using Newtonsoft.Json;

namespace PortfolioCalculator
{
	class Program
	{
		static int Main(string[] args)
		{
			var dataDir = Path.GetFullPath(Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["DataDirectoryLocation"]));

			if (!Directory.Exists(dataDir)) {
				Console.Error.WriteLine("Data directory at {0} does not exist.", dataDir);
				return Exit(1);
			}

			var portfolioFile = Path.Combine(dataDir, "portfolio.js");
			if (!File.Exists(portfolioFile)) {
				Console.Error.WriteLine("Portfolio file at {0} does not exist.", portfolioFile);
				return Exit(2);
			}

			var portfolioFileContents = File.ReadAllText(portfolioFile, Encoding.UTF8);
			var portfolio = JsonConvert.DeserializeObject<Portfolio>(portfolioFileContents);

			var categoriesFile = Path.Combine(dataDir, "categories.js");
			if (!File.Exists(categoriesFile))
			{
				Console.Error.WriteLine("Categories file at {0} does not exist.", categoriesFile);
				return Exit(3);
			}

			var categoryFileContents = File.ReadAllText(categoriesFile, Encoding.UTF8);
			var categories = JsonConvert.DeserializeObject<IEnumerable<Category>>(categoryFileContents);

			var quoter = new YahooStockService(new QuoteServiceFactory());
			var reporter = new StringValueReporter(quoter);
			var report = reporter.GetReport(portfolio);

			Console.Write(report);

			return Exit(0);
		}

		private static int Exit(int code)
		{
			Console.ReadKey(true);
			return code;
		}
	}
}
