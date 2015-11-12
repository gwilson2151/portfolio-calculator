using System;
using System.Collections.Generic;
using System.Globalization;

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
			var transactions = new List<Transaction>();

			while (_csvReader.ReadNextRecord())
			{
				Transaction transaction;
				if (_csvReader.HasHeaders)
				{
					transaction = new Transaction
					{
						Security = new Security { Symbol = _csvReader["Symbol"] },
						Date = DateTime.Parse(_csvReader["Date"], CultureInfo.CreateSpecificCulture("en-CA")),
						Price = Decimal.Parse(_csvReader["Price"]),
						Shares = Decimal.Parse(_csvReader["Shares"]),
						Type = TransactionType.Buy
					};
				}
				else
				{
					transaction = new Transaction
					{
						Security = new Security { Symbol = _csvReader[0] },
						Date = DateTime.Parse(_csvReader[1], CultureInfo.CreateSpecificCulture("en-CA")),
						Price = Decimal.Parse(_csvReader[3]),
						Shares = Decimal.Parse(_csvReader[2]),
						Type = TransactionType.Buy
					};
				}
				transactions.Add(transaction);
			}

			return transactions;
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
