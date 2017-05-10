using System;
using System.Data.SQLite;
using System.IO;

namespace DAL.DapperTest
{
	public class SqliteBaseRepository
	{
		public static string DbFile
		{
			get { return Path.Combine(Environment.CurrentDirectory, @"DapperTest.sqlite"); }
		}

		public static SQLiteConnection SimpleDbConnection()
		{
			return new SQLiteConnection("DataSource="+DbFile);
		}
	}
}