using System;
using System.Linq;
using System.Threading.Tasks;
using PortfolioSmarts.Domain;
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
                var balancesTask = _api.GetBalances(account);
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

                var balances = await balancesTask;
                foreach (var balance in balances) {
                    if (balance.Amount > 0M) {
                        total += balance.Amount;
                        sb.AppendLine($"  {balance.Currency.ToString()}    = {balance.Amount.ToString("F2")}");
                    }
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
            var loadPositionsTasks = accounts.Select(a => LoadPositions(a));
            var loadBalancesTasks = accounts.Select(a => LoadBalances(a));
            var weights = await weightsTask;
            foreach (var accountPositionTask in loadPositionsTasks) {
                var account = await accountPositionTask;
                foreach (var position in account.Positions) {
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

            foreach (var accountBalanceTask in loadBalancesTasks) {
                var account = await accountBalanceTask;
                foreach (var balance in account.Balances) {
                    var weight = weights[balance.Currency.ToString()].Where(w => w.Value.Category == portfolioCategory).Single();
                    weightCalc[weight.Value] = weightCalc[weight.Value] + balance.Amount;
                    total += balance.Amount;
                }
            }

            foreach (var kvp in weightCalc) {
                Console.WriteLine($"{kvp.Key.Name,-20} - {kvp.Value.ToString("F2"),9} - {(kvp.Value / total).ToString("P"),6}");
            }
            Console.WriteLine($"Total                - {total.ToString("F2"),9}");
        }

        private async Task<Account> LoadPositions(Account account) {
            var loadedAccount = new Account(account);

            var positions = await _api.GetPositions(loadedAccount);
            loadedAccount.Positions = loadedAccount.Positions.Concat(positions);

            return loadedAccount;
        }

        private async Task<Account> LoadBalances(Account account) {
            var loadedAccount = new Account(account);

            var balances = await _api.GetBalances(loadedAccount);
            loadedAccount.Balances = loadedAccount.Balances.Concat(balances);

            return loadedAccount;
        }
    }
}
