using System.Collections.Generic;

using Contracts;

namespace DataGatherer.Interfaces
{
	public interface ICategoryReader
	{
		void GetCategoriesAndWeights(out IEnumerable<Category> categories, out IEnumerable<CategoryWeight> categoryWeights);
	}
}