using System.Runtime.Serialization;

namespace PortfolioSmarts.Questrade.Model
{
	[DataContract]
	public class TokenResponse
	{
		[DataMember(Name="refresh_token")]
		public string RefreshToken { get; set; }
		[DataMember(Name="access_token")]
		public string AccessToken { get; set; }
		[DataMember(Name="token_type")]
		public string TokenType { get; set; }
		[DataMember(Name="api_server")]
		public string ApiServer { get; set; }
		[DataMember(Name="expires_in")]
		public int ExpiresIn { get; set; }
	}
}
