using System;
using System.Threading.Tasks;
using PortfolioSmarts.Questrade;

namespace PortfolioSmarts.PortfolioApp
{
    class Program
    {
        private static void Main(string[] args)
        {
            var program = new Program();
            program.ExecuteClient().Wait();
        }

        private readonly QuestradeClient _client;
        
        private Program()
        {
            _client = new QuestradeClient();
        }

        private async Task ExecuteClient()
        {
            Console.Write("Enter refresh token: ");
            var refreshToken = Console.ReadLine();
            Console.WriteLine();
            await _client.RedeemRefreshToken(refreshToken);

            Console.WriteLine("Done");
            return;
        }
    }
}
