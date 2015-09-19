using System.Collections.Generic;
using Contracts;

namespace DataGatherer.Interfaces
{
	public interface ITransactionReader
	{
		IEnumerable<Transaction> GetTransactions();
	}
}