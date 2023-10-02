using System;
using System.Data.Common;
using System.Linq;
using DotNetMigrations.Core.Data;
using System.Collections.Generic;

namespace DotNetMigrations.Core
{
	/// <summary>
	/// Base class for commands that will interact with the database.
	/// 
	/// Using this base class adds the requirement that the 2nd
	/// command argument must always be the connection string.
	/// </summary>
	public abstract class DatabaseCommandBase<TArgs> : CommandBase<TArgs>
			where TArgs : DatabaseCommandArguments, new()
	{
		private readonly ConnectionStringFactory _connectionStringFactory;

		protected DatabaseCommandBase() : this(new ConnectionStringFactory())
		{
			CommandStarting += OnCommandStarting;
			CommandEnded += OnCommandEnded;
		}

		protected DatabaseCommandBase(ConnectionStringFactory connectionStringFactory)
		{
			_connectionStringFactory = connectionStringFactory;
		}

		protected DataAccess Database { get; private set; }

		private void OnCommandStarting(object sender, CommandEventArgs<TArgs> e)
		{
			//  initialize the data access class
			string connStr = GetConnectionString(e.CommandArguments);
			Database = DataAccessFactory.Create(connStr,Log);

			//  perform the database initialization
			Database.OpenConnection();
			var dbInit = new DatabaseInitializer(Database);
			dbInit.Initialize();
		}

		private string GetConnectionString(TArgs args)
		{
			string connStrArg = args.Connection;
			string connStr;
			if (_connectionStringFactory.IsConnectionString(connStrArg))
			{
				connStr = connStrArg;
			}
			else
			{
				connStr = _connectionStringFactory.GetConnectionString(connStrArg);
			}
			return connStr;
		}

		private void OnCommandEnded(object sender, CommandEventArgs<TArgs> e)
		{
			if (Database != null)
			{
				Database.Dispose();
			}
		}

		/// <summary>
		/// Retrieves the current schema version of the database.
		/// </summary>
		/// <returns>The current version of the database.</returns>
		protected List<long> GetDatabaseVersion()
		{
			const string command = "SELECT [version] FROM [schema_migrations]";

			var result = new List<long>();

			try
			{
				using (DbCommand cmd = Database.CreateCommand())
				{
					cmd.CommandText = command;
					var reader = cmd.ExecuteReader();
					if (!reader.HasRows)
						return result;

					while (reader.Read())
					{
						result.Add(long.Parse(reader.GetString(0)));
					}
				}
			}
			catch (DbException ex)
			{
				Log.WriteError(ex.Message);
			}

			return result;
		}

	}
}