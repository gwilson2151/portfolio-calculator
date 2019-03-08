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
            var stuff = await _client.GetStuff();
            Console.WriteLine(stuff);
            Console.WriteLine("Done");
            return;
        }
    }
}
