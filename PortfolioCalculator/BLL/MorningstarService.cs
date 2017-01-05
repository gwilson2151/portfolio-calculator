using System;
using System.Collections.Generic;
using System.Linq;
using BLL.Exceptions;
using BLL.Interfaces;
using Contracts;
using MorningstarScraper;

namespace BLL
{
	public class MorningstarService : ISecurityCategoriser
	{
		private const string NotApplicable = "n/a";
		private readonly IScraper _scraper;
		private readonly ISecurityRepository _securityRepository;
		private readonly ICategoryRepository _categoryRepository;

		public MorningstarService(IScraper scraper, ISecurityRepository securityRepository, ICategoryRepository categoryRepository)
		{
			_scraper = scraper;
			_securityRepository = securityRepository;
			_categoryRepository = categoryRepository;
		}

		public IList<CategoryWeight> GetWeights(Category category, Security security)
		{
			var weights = _categoryRepository.GetWeights(category, security).ToList();
			if (weights.Any(w => w.Value.Category == category))
				return weights.Where(w => w.Value.Category == category).ToList();
			
			return GetWeightDataFromMorningstar(category, security);
		}

		private IList<CategoryWeight> GetWeightDataFromMorningstar(Category category, Security security)
		{
			if (category.Name.Equals("assetallocation", StringComparison.InvariantCultureIgnoreCase))
			{
				var assetAllocation = _scraper.GetAssetAllocation(TranslateSymbol(security.Symbol));
				var values = _categoryRepository.GetValues(category).ToList();
				var list = new List<CategoryWeight>();

				if (!assetAllocation.Any())
				{
					var value = values.SingleOrDefault(v => v.Name.Equals(NotApplicable));
					if (value == null)
					{
						value = new CategoryValue { Category = category, Name = NotApplicable };
						_categoryRepository.AddValues(category, new[] { value });
					}
					list.Add(new CategoryWeight { Security = security, Weight = 100M, Value = value });
				}
				else
				{
					foreach (var kvpair in assetAllocation)
					{
						var categoryValueName = kvpair.Key;
						var value = values.SingleOrDefault(v => v.Name.Equals(categoryValueName));
						if (value == null)
						{
							value = new CategoryValue { Category = category, Name = categoryValueName };
							_categoryRepository.AddValues(category, new[] { value });
						}
						list.Add(new CategoryWeight { Security = security, Weight = kvpair.Value, Value = value });
					}
				}

				_categoryRepository.AddWeights(category, security, list);
				return list;
			}

			throw new CategoryNotSupportedException(string.Format("Category not supported: {0}", category.Name));
		}

		private static string TranslateSymbol(string symbol)
		{
			return symbol.Split('.')[0];
		}
	}
}
