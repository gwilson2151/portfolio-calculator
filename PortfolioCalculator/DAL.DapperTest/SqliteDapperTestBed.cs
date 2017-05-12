using System.Collections.Generic;
using System.Linq;
using Contracts;
using NUnit.Framework;

namespace DAL.DapperTest
{
	[TestFixture, Explicit]
	public class SqliteDapperTestBed
	{
		private readonly CategoryRepository _categoryRepository;

		public SqliteDapperTestBed() {
			_categoryRepository = new CategoryRepository();
		}

		[Test]
		public void Test() {
			if (!_categoryRepository.DbExists())
				_categoryRepository.InitDb();

			var category = _categoryRepository.GetCategory("Security");
			Assert.That(category.Name, Is.EqualTo("Security"));
			Assert.That(category.Id, Is.Not.EqualTo(default(long)));

			category.Values = _categoryRepository.GetValues(category).ToList();
			foreach (var value in category.Values)
			{
				Assert.That(category.Id, Is.Not.EqualTo(default(long)));
				Assert.That(value.Name, Is.Not.Empty);
			}

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
		}
	}
}
