using System;
using System.Configuration;
using System.IO;

using BLL.Interfaces;

namespace BLL
{
	public class Configuration : IConfiguration
	{
		public string DataDirectoryPath {
			get
			{
				return Path.GetFullPath(Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["DataDirectoryLocation"]));
			}
		}

		public string QuestradeApiKeyFileName
		{
			get { return ConfigurationManager.AppSettings["QuestradeApiKeyFileName"]; }
		}
	}
}