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
            var accounts = await _api.GetAccounts();
            Console.WriteLine($"Accounts{Environment.NewLine}--------");
            foreach (var account in accounts)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                var total = 0M;
                var positions = await _api.GetPositions(account);
                foreach (var position in positions)
                {
                    decimal? currentValue = null;
                    if (position.ExtraData.TryGetValue("CurrentValue", out var outVar))
                    {
                        currentValue = Convert.ToDecimal(outVar);
                        total += currentValue.Value;
                    }
                    decimal? currentPrice = null;
                    if (position.ExtraData.TryGetValue("CurrentPrice", out var outPrice))
                    {
                        currentPrice = Convert.ToDecimal(outPrice);
                    }
                    var valStr = currentValue != null ? currentValue.Value.ToString("F2") : "--";
                    var priceStr = currentPrice != null ? currentPrice.Value.ToString("F2") : "--";
                    sb.AppendLine($"  {position.Security.Symbol} - {position.Shares} x {priceStr} = {valStr}");
                }

                Console.WriteLine($"{account.ExternalId} - {account.Name} = {total.ToString("F2")}");
                Console.WriteLine(sb.ToString());
            }
        }
    }
}
