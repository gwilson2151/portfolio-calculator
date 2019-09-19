using System;
using PortfolioSmarts.Questrade.Model;

namespace PortfolioSmarts.Questrade
{
	public class SessionState
	{
		public string RefreshToken {get; private set; }
		public string AccessToken { get; private set; }
		public string ApiUrl { get; private set; }
		public string TokenType { get; private set; }
		public DateTime TokenExpires { get; private set; }
		private const int ExpireTimeBufferSeconds = 30;

		public SessionState(TokenResponse tokenDetails, DateTime requestTime)
		{
			RefreshToken = tokenDetails.RefreshToken;
			AccessToken = tokenDetails.AccessToken;
			ApiUrl = tokenDetails.ApiServer?.TrimEnd('/');
			TokenType = tokenDetails.TokenType;
			TokenExpires = requestTime + new TimeSpan(0, 0, tokenDetails.ExpiresIn - ExpireTimeBufferSeconds);
		}

		public bool SessionValid()
		{
			return DateTime.UtcNow < TokenExpires;
		}
	}
}
