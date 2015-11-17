using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Contracts;
using DataGatherer.Interfaces;
using LumenWorks.Framework.IO.Csv;

namespace DataGatherer
{
	public class FundBotImporter : ITransactionReader, ICategoryReader, IDisposable
	{
		private CsvReader _csvReader;
		private bool _disposed;

		private const string RegionKey = "region";
		private const string AssetClassKey = "assetClass";
		private const string CurrencyKey = "currency";

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

		public IEnumerable<Category> GetCategories()
		{
			var region = new Category { Name = RegionKey };
			var assetClass = new Category { Name = AssetClassKey };
			var currency = new Category { Name = CurrencyKey };

			while (_csvReader.ReadNextRecord())
			{
				if (_csvReader.HasHeaders)
				{
					
				}
				else
				{
					var security = new Security {Symbol = _csvReader[0]};
					var currencyValue = _csvReader[1];
					var regionValue = _csvReader[2];
					var assetClassValue = _csvReader[3];

					if (!region.Values.Any(v => v.Name.Equals(regionValue, StringComparison.InvariantCultureIgnoreCase)))
						region.Values.Add(new CategoryValue { Category = region, Name = regionValue});
					if (!assetClass.Values.Any(v => v.Name.Equals(assetClassValue, StringComparison.InvariantCultureIgnoreCase)))
						assetClass.Values.Add(new CategoryValue { Category = assetClass, Name = assetClassValue });
					if (!currency.Values.Any(v => v.Name.Equals(currencyValue, StringComparison.InvariantCultureIgnoreCase)))
						currency.Values.Add(new CategoryValue { Category = currency, Name = currencyValue });
				}
			}

			return new List<Category> {region, assetClass, currency};
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
