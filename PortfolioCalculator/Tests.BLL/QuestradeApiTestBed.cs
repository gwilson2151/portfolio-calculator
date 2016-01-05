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

			var qapikeyFile = Path.Combine(dataDir, "qapikey");
			if (!File.Exists(qapikeyFile))
			{
				Console.Error.WriteLine("qapikey file at {0} does not exist.", qapikeyFile);
				return;
			}

			var qapikeyReader = new StreamReader(qapikeyFile);
			var qapikey = qapikeyReader.ReadToEnd();

			var accounts = new List<Account>();

			using (var api = new QuestradeService(qapikey))
			{
				var qaccounts = api.GetAccounts();

				foreach (var qaccount in qaccounts)
				{
					var account = new Account
					{
						Name = qaccount.m_number
					};
					var positions = api.GetPositions(qaccount);
					account.Positions.AddRange(positions.Select(qp => new Position
					{
						Account = account,
						Security = new Security
						{
							Symbol = qp.m_symbol
						},
						Shares = Convert.ToDecimal(qp.m_closedQuantity)
					}));
					accounts.Add(account);
				}
			}
		}
	}
}