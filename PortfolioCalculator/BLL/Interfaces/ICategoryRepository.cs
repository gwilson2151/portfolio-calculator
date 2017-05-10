using System.Collections.Generic;
using Contracts;

namespace BLL.Interfaces
{
	public interface ICategoryRepository
	{
		Category GetCategory(string name);
		IEnumerable<CategoryValue> GetValues(Category category);
		void AddValues(Category category, IEnumerable<CategoryValue> values);
		IEnumerable<CategoryWeight> GetWeights(Category category, Security security);
		void AddWeights(Category category, Security security, IEnumerable<CategoryWeight> weights);
	}
}