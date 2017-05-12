using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using BLL.Interfaces;
using Contracts;
using Dapper;

namespace DAL.DapperTest
{
	public class CategoryRepository : ICategoryRepository
	{
		private readonly string _dbFilePath;

		public CategoryRepository(string dbFilePath)
		{
			_dbFilePath = dbFilePath;
		}

		private SQLiteConnection GetDbConnection()
		{
			return new SQLiteConnection("DataSource=" + _dbFilePath);
		}

		public Category GetCategory(string name) {
			using (var conn = GetDbConnection())
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
			using (var conn = GetDbConnection())
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
			using (var conn = GetDbConnection())
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