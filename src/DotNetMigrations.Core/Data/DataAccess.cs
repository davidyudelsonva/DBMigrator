using DotNetMigrations.Core.Migrations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Transactions;

namespace DotNetMigrations.Core.Data
{
	public class DataAccess : IDisposable
	{
		private readonly DbConnection _connection;
		private readonly DbProviderFactory _factory;
		private readonly string _provider;
		private readonly int? _commandTimeout;

		public virtual ILogger Log { get; protected set; }


		public DataAccess(DbProviderFactory factory, string connectionString, string provider, int? commandTimeout, ILogger logger)
		{
			_factory = factory;
			_provider = provider;
			_commandTimeout = commandTimeout;
			ConnectionString = connectionString;
			_connection = GetConnection(connectionString);
			DatabaseName = _connection.Database;
			Log = logger;
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (_connection != null)
				_connection.Dispose();
		}

		#endregion

		public string ConnectionString { get; protected set; }

		public string DatabaseName { get; protected set; }

		public void OpenConnection()
		{
			_connection.Open();
		}

		public void CloseConnection()
		{
			_connection.Close();
		}

          public DbConnection GetDbConnection()
          {
               return _connection;
          }

		public DbCommand CreateCommand()
		{
			DbCommand cmd = _connection.CreateCommand();
			return cmd;
		}

		public DbTransaction BeginTransaction()
		{
			DbTransaction tran = _connection.BeginTransaction();
			return tran;
		}

		/// <summary>
		/// Creates a connection object based on the Provider specified in the connection string
		/// </summary>
		/// <param name="connectionString">The connection string to use.</param>
		/// <returns>A typed database connection object</returns>
		private DbConnection GetConnection(string connectionString)
		{
			DbConnection conn = _factory.CreateConnection();
			if (conn == null)
			{
				throw new InvalidOperationException("Factory failed to create connection. Returned null.");
			}

			if (!connectionString.ToLower().Contains("Connect Timeout"))
				connectionString += ";Connect Timeout=15;";

			conn.ConnectionString = connectionString;
			

			return conn;
		}

		/// <summary>
		/// Executes a SQL script. Includes support for executing
		/// scripts in batches using the GO keyword.
		/// </summary>
		public void ExecuteScript(DbTransaction tran, string script)
		{
			const string providerVariableName = "/*DNM:PROVIDER*/";

			if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["OutputSql"]))
				Log.WriteLine(script);

			var batches = new ScriptSplitter(script);
			foreach (var batch in batches)
			{
				// replace the provider name token in the script
				var bakedBatch = batch.Replace(providerVariableName, _provider, StringComparison.OrdinalIgnoreCase);


				using (var cmd = CreateCommand())
				{
					cmd.Transaction = tran;
					cmd.CommandText = bakedBatch;

					if (_commandTimeout.HasValue)
						cmd.CommandTimeout = _commandTimeout.Value;
					try
					{
						cmd.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						throw new SqlExecutionException(bakedBatch, ex);
					}
				}
			}
		}

          public void ExecuteScript(string script)
          {
               const string providerVariableName = "/*DNM:PROVIDER*/";

               if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["OutputSql"]))
                    Log.WriteLine(script);

               var batches = new ScriptSplitter(script);
               foreach (var batch in batches)
               {
                    // replace the provider name token in the script
                    var bakedBatch = batch.Replace(providerVariableName, _provider, StringComparison.OrdinalIgnoreCase);


                    using (var cmd = CreateCommand())
                    {
                         cmd.CommandText = bakedBatch;

                         if (_commandTimeout.HasValue)
                              cmd.CommandTimeout = _commandTimeout.Value;
                         try
                         {
                              cmd.ExecuteNonQuery();
                         }
                         catch (Exception ex)
                         {
                              throw new SqlExecutionException(bakedBatch, ex);
                         }
                    }
               }
          }

          /// <summary>
          /// Retrieves the current schema version of the database.
          /// </summary>
          /// <returns>The current version of the database.</returns>
          public List<SchemaMigration> GetAppliedMigrations()
		{
			string command = "SELECT " +
				string.Join(", ", DatabaseInitializer.SchemaMigrationColumns.Select(m => $"[{m.Name}]")) +
				" FROM [schema_migrations]";

			var result = new List<SchemaMigration>();

			try
			{
				using (DbCommand cmd = CreateCommand())
				{
					cmd.CommandText = command;
					var reader = cmd.ExecuteReader();
					if (!reader.HasRows)
						return result;

					while (reader.Read())
					{
						int x = 0;
						result.Add(
							new SchemaMigration()
							{
								Id = reader.GetInt32(x++),
								Version = long.Parse(reader.GetString(x++)),
								Name = reader.IsDBNull(x++) ? null : reader.GetString(x-1),
								Author = reader.IsDBNull(x++) ? null : reader.GetString(x-1),
								Ticket_Number = reader.IsDBNull(x++) ? null : reader.GetString(x-1),
								Description = reader.IsDBNull(x++) ? null : reader.GetString(x-1),
								CreatedDate = reader.IsDBNull(x++) ? null : (DateTime?) reader.GetDateTime(x-1),
								AppliedDate = reader.IsDBNull(x++) ? null : (DateTime?) reader.GetDateTime(x-1),
								AppliedByUser = reader.IsDBNull(x++) ? null : reader.GetString(x-1)
							});
					}
				}
			}
			catch (DbException ex)
			{
				Log.WriteError(ex.Message);
			}

			return result;
		}

          public bool ExecuteMigrationScriptsInReleaseMode(IEnumerable<Tuple<IMigrationScriptFile, string>> scripts, 
               DbConnection conn, ILogger log, MigrationDirection direction, string dbMigratorVersionNumber)
          {
               IMigrationScriptFile currentScript = null;
               IList<SchemaMigration> schemaMigrations = new List<SchemaMigration>();

               // Create the TransactionScope to execute the commands, guaranteeing
               // that both commands can commit or roll back as a single unit of work.
               using (TransactionScope scope = new TransactionScope())
               {
                    try
                    {
                         foreach (var script in scripts)
                         {
                              currentScript = script.Item1;
                              
                              Log.WriteLine(string.Format("--Applying Script {0}", currentScript.FilePath));
                              var scriptText = script.Item2;
                              ExecuteScript(scriptText);
                              if (direction == MigrationDirection.Up)
                                   UpdateSchemaVersionUp(currentScript, log, dbMigratorVersionNumber);
                              else
                                   UpdateSchemaVersionDown(currentScript, log);


                         }

                         scope.Complete();

                         Log.WriteLine($"All scripts completed in RELEASE mode for {conn.Database} database migration.");
                         return true;
                    }
                    catch (Exception ex)
                    {
                         Log.WriteError($"One or more scripts erred in RELEASE mode for {conn.Database} database migration.");
                         return false;
                    }
                    
               }
          }

		public bool ExecuteMigrationScripts(IEnumerable<Tuple<IMigrationScriptFile, string>> scripts, Action<DbTransaction, IMigrationScriptFile, ILogger> updateVersionAction)
		{
			IMigrationScriptFile currentScript = null;

			foreach (var script in scripts)
			{
				using (DbTransaction tran = BeginTransaction())
				{
					try
					{
						currentScript = script.Item1;
						Log.WriteLine(string.Format("--Applying Script {0}", currentScript.FilePath));
						var scriptText = script.Item2;

						ExecuteScript(tran, scriptText);
						updateVersionAction(tran, currentScript, Log);
						tran.Commit();
					}
					catch (Exception ex)
					{
						tran.Rollback();

						string filePath = (currentScript == null) ? "NULL" : currentScript.FilePath;
						Log.WriteError("Error executing migration script: {0}\n", filePath);
						Log.WriteError(ex.Message);
						Exception innerEx = ex.InnerException;
						while (innerEx != null)
						{
							Log.WriteLine(string.Empty);
							Log.WriteError(innerEx.Message);

							innerEx = innerEx.InnerException;
						}

                              return false;
                         }
				}
			}

               return true;
          }

		/// <summary>
		/// Updates the database with the version provided
		/// </summary>
		/// <param name="transaction">The transaction to execute the command in</param>
		/// <param name="version">The version to log</param>
		public static void UpdateSchemaVersionUp(DbTransaction transaction, IMigrationScriptFile script, ILogger log)
		{
			using (DbCommand cmd = transaction.CreateCommand())
			{
				cmd.CommandText = string.Format("USE [{0}]", transaction.Connection.Database);
				if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["OutputSql"]))
				{
					log.WriteLine(cmd.CommandText);
				}
				cmd.ExecuteNonQuery();
			}

			using (var cmd = transaction.CreateCommand())
			{
                //  TODO: We should probably move all these random scripts to a scripts.resx to better track them.
                cmd.CommandText = @"INSERT INTO [schema_migrations] (
                    version,
                    name,
                    author,
                    ticket_number,
                    description,
                    created_date,
                    applied_date,
                    applied_by_user
                ) VALUES (
                    @version,
                    @name,
                    @author,
                    @ticket_number,
                    @description,
                    @created_date,
                    GETDATE(),
                    @applied_by_user
                )";

                var versionParam = cmd.CreateParameter();
                versionParam.ParameterName = "@version";
                versionParam.Value = script.Version;
                cmd.Parameters.Add(versionParam);

                var nameParam = cmd.CreateParameter();
                nameParam.ParameterName = "@name";
                nameParam.Value = script.Name;
                cmd.Parameters.Add(nameParam);

                var authorParam = cmd.CreateParameter();
                authorParam.ParameterName = "@author";
                authorParam.Value = script.Author;
                cmd.Parameters.Add(authorParam);

                var ticketNumberParam = cmd.CreateParameter();
                ticketNumberParam.ParameterName = "@ticket_number";
                ticketNumberParam.Value = script.Ticket_Number;
                cmd.Parameters.Add(ticketNumberParam);

                var descriptionParam = cmd.CreateParameter();
                descriptionParam.ParameterName = "@description";
                descriptionParam.Value = script.Description;
                cmd.Parameters.Add(descriptionParam);

                var createdDateParam = cmd.CreateParameter();
                createdDateParam.ParameterName = "@created_date";
                createdDateParam.Value = script.CreatedDate;
                cmd.Parameters.Add(createdDateParam);

                var appliedByUserParam = cmd.CreateParameter();
                appliedByUserParam.ParameterName = "@applied_by_user";
                appliedByUserParam.Value = Environment.UserName;
                cmd.Parameters.Add(appliedByUserParam);

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["OutputSql"]))
				{
					log.WriteLine(cmd.CommandText);
				}
				cmd.ExecuteNonQuery();
			}
		}

          public void UpdateSchemaVersionUp(IMigrationScriptFile script, ILogger log, string dbMigratorVersionNumber)
          {
               var conn = GetDbConnection();
               using (DbCommand cmd = conn.CreateCommand())
               {
                    cmd.CommandText = string.Format("USE [{0}]", conn.Database);
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["OutputSql"]))
                    {
                         log.WriteLine(cmd.CommandText);
                    }
                    cmd.ExecuteNonQuery();
               }

               using (DbCommand cmd = conn.CreateCommand())
               {
                    //  TODO: We should probably move all these random scripts to a scripts.resx to better track them.
                    cmd.CommandText = @"INSERT INTO [schema_migrations] (
                        version,
                        name,
                        author,
                        ticket_number,
                        description,
                        created_date,
                        applied_date,
                        applied_by_user
                    ) VALUES (
                        @version,
                        @name,
                        @author,
                        @ticket_number,
                        @description,
                        @created_date,
                        GETDATE(),
                        @applied_by_user
                    )";

                    var versionParam = cmd.CreateParameter();
                    versionParam.ParameterName = "@version";
                    versionParam.Value = script.Version;
                    cmd.Parameters.Add(versionParam);

                    var nameParam = cmd.CreateParameter();
                    nameParam.ParameterName = "@name";
                    nameParam.Value = script.Name;
                    cmd.Parameters.Add(nameParam);

                    var authorParam = cmd.CreateParameter();
                    authorParam.ParameterName = "@author";
                    authorParam.Value = script.Author;
                    cmd.Parameters.Add(authorParam);

                    var ticketNumberParam = cmd.CreateParameter();
                    ticketNumberParam.ParameterName = "@ticket_number";
                    ticketNumberParam.Value = script.Ticket_Number;
                    cmd.Parameters.Add(ticketNumberParam);

                    var descriptionParam = cmd.CreateParameter();
                    descriptionParam.ParameterName = "@description";
                    descriptionParam.Value = script.Description;
                    cmd.Parameters.Add(descriptionParam);

                    var createdDateParam = cmd.CreateParameter();
                    createdDateParam.ParameterName = "@created_date";
                    createdDateParam.Value = script.CreatedDate;
                    cmd.Parameters.Add(createdDateParam);

                    var appliedByUserParam = cmd.CreateParameter();
                    appliedByUserParam.ParameterName = "@applied_by_user";
                    appliedByUserParam.Value = Environment.UserName;
                    cmd.Parameters.Add(appliedByUserParam);

                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["OutputSql"])) {
                             log.WriteLine(cmd.CommandText);
                        }
                    cmd.ExecuteNonQuery();
               }
          }

          public void UpdateSchemaVersionDown(IMigrationScriptFile script, ILogger log)
          {
               var conn = GetDbConnection();
               using (DbCommand cmd = conn.CreateCommand())
               {
                    cmd.CommandText = string.Format("USE [{0}]", conn.Database);
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["OutputSql"]))
                    {
                         log.WriteLine(cmd.CommandText);
                    }
                    cmd.ExecuteNonQuery();
               }

               string sql = "DELETE FROM [schema_migrations] WHERE version = {0}";

               using (DbCommand cmd = conn.CreateCommand())
               {
                    cmd.CommandText = string.Format(sql, script.Version);
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["OutputSql"]))
                    {
                         log.WriteLine(cmd.CommandText);
                    }
                    cmd.ExecuteNonQuery();
               }
          }

          /// <summary>
          /// Removes the provided version from the database log table.
          /// </summary>
          /// <param name="transaction">The transaction to execute the command in</param>
          /// <param name="version">The version to log</param>
          public static void UpdateSchemaVersionDown(DbTransaction transaction, IMigrationScriptFile script, ILogger log)
          {
               using (DbCommand cmd = transaction.CreateCommand())
               {
                    cmd.CommandText = string.Format("USE [{0}]", transaction.Connection.Database);
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["OutputSql"]))
                    {
                         log.WriteLine(cmd.CommandText);
                    }
                    cmd.ExecuteNonQuery();
               }

               string sql = "DELETE FROM [schema_migrations] WHERE version = {0}";

               using (DbCommand cmd = transaction.CreateCommand())
               {
                    cmd.CommandText = string.Format(sql, script.Version);
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["OutputSql"]))
                    {
                         log.WriteLine(cmd.CommandText);
                    }
                    cmd.ExecuteNonQuery();
               }
          }
     }
}