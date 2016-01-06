using System;
using System.Collections.Generic;
using System.Linq;

using Contracts;

using QuestradeAPI;

namespace BLL
{
	public class QuestradeService : IDisposable
	{
		private bool _disposed;

		private readonly string _refreshToken;
		private AuthenticationInfoImplementation _authToken;

		public QuestradeService(string refreshToken)
		{
			_refreshToken = refreshToken;
		}

		public List<Account> GetAccounts()
		{
			EnsureAuthenticated();
			var response = GetAccountsResponse.GetAccounts(_authToken);
			return response.Accounts.Select(a => new Account { Name = a.m_number }).ToList();
		}

		public List<Position> GetPositions(Account account)
		{
			EnsureAuthenticated();
			var response = GetPositionsResponse.GetPositions(_authToken, account.Name);
			return response.Positions.Select(qp => new Position
			{
				Account = account,
				Security = new Security
				{
					Symbol = qp.m_symbol
				},
				Shares = Convert.ToDecimal(qp.m_openQuantity)
			}).ToList();
		}

		private void EnsureAuthenticated()
		{
			if (_authToken == null)
			{
				_authToken = AuthAgent.GetInstance().Authenticate(_refreshToken, isDemo: false);
			}
			else if (!_authToken.IsValid)
			{
				_authToken.Reauthenticate();
			}

			if (!_authToken.IsValid)
				throw new Exception(string.Format("Couldn't authenticate with API. message: {0}", _authToken.ErrorMessage));
		}

		#region IDisposable
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
			{
				if (disposing)
				{
					AuthAgent.GetInstance().Stop();
					_authToken = null;
				}

				_disposed = true;
			}
		}
		#endregion
	}
}