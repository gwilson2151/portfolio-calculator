using System;
using System.Collections.Generic;
using Contracts;
using DataGatherer.Interfaces;
using LumenWorks.Framework.IO.Csv;

namespace DataGatherer
{
	public class FundBotImporter : ITransactionReader, IDisposable
	{
		private CsvReader _csvReader;
		private bool _disposed;

		public FundBotImporter(CsvReader reader)
		{
			_csvReader = reader;
		}

		public IEnumerable<Transaction> GetTransactions()
		{
			throw new NotImplementedException();
		}

		#region IDisposable
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
			{
				if (disposing)
				{
					if (_csvReader != null)
					{
						_csvReader.Dispose();
						_csvReader = null;
					}
				}

				_disposed = true;
			}
		}
		#endregion
	}
}
