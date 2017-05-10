using System.Collections.Generic;
using System.IO;
using BLL.Interfaces;
using Contracts;
using Dapper;

namespace DAL.DapperTest
{
	public class CategoryRepository : SqliteBaseRepository, ICategoryRepository
	{
		public bool DbExists() {
			return File.Exists(DbFile);
		}

		public void InitDb() {
			using (var conn = SimpleDbConnection())
			{
				conn.Open();
				conn.Execute(@"create table Category
(
	Id integer primary key autoincrement,
	Name nvarchar(100) not null
)");
				conn.Execute(@"create table CategoryValue
(
	Id integer primary key autoincrement,
	Name nvarchar(100) not null,
	CategoryId integer not null
)");
				conn.Execute(@"create table Security
(
	Id integer primary key autoincrement,
	Symbol nvarchar(24) not null,
	Exchange nvarchar(24) not null
)");
				conn.Execute(@"create table CategoryWeight
(
	Id integer primary key autoincrement,
	CategoryValueId integer not null,
	Weight numeric not null,
	SecurityId integer not null
)");
			}
		}

		public Category GetCategory(string name) {
			throw new System.NotImplementedException();
		}

		public void AddWeights(Category category, Security security, IEnumerable<CategoryWeight> weights)
		{
			throw new System.NotImplementedException();
		}

		public IEnumerable<CategoryWeight> GetWeights(Category category, Security security)
		{
			throw new System.NotImplementedException();
		}

		public void AddValues(Category category, IEnumerable<CategoryValue> values)
		{
			throw new System.NotImplementedException();
		}

		public IEnumerable<CategoryValue> GetValues(Category category)
		{
			throw new System.NotImplementedException();
		}
	}
}