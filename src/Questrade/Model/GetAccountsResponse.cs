using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PortfolioSmarts.Questrade.Model
{
	[DataContract]
	public class GetAccountsResponse
	{
		[DataMember(Name="accounts")]
		public IEnumerable<AccountDto> Accounts { get; set; }
		[DataMember(Name="userId")]
		public string UserId { get; set; }
	}
}
