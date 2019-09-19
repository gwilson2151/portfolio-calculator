using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

using PortfolioSmarts.Questrade.Model;

namespace PortfolioSmarts.Questrade
{
	public class QuestradeClient : HttpClient
	{
		public QuestradeClient()
			: base()
		{
			DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("PortfolioSmarts", "v0.0"));
		}

		public async Task<SessionState> Authenticate(string refreshToken)
		{
			var contentBody = $"grant_type=refresh_token&refresh_token={refreshToken}";
			var message = new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(contentBody)));
			message.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
			var tokenRequestTime = DateTime.UtcNow;
			var response = await PostAsync("https://login.questrade.com/oauth2/token", message);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				var serializer = new DataContractJsonSerializer(typeof(TokenResponse));
				var result = serializer.ReadObject(await response.Content.ReadAsStreamAsync()) as TokenResponse;

				var sessionState = new SessionState(result, tokenRequestTime);
				Console.WriteLine($"{sessionState.RefreshToken}, {sessionState.AccessToken}, {sessionState.TokenType}, {sessionState.TokenExpires}, {sessionState.ApiUrl}");

				return sessionState;
			}

			throw new Exception($"Authenticate error: {response.StatusCode}{Environment.NewLine}{await response.Content.ReadAsStringAsync()}");
		}

		public async Task<IEnumerable<AccountDto>> GetAccounts(SessionState sessionState)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, $"{sessionState.ApiUrl}/v1/accounts");
			request.Headers.Authorization = new AuthenticationHeaderValue(sessionState.TokenType, sessionState.AccessToken);
			var response = await SendAsync(request);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				var serializer = new DataContractJsonSerializer(typeof(GetAccountsResponse));
				var result = serializer.ReadObject(await response.Content.ReadAsStreamAsync()) as GetAccountsResponse;
				return result.Accounts;
			}

			throw new Exception($"GetAccounts error: {response.StatusCode}{Environment.NewLine}{await response.Content.ReadAsStringAsync()}");
		}

		public async Task<IEnumerable<PositionDto>> GetPositions(SessionState sessionState, string accountId)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, $"{sessionState.ApiUrl}/v1/accounts/{accountId}/positions");
			request.Headers.Authorization = new AuthenticationHeaderValue(sessionState.TokenType, sessionState.AccessToken);
			var response = await SendAsync(request);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				var serializer = new DataContractJsonSerializer(typeof(GetPositionsResponse));
				var result = serializer.ReadObject(await response.Content.ReadAsStreamAsync()) as GetPositionsResponse;
				return result.Positions;
			}

			throw new Exception($"GetPositions error: {response.StatusCode}{Environment.NewLine}{await response.Content.ReadAsStringAsync()}");
		}

		public async Task<IEnumerable<BalanceDto>> GetBalances(SessionState sessionState, string accountId) {
			var request = new HttpRequestMessage(HttpMethod.Get, $"{sessionState.ApiUrl}/v1/accounts/{accountId}/balances");
			request.Headers.Authorization = new AuthenticationHeaderValue(sessionState.TokenType, sessionState.AccessToken);
			var response = await SendAsync(request);

			if (response.StatusCode == HttpStatusCode.OK) {
				var serializer = new DataContractJsonSerializer(typeof(GetBalancesResponse));
				var result = serializer.ReadObject(await response.Content.ReadAsStreamAsync()) as GetBalancesResponse;
				return result.Balances;
			}

			throw new Exception($"GetBalances error: {response.StatusCode}{Environment.NewLine}{await response.Content.ReadAsStringAsync()}");
		}
	}
}
