using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

using BLL;
using BLL.Factories;

using Contracts;
using DAL.SQLite;
using Newtonsoft.Json;

namespace PortfolioCalculator
{
	class Program
	{
		static int Main(string[] args)
		{
			//SqliteTest();
			//EF6Test();

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

		public static void EF6Test()
		{
			PortfolioContext db = new PortfolioContext();
			var first = db.Portfolios.First();
		}

		private static void SqliteTest()
		{
			var conn = new SQLiteConnection("Data Source=portfolio.sqlite;Version=3;");
			conn.Open();
			//InitializeSqliteSchema(conn);
			SQLiteCommand comm = new SQLiteCommand("select * from Portfolios;",conn);
			SQLiteDataReader reader = comm.ExecuteReader();
			while (reader.Read())
				Console.WriteLine("Name: " + reader["name"]);
			conn.Close();
		}

		private static void InitializeSqliteSchema(SQLiteConnection conn) {
			string sql = "CREATE TABLE Portfolios (name VARCHAR(128))";
			SQLiteCommand command = new SQLiteCommand(sql, conn);
			command.ExecuteNonQuery();

			sql = "insert into Portfolios (name) values ('Mandingo')";
			command = new SQLiteCommand(sql, conn);
			command.ExecuteNonQuery();

			sql = "insert into Portfolios (name) values ('Brash')";
			command = new SQLiteCommand(sql,conn);
			command.ExecuteNonQuery();
		}
	}
}
