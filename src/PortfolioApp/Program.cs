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
            program.InitialiseApi().Wait();
            program.ShowAccounts().Wait();
        }

        private readonly QuestradeApi _api;
        
        private Program()
        {
            _api = new QuestradeApi(new QuestradeClient());
        }

        private async Task InitialiseApi()
        {
            Console.Write("Enter refresh token: ");
            var refreshToken = Console.ReadLine();
            await _api.Initialise(refreshToken);
            Console.WriteLine("Initialisation done.");
        }

        private async Task ShowAccounts()
        {
            var blah = await _api.GetAccounts();
        }
    }
}
