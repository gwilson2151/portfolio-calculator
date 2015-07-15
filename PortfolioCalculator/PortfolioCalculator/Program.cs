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
			var dataDir = Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["DataDirectoryLocation"]);

			if (!Directory.Exists(dataDir)) {
				Console.Error.WriteLine("Data directory at {0} does not exist.", dataDir);
				return 1;
			}

			var portfolioFile = Path.Combine(dataDir, "portfolio.js");
			if (!File.Exists(portfolioFile)) {
				Console.Error.WriteLine("Portfolio file at {0} does not exist.", portfolioFile);
				return 2;
			}

			var portfolioFileContents = File.ReadAllText(portfolioFile, Encoding.UTF8);
			var portfolio = JsonConvert.DeserializeObject<Portfolio>(portfolioFileContents);

			var quoter = new YahooStockService(new QuoteServiceFactory());
			var reporter = new StringValueReporter(quoter);
			var report = reporter.GetReport(portfolio);

			Console.Write(report);

			Console.ReadKey(true);
			return 0;
		}
	}
}
