using System;

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

		private void GetAccounts()
		{
			EnsureAuthenticated();
			var response = GetAccountsResponse.GetAccounts(_authToken);

		}

		private void EnsureAuthenticated()
		{
			if (_authToken == null)
			{
				_authToken = AuthAgent.GetInstance().Authenticate(_refreshToken, isDemo: true);
			}
			else if (!_authToken.IsValid)
			{
				_authToken.Reauthenticate();
			}
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