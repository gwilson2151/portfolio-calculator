using System;
using System.Collections.Generic;

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

		public List<Questrade.BusinessObjects.Entities.AccountData> GetAccounts()
		{
			EnsureAuthenticated();
			var response = GetAccountsResponse.GetAccounts(_authToken);
			return response.Accounts;
		}

		public List<Questrade.BusinessObjects.Entities.PositionData> GetPositions(Questrade.BusinessObjects.Entities.AccountData account)
		{
			EnsureAuthenticated();
			var response = GetPositionsResponse.GetPositions(_authToken, account.m_number);
			return response.Positions;
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