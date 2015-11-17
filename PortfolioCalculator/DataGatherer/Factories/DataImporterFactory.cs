using System.IO;
using DataGatherer.Interfaces;
using LumenWorks.Framework.IO.Csv;

namespace DataGatherer.Factories
{
	public class DataImporterFactory : IDataImporterFactory
	{
		public ITransactionReader GetFundbotTransactions(string filePath)
		{
			CsvReader csv = new CsvReader(new StreamReader(filePath), false);
			return new FundBotImporter(csv);
		}

		public ICategoryReader GetFundbotCategories(string filePath)
		{
			CsvReader csv = new CsvReader(new StreamReader(filePath), false);
			return new FundBotImporter(csv);
		}
	}
}