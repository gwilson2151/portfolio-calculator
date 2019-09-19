using System.Runtime.Serialization;

namespace PortfolioSmarts.Questrade.Model
{
	[DataContract]
	public class AccountDto
	{
		[DataMember(Name="type")]
		public string Type { get; set; }
		[DataMember(Name="number")]
		public string Number { get; set; }
		[DataMember(Name="status")]
		public string Status { get; set; }
		[DataMember(Name="isPrimary")]
		public bool IsPrimary { get; set; }
		[DataMember(Name="isBilling")]
		public bool IsBilling { get; set; }
		[DataMember(Name="clientAccountType")]
		public string ClientAccountType { get; set; }
	}
}
