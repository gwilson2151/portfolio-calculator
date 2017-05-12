using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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

		public Category GetCategory(string name) {
			using (var conn = SimpleDbConnection())
			{
				conn.Open();
				var result = conn.Query<Category>(@"SELECT Id, Name FROM Category WHERE Name = @name", new {name}).FirstOrDefault();
				if (result != default(Category))
					return result;

				result = new Category {Name = name};
				result.Id = conn.Query<long>(@"INSERT INTO Category (Name) VALUES (@Name); SELECT last_insert_rowid();", result).Single();
				return result;
			}
		}

		public void AddValues(Category category, IEnumerable<CategoryValue> values)
		{
			using (var conn = SimpleDbConnection())
			{
				conn.Open();
				var existingValues = conn.Query<CategoryValue>(@"SELECT Id, Name, CategoryId FROM CategoryValue WHERE CategoryId = @categoryId",
						new {categoryId = category.Id});

				var existingValueNames = new HashSet<string>(existingValues.Select(v => v.Name));
				foreach (var newValue in values.Where(v => !existingValueNames.Contains(v.Name)))
				{
					newValue.Id = conn.Query<long>(@"INSERT INTO CategoryValue (Name, CategoryId) VALUES (@name, @categoryId); SELECT last_insert_rowid()", new {name = newValue.Name, categoryId = category.Id}).Single();

					newValue.Category = category;
					category.Values.Add(newValue);
				}
			}
		}

		public IEnumerable<CategoryValue> GetValues(Category category)
		{
			using (var conn = SimpleDbConnection())
			{
				conn.Open();
				var values = conn.Query<CategoryValue>(@"SELECT Id, Name, CategoryId FROM CategoryValue WHERE CategoryId = @categoryId",
						new { categoryId = category.Id }).ToList();
				foreach (var value in values)
				{
					value.Category = category;
				}
				return values;
			}
		}

		public void AddWeights(Category category, Security security, IEnumerable<CategoryWeight> weights)
		{
			throw new System.NotImplementedException();
		}

		public IEnumerable<CategoryWeight> GetWeights(Category category, Security security)
		{
			throw new System.NotImplementedException();
		}
	}
}