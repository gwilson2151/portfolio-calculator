using System.Globalization;
using Contracts.Interfaces;

namespace Contracts
{
	public class Security : IDomainEntity
	{
	    private string _symbol;

		public long Id { get; set; }
		public string Symbol { get { return _symbol; } set { _symbol = value.ToUpper(CultureInfo.InvariantCulture); } }
	}
}