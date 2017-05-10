using Dapper;
using NUnit.Framework;

namespace DAL.DapperTest
{
	[TestFixture, Explicit]
	public class SqliteDapperTestBed
	{
		private CategoryRepository _categoryRepository;

		public SqliteDapperTestBed() {
			_categoryRepository = new CategoryRepository();
		}

		[Test]
		public void Test() {
			if (!_categoryRepository.DbExists())
				_categoryRepository.InitDb();


		}
	}
}
