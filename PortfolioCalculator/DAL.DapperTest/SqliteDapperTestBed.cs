using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Contracts;
using Dapper;
using NUnit.Framework;

namespace DAL.DapperTest
{
	[TestFixture, Explicit]
	public class SqliteDapperTestBed
	{
		private CategoryRepository _categoryRepository;
		private string _dbFilePath;

		[SetUp]
		public void SetUp()
		{
			var dbFileName = string.Format("{0}.sqlite", Guid.NewGuid());
			_dbFilePath = Path.Combine(Environment.CurrentDirectory, dbFileName);
			_categoryRepository = new CategoryRepository(_dbFilePath);
			InitDb();
		}

		[TearDown]
		public void TearDown()
		{
			if (File.Exists(_dbFilePath))
				File.Delete(_dbFilePath);
		}

		[Test]
		public void Test() {

			var category = _categoryRepository.GetCategory("Security");
			Assert.That(category.Name, Is.EqualTo("Security"));
			Assert.That(category.Id, Is.Not.EqualTo(default(long)));

			var values = new List<CategoryValue>
			{
				new CategoryValue { Name = "Stock"},
				new CategoryValue { Name = "Bond"}
			};
			_categoryRepository.AddValues(category, values);
			Assert.That(category.Values.Count, Is.EqualTo(2));
			foreach (var value in category.Values)
			{
				Assert.That(category.Id, Is.Not.EqualTo(default(long)));
				Assert.That(value.Name, Is.Not.Empty);
			}

			var retrievedValues = _categoryRepository.GetValues(category).ToList();
			foreach (var value in retrievedValues)
			{
				Assert.That(category.Id, Is.Not.EqualTo(default(long)));
				Assert.That(value.Name, Is.Not.Empty);
			}

			var security = new Security {Exchange = "TSX", Symbol = "XSP"};
			security = AddSecurity(security);
			Assert.That(security.Symbol, Is.EqualTo("XSP"));
			Assert.That(security.Exchange, Is.EqualTo("TSX"));
			Assert.That(security.Id, Is.Not.EqualTo(0L));
		}

		public void InitDb()
		{
			using (var conn = new SQLiteConnection("DataSource=" + _dbFilePath))
			{
				conn.Open();
				CreateSchema(conn);
			}
		}

		private static void CreateSchema(IDbConnection conn)
		{
			conn.Execute(@"CREATE TABLE Category
(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	Name NVARCHAR(100) NOT NULL
)");
			conn.Execute(@"CREATE TABLE CategoryValue
(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	Name NVARCHAR(100) NOT NULL,
	CategoryId INTEGER NOT NULL,
	FOREIGN KEY(CategoryId) REFERENCES Category(Id)
)");
			conn.Execute(@"CREATE TABLE Security
(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	Symbol NVARCHAR(24) NOT NULL,
	Exchange NVARCHAR(24) NOT NULL
)");
			conn.Execute(@"CREATE TABLE CategoryWeight
(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	CategoryValueId INTEGER NOT NULL,
	Weight NUMERIC NOT NULL,
	SecurityId INTEGER NOT NULL,
	FOREIGN KEY(CategoryValueId) REFERENCES CategoryValue(Id),
	FOREIGN KEY(SecurityId) REFERENCES Category(Security)
)");
		}

		private Security AddSecurity(Security security)
		{
			using (var conn = new SQLiteConnection("DataSource=" + _dbFilePath))
			{
				var result = conn.Query<Security>(@"SELECT Id, Symbol, Exchange FROM Security WHERE Symbol = @Symbol AND Exchange = @Exchange", security).FirstOrDefault();
				if (result != default(Security))
					return result;

				security.Id = conn.Query<long>(@"INSERT INTO Security (Symbol, Exchange) VALUES (@Symbol, @Exchange); SELECT last_insert_rowid();", security).Single();
				return security;
			}
		}
	}
}
