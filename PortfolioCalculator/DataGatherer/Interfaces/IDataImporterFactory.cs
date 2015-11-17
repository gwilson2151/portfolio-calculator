using System.Collections.Generic;
using Contracts;

namespace DataGatherer.Interfaces
{
	public interface IDataImporterFactory
	{
		ITransactionReader GetFundbotTransactions(string filePath);
		//ICategoryReader GetFundbotWeightingsFileWeights(string filePath);
		ICategoryReader GetFundbotCategories(string filePath);
	}
}