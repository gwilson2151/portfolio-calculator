using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

using BLL;

using Contracts;

using NUnit.Framework;

namespace Tests.BLL
{
	[TestFixture, Explicit]
	public class QuestradeApiTestBed
	{
		[Test]
		public void Test()
		{
			var dataDir = Path.GetFullPath(Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["DataDirectoryLocation"]));

			if (!Directory.Exists(dataDir))
			{
				Console.Error.WriteLine("Data directory at {0} does not exist.", dataDir);
				return;
			}

			var qapikeyFilePath = Path.Combine(dataDir, "qapikey");
			if (!File.Exists(qapikeyFilePath))
			{
				Console.Error.WriteLine("qapikey file at {0} does not exist.", qapikeyFilePath);
				return;
			}

			string qapikey;
			using (var qapikeyReader = new StreamReader(qapikeyFilePath))
			{
				qapikey = qapikeyReader.ReadToEnd();
			}

			using (var api = new QuestradeService(qapikey))
			{
				var accounts = api.GetAccounts();

				foreach (var account in accounts)
				{
					account.Positions = api.GetPositions(account);
				}
			}
		}
	}
}