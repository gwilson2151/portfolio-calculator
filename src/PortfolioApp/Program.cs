using System;
using System.Linq;
using System.Threading.Tasks;
using PortfolioSmarts.Questrade;

namespace PortfolioSmarts.PortfolioApp
{
    class Program
    {
        private readonly QuestradeApi _api;
        
        private Program()
        {
            _api = new QuestradeApi(new QuestradeClient());
        }

        private static void Main(string[] args)
        {
            char op;
            var program = new Program();
            program.InitialiseApi().Wait();

            do {
                Console.WriteLine("Perform an operation by pressing its key:");
                Console.WriteLine("  [S]how Accounts");
                Console.WriteLine("  Calculate [W]eights");
                Console.WriteLine("  E[x]it");

                op = Console.ReadKey(true).KeyChar;

                if (op == 's') {
                    program.ShowAccounts().Wait();
                } else if (op == 'w') {
                    program.CalculateWeights().Wait();
                } else if (op == 'x') {
                    Console.WriteLine("Exiting.");
                } else {
                    Console.WriteLine($"No operation for [{op}].");
                }

                Console.WriteLine();
            } while (op != 'x');
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

        private async Task CalculateWeights() {
            var total = 0M;
            var loader = new HardCodedLoader();
            var accountsTask = _api.GetAccounts();
            var categories = await loader.LoadCategories();
            var weightsTask = loader.LoadWeights(categories);

            var portfolioCategory = categories.Where(c => c.Id == HardCodedLoader.PortfolioCategoryId).Single();
            var weightCalc = portfolioCategory.Values.ToDictionary(v => v, v => 0M);

            var accounts = await accountsTask;
            var getPositionsTasks = accounts.Select(a => _api.GetPositions(a));
            var weights = await weightsTask;
            foreach (var positionTask in getPositionsTasks) {
                var positions = await positionTask;
                foreach (var position in positions) {
                    decimal currentValue;
                    if (position.ExtraData.TryGetValue("CurrentValue", out var outVar))
                    {
                        currentValue = Convert.ToDecimal(outVar);
                    } else {
                        Console.WriteLine($"{position.Security.Symbol} has no value in {position.Account.Name}.");
                        continue;
                    }
                    var weight = weights[position.Security.Symbol].Where(w => w.Value.Category == portfolioCategory).Single();
                    weightCalc[weight.Value] = weightCalc[weight.Value] + currentValue;
                    total += currentValue;
                }
            }

            foreach (var kvp in weightCalc) {
                Console.WriteLine($"{kvp.Key.Name} - {kvp.Value.ToString("F2")} - {(kvp.Value / total).ToString("F2")}");
            }
            Console.WriteLine($"Total - {total}");
        }
    }
}
