using System;
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

        }

        public async Task<string> GetStuff()
        {
            this.DefaultRequestHeaders.Add("User-Agent", "PortfolioSmarts-v0");

            var stringTask = this.GetStringAsync("https://www.google.com");
            var msg = await stringTask;
            return msg;
        }
    }
}
