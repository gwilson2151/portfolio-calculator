using System.Collections.Generic;
using Contracts;

namespace DataGatherer.Interfaces
{
	public interface IDataImporterFactory
	{
		ITransactionReader GetFundbotBuysFileTransactions(string filePath);
		//ICategoryReader GetFundbotWeightingsFileWeights(string filePath);
	}
}