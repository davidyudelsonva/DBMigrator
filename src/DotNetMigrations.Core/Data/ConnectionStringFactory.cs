using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;

namespace DotNetMigrations.Core.Data
{
	public class ConnectionStringFactory
	{
		private readonly IConfigurationManager _configManager;

		public ConnectionStringFactory()
				: this(new ConfigurationManagerWrapper())
		{
		}

		public ConnectionStringFactory(IConfigurationManager configManager)
		{
			_configManager = configManager;
		}


		public IEnumerable<ConnectionStringSettings> GetConnectionStrings()
		{
			foreach(var con in _configManager.ConnectionStrings)
			{
				yield return ((ConnectionStringSettings)con);
			}
		}

		/// <summary>
		/// Gets the named connection string from app.Config
		/// </summary>
		public string GetConnectionString(string name)
		{
			var connStr = _configManager.ConnectionStrings[name];
			if (connStr == null)
				throw new ArgumentException("No connection string was found with the name \"" + name + "\"", nameof(name));

			return connStr.ConnectionString;
		}

		/// <summary>
		/// Returns true/false whether the given value looks like a connection string.
		/// </summary>
		public bool IsConnectionString(string val)
		{
			if (string.IsNullOrEmpty(val))
				return false;

			var csb = new DbConnectionStringBuilder();
			try
			{
				csb.ConnectionString = val;
				return csb.Count > 0;
			}
			catch (ArgumentException)
			{
				return false;
			}
		}
	}
}