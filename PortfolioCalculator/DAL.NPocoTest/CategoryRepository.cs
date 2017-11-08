using System.Collections.Generic;
using System.Data.Common;
using BLL.Interfaces;
using Contracts;
using NPoco;

namespace DAL.NPocoTest
{
    public class CategoryRepository : ICategoryRepository
	{
		private readonly Database _database;

		public CategoryRepository(DbConnection connection)
		{
			_database = new Database(connection);
		}

		public Category GetCategory(string name)
		{
			var category = _database.SingleOrDefault<Category>("where Name = @0", name);
			if (category != default(Category))
				return category;

			category = new Category { Name = name };
			_database.Save(category);
			return category;
		}

		public IEnumerable<CategoryValue> GetValues(Category category)
		{
			throw new System.NotImplementedException();
		}

		public void AddValues(Category category, IEnumerable<CategoryValue> values)
		{
			throw new System.NotImplementedException();
		}

		public IEnumerable<CategoryWeight> GetWeights(Category category, Security security)
		{
			throw new System.NotImplementedException();
		}

		public void AddWeights(Category category, Security security, IEnumerable<CategoryWeight> weights)
		{
			throw new System.NotImplementedException();
		}
	}
}
