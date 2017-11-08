using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using NUnit.Framework;

namespace DAL.NPocoTest
{

	[TestFixture, Explicit]
	public class SqliteNPocoTestBed
	{
		private IDbConnection _dbConnection;
		private CategoryRepository _categoryRepository;
		private string _dbFilePath;

		[SetUp]
		public void SetUp()
		{
			var dbFileName = string.Format("{0}.sqlite", Guid.NewGuid());
			_dbFilePath = Path.Combine(Environment.CurrentDirectory, dbFileName);
			var connection = new SQLiteConnection("DataSource=" + _dbFilePath);
			_dbConnection = connection;
			_dbConnection.Open();
			CreateSchema(_dbConnection);
			_categoryRepository = new CategoryRepository(connection);
		}

		[TearDown]
		public void TearDown()
		{
			if (_dbConnection.State != ConnectionState.Closed)
				_dbConnection.Close();
			if (File.Exists(_dbFilePath))
				File.Delete(_dbFilePath);
		}

		[Test]
		public void Test()
		{
			var category = _categoryRepository.GetCategory("Security");
			Assert.That(category.Name, Is.EqualTo("Security"));
			Assert.That(category.Id, Is.Not.EqualTo(default(long)));
		}

		private static void CreateSchema(IDbConnection conn)
		{
			var command = conn.CreateCommand();
			command.CommandText = @"CREATE TABLE Category
(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	Name NVARCHAR(100) NOT NULL
)";
			command.ExecuteNonQuery();

			command = conn.CreateCommand();
			command.CommandText = @"CREATE TABLE CategoryValue
(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	Name NVARCHAR(100) NOT NULL,
	CategoryId INTEGER NOT NULL,
	FOREIGN KEY(CategoryId) REFERENCES Category(Id)
)";
			command.ExecuteNonQuery();

			command = conn.CreateCommand();
			command.CommandText = @"CREATE TABLE Security
(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	Symbol NVARCHAR(24) NOT NULL,
	Exchange NVARCHAR(24) NOT NULL
)";
			command.ExecuteNonQuery();

			command = conn.CreateCommand();
			command.CommandText = @"CREATE TABLE CategoryWeight
(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	CategoryValueId INTEGER NOT NULL,
	Weight NUMERIC NOT NULL,
	SecurityId INTEGER NOT NULL,
	FOREIGN KEY(CategoryValueId) REFERENCES CategoryValue(Id),
	FOREIGN KEY(SecurityId) REFERENCES Category(Security)
)";
			command.ExecuteNonQuery();
		}
	}
}