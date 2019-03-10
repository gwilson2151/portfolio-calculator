using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortfolioSmarts.Domain;

namespace PortfolioSmarts.Questrade
{
    public class QuestradeApi
    {
        private readonly QuestradeClient _client;
        private SessionState _sessionState;

        public QuestradeApi(QuestradeClient client)
        {
            _client = client;
        }

        public async Task Initialise(string refreshToken)
        {
            _sessionState = await _client.Authenticate(refreshToken);
        }

        public async Task<IEnumerable<Account>> GetAccounts()
        {
            var qAccounts = await _client.GetAccounts(_sessionState);

            return qAccounts.Select(a => new Account {
                Id = int.Parse(a.Number)
            });
        }

        private async Task EnsureSessionAuthentication()
        {
            if (!_sessionState.SessionValid()) {
                _sessionState = await _client.Authenticate(_sessionState.RefreshToken);
            }
        }
    }
}