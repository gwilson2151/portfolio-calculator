using System.IO;
using DataGatherer.Interfaces;
using LumenWorks.Framework.IO.Csv;

namespace DataGatherer.Factories
{
	public class DataImporterFactory : IDataImporterFactory
	{
		public ITransactionReader GetFundbotBuysFileTransactions(string filePath)
		{
			CsvReader csv = new CsvReader(new StreamReader(filePath), false);
			return new FundBotImporter(csv);
		}
	}
}