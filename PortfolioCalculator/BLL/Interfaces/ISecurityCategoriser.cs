using System.Collections.Generic;
using Contracts;

namespace BLL.Interfaces
{
	public interface ISecurityCategoriser
	{
		IList<CategoryWeight> GetWeights(Category category, Security security);
	}
}