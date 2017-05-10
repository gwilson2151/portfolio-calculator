using System.Diagnostics;
using System.Globalization;
using Contracts.Interfaces;

namespace Contracts
{
	[DebuggerDisplay("{Exchange}:{Symbol}")]
	public class Security : IDomainEntity
	{
		private string _symbol;
		private string _exchange;

		public long Id { get; set; }
		public string Symbol { get { return _symbol; } set { _symbol = value.ToUpper(CultureInfo.InvariantCulture); } }
		public string Exchange { get {return _exchange;} set { _exchange = value.ToUpper(CultureInfo.InvariantCulture); } }
	}
}