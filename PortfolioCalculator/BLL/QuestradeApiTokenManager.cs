using System;
using System.IO;

using BLL.Interfaces;

using QuestradeAPI;

namespace BLL
{
	public class QuestradeApiTokenManager : IDisposable, IQuestradeApiTokenManager
	{
		private readonly IConfiguration _config;
		private bool _disposed;
		private AuthenticationInfoImplementation _authToken;
		private string _refreshToken;

		public QuestradeApiTokenManager(IConfiguration config)
		{
			_config = config;
		}

		public AuthenticationInfoImplementation GetAuthToken()
		{
			if (_authToken == null)
			{
				_authToken = AuthAgent.GetInstance().Authenticate(GetRefreshToken(), isDemo: false);
				SetRefreshToken(_authToken.RefreshToken);
			}
			else if (!_authToken.IsValid)
			{
				_authToken.Reauthenticate();
				SetRefreshToken(_authToken.RefreshToken);
			}

			if (!_authToken.IsValid)
				throw new Exception(string.Format("Couldn't authenticate with API. message: {0}", _authToken.ErrorMessage));

			return _authToken;
		}

		private string GetRefreshToken()
		{
			if (string.IsNullOrEmpty(_refreshToken))
				_refreshToken = ReadRefreshTokenFromFile();

			return _refreshToken;
		}

		private string ReadRefreshTokenFromFile()
		{
			var qapikeyFilePath = GetApiKeyFilePath();

			using (var qapikeyReader = new StreamReader(qapikeyFilePath))
			{
				return qapikeyReader.ReadToEnd();
			}
		}

		private void SetRefreshToken(string newRefreshToken)
		{
			var qapikeyFilePath = GetApiKeyFilePath();

			using (var qapikeyWriter = new StreamWriter(qapikeyFilePath, false))
			{
				qapikeyWriter.Write(newRefreshToken);
			}

			_refreshToken = newRefreshToken;
		}

		private string GetApiKeyFilePath()
		{
			var dataDir = _config.DataDirectoryPath;

			if (!Directory.Exists(dataDir))
			{
				var message = string.Format("Data directory at {0} does not exist.", dataDir);
				Console.Error.WriteLine(message);
				throw new Exception(message);
			}

			var qapikeyFilePath = Path.Combine(dataDir, _config.QuestradeApiKeyFileName);
			if (!File.Exists(qapikeyFilePath))
			{
				var message = string.Format("qapikey file at {0} does not exist.", qapikeyFilePath);
				Console.Error.WriteLine(message);
				throw new Exception(message);
			}
			return qapikeyFilePath;
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