using System;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using PortfolioSmarts.Questrade.Model;

namespace PortfolioSmarts.Questrade
{
    public class QuestradeClient : HttpClient
    {
        private string _refreshToken;
        private string _accessToken;
        private string _apiUrl;
        private string _tokenType;
        private DateTime _tokenRequestTime;
        private TimeSpan _expiry;

        public QuestradeClient()
            : base()
        {
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("PortfolioSmarts", "v0.0"));
        }

        public async Task RedeemRefreshToken(string refreshToken)
        {
            var contentBody = $"grant_type=refresh_token&refresh_token={refreshToken}";
            var message = new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(contentBody)));
            message.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            _tokenRequestTime = DateTime.UtcNow;
            Console.WriteLine("Requesting");
            var response = await this.PostAsync("https://login.questrade.com/oauth2/token", message);
            Console.WriteLine("Responded");
            Console.WriteLine(await response.Content.ReadAsStringAsync());

            var serializer = new DataContractJsonSerializer(typeof(TokenResponse));
            var result = serializer.ReadObject(await response.Content.ReadAsStreamAsync()) as TokenResponse;
            Console.WriteLine("Deserialised");

            Console.WriteLine(result.ToString());
            _refreshToken = result.RefreshToken;
            _accessToken = result.AccessToken;
            _tokenType = result.TokenType;
            _expiry = new TimeSpan(0, 0, result.ExpiresIn);
            _apiUrl = result.ApiServer;

            return;
        }
    }
}
