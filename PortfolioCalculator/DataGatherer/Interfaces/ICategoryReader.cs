using System.Collections.Generic;

using Contracts;

namespace DataGatherer.Interfaces
{
	public interface ICategoryReader
	{
		IEnumerable<Category> GetCategories();
	}
}