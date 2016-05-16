using System;
using System.Collections.Generic;
using System.Linq;
using BLL.Exceptions;
using BLL.Interfaces;
using Contracts;

namespace BLL
{
    public class InMemoryCategoryRepository : ICategoryRepository
    {
        private readonly Dictionary<string, Category> _categoryCache = new Dictionary<string, Category>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<Security, List<CategoryWeight>> _securityCache = new Dictionary<Security, List<CategoryWeight>>();

        public Category GetCategory(string name)
        {
            if (!_categoryCache.ContainsKey(name))
                _categoryCache[name] = new Category
                {
                    Name = name
                };

            return _categoryCache[name];
        }

        public IEnumerable<CategoryValue> GetValues(Category category)
        {
            if (!_categoryCache.ContainsKey(category.Name))
                throw new CategoryNotFoundException(string.Format("Category '{0}' not found.", category.Name));

            return _categoryCache[category.Name].Values;
        }

        public void AddValues(Category category, IEnumerable<CategoryValue> values)
        {
            if (!_categoryCache.ContainsKey(category.Name))
                throw new CategoryNotFoundException(string.Format("Category '{0}' not found.", category.Name));

            var tempValues = new List<CategoryValue>(category.Values);
            tempValues.AddRange(values.Except(category.Values));
            category.Values = tempValues;
        }

        public IEnumerable<CategoryWeight> GetWeights(Category category, Security security)
        {
            if (!_categoryCache.ContainsKey(category.Name))
                throw new CategoryNotFoundException(string.Format("Category '{0}' not found.", category.Name));

            if (!_securityCache.ContainsKey(security))
                throw new SecurityNotFoundException(string.Format("Security '{0}' not found.", security.Symbol));

            return _securityCache[security].Where(cw => category.Values.Contains(cw.Value));
        }

        public void AddWeights(Category category, Security security, IEnumerable<CategoryWeight> weights)
        {
            if (!_categoryCache.ContainsKey(category.Name))
                throw new CategoryNotFoundException(string.Format("Category '{0}' not found.", category.Name));

            if (!_securityCache.ContainsKey(security))
            {
                _securityCache.Add(security, new List<CategoryWeight>());
            }

            _securityCache[security].AddRange(weights.Except(_securityCache[security]));
        }
    }
}