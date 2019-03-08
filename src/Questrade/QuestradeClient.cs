using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PortfolioSmarts.Questrade
{
    public class QuestradeClient : HttpClient
    {
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
            var response = await this.PostAsync("https://login.questrade.com/oauth2/token", message);

            Console.WriteLine(await response.Content.ReadAsStringAsync());
            Console.WriteLine($"{response.StatusCode} {response.ReasonPhrase}");

            return;
        }
    }
}
