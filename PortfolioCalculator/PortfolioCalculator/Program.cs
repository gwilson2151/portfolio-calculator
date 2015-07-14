using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BLL;
using BLL.Factories;
using BLL.Interfaces;

using Contracts;

namespace PortfolioCalculator
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Blargh!!!");

			var quoter = new YahooStockService(new QuoteServiceFactory());
			//var result = quoter.GetQuotes(new [] {new Security {Symbol = "GOOG"}, new Security { Symbol = "XCS.TO"} });
			var report = new StringValueReporter(quoter);

			Console.ReadKey(true);
		}
	}
}
