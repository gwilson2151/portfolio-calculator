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

		private const string RegionName = "Region";
		private const string AssetClassName = "Asset Class";
		private const string CurrencyName = "Currency";

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

		public void GetCategoriesAndWeights(out IEnumerable<Category> categories, out IEnumerable<CategoryWeight> categoryWeights)
		{
			var region = new Category { Name = RegionName };
			var assetClass = new Category { Name = AssetClassName };
			var currency = new Category { Name = CurrencyName };

			var weights = new List<CategoryWeight>();

			while (_csvReader.ReadNextRecord())
			{
				Security security;
				string currencyText, regionText, assetClassText;

				if (_csvReader.HasHeaders)
				{
					security = new Security { Symbol = _csvReader["Symbol"] };
					currencyText = _csvReader["Currency"];
					regionText = _csvReader["Region"];
					assetClassText = _csvReader["AssetClass"];
				}
				else
				{
					security = new Security { Symbol = _csvReader[0] };
					currencyText = _csvReader[1];
					regionText = _csvReader[2];
					assetClassText = _csvReader[3];
				}

				if (!region.Values.Any(v => v.Name.Equals(regionText, StringComparison.InvariantCultureIgnoreCase)))
					region.Values.Add(new CategoryValue { Category = region, Name = regionText });
				if (!assetClass.Values.Any(v => v.Name.Equals(assetClassText, StringComparison.InvariantCultureIgnoreCase)))
					assetClass.Values.Add(new CategoryValue { Category = assetClass, Name = assetClassText });
				if (!currency.Values.Any(v => v.Name.Equals(currencyText, StringComparison.InvariantCultureIgnoreCase)))
					currency.Values.Add(new CategoryValue { Category = currency, Name = currencyText });

				var regionValue = region.Values.Single(v => v.Name.Equals(regionText, StringComparison.InvariantCultureIgnoreCase));
				var assetClassValue = assetClass.Values.Single(v => v.Name.Equals(assetClassText, StringComparison.InvariantCultureIgnoreCase));
				var currencyValue = currency.Values.Single(v => v.Name.Equals(currencyText, StringComparison.InvariantCultureIgnoreCase));

				weights.Add(new CategoryWeight
				{
					Security = security,
					Value = regionValue,
					Weight = 100M
				});
				weights.Add(new CategoryWeight
				{
					Security = security,
					Value = assetClassValue,
					Weight = 100M
				});
				weights.Add(new CategoryWeight
				{
					Security = security,
					Value = currencyValue,
					Weight = 100M
				});
			}

			categories = new List<Category> { region, assetClass, currency };
			categoryWeights = weights;
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
