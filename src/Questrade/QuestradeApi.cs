using System;
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
            await EnsureSessionAuthentication();

            var qAccounts = await _client.GetAccounts(_sessionState);

            return qAccounts.Select(a => new Account {
                ExternalId = a.Number,
                Name = a.Type
            });
        }

        public async Task<IEnumerable<Position>> GetPositions(Account account)
        {
            await EnsureSessionAuthentication();

            var qPositions = await _client.GetPositions(_sessionState, account.ExternalId);

            return qPositions.Select(p => new Position {
                Account = account,
                Security = new Security {
                    Symbol = p.Symbol
                },
                Shares = Convert.ToDecimal(p.OpenQuantity),
                ExtraData = new Dictionary<string, object> {
                    { "CurrentValue", Convert.ToDecimal(p.CurrentMarketValue) },
                    { "CurrentPrice", Convert.ToDecimal(p.CurrentPrice) }
                }
            });
        }

        private async Task EnsureSessionAuthentication()
        {
            if (_sessionState == null)
            {
                throw new Exception("QuestradeApi must be initialised.");
            }
            else if (!_sessionState.SessionValid())
            {
                _sessionState = await _client.Authenticate(_sessionState.RefreshToken);
            }
        }
    }
}