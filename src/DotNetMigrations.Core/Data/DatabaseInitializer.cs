using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetMigrations.Core.Data
{
	/// <summary>
	/// Initializes the database schema with objects
	/// required by DNM.
	/// </summary>
	public class DatabaseInitializer
	{
		private readonly DataAccess _dataAccess;
		private const string MIGRATION_TABLE_NAME = "schema_migrations";
		public static readonly List<ColumnDefinition> SchemaMigrationColumns;

		static DatabaseInitializer()
		{
			SchemaMigrationColumns = new List<ColumnDefinition>
			{
				new ColumnDefinition("id","INT NOT NULL"),
				new ColumnDefinition("version", "[nvarchar](14) NOT NULL"),
				new ColumnDefinition("name", "nvarchar(256) NULL"),
				new ColumnDefinition("author", "nvarchar(256) NULL"),
				new ColumnDefinition("ticket_number", "nvarchar(64) NULL"),
				new ColumnDefinition("description", "nvarchar(1024) NULL"),
				new ColumnDefinition("created_date", "datetime NULL"),
				new ColumnDefinition("applied_date", "datetime NULL"),
				new ColumnDefinition("applied_by_user", "nvarchar(256) NULL"),
                    new ColumnDefinition("dbmigrator_version_number", "nvarchar(256) NULL")
			};
		}

		public DatabaseInitializer(DataAccess dataAccess)
		{
			_dataAccess = dataAccess;
		}

		/// <summary>
		/// Initializes the database schema by creating objects
		/// required by DNM.
		/// </summary>
		public void Initialize()
		{
			// Do nothing if table already exists.
			if (!TableExists(MIGRATION_TABLE_NAME))
				CreateMigrationTable();
			var cols = GetColumnsToAdd(MIGRATION_TABLE_NAME);

			if (!cols.Any())
				return;

			AddColumnsToTable(cols);

		}

		/// <summary>
		/// Creates the migration table into the database.
		/// </summary>
		private void CreateMigrationTable()
		{
			const string createTableCommand = "CREATE TABLE [schema_migrations]([id] INT NOT NULL IDENTITY(1,1) CONSTRAINT [PK_schema_migrations] PRIMARY KEY, [version] [nvarchar](14) NOT NULL)";
			const string firstRecordCommand = "INSERT INTO [schema_migrations] ([version]) VALUES (0)";

			using (var tran = _dataAccess.BeginTransaction())
			{
				try
				{
					using (var cmd = tran.CreateCommand())
					{
						cmd.CommandText = createTableCommand;
						cmd.ExecuteNonQuery();
					}

					using (var cmd = tran.CreateCommand())
					{
						cmd.CommandText = firstRecordCommand;
						cmd.ExecuteNonQuery();
					}

					tran.Commit();
				}
				catch (Exception)
				{
					tran.Rollback();
					throw;
				}
			}
		}


		private void AddColumnsToTable(List<ColumnDefinition> newColumns)
		{
			string updateTableCommand = @"ALTER TABLE [schema_migrations] ADD ";

			updateTableCommand += string.Join(",\n ", newColumns.Select(c => $"[{c.Name}] {c.DataType}"));

			using (var tran = _dataAccess.BeginTransaction())
			{
				try
				{
					using (var cmd = tran.CreateCommand())
					{
						cmd.CommandText = updateTableCommand;
						cmd.ExecuteNonQuery();
					}

					tran.Commit();
				}
				catch (Exception)
				{
					tran.Rollback();
					throw;
				}
			}
		}

		/// <summary>
		/// Returns true/false whether a table with the given
		/// name exists in the database.
		/// 
		/// Throws a SchemaException if more than one
		/// table is found with the given name.
		/// </summary>
		/// <exception cref="SchemaException" />
		private bool TableExists(string tableName)
		{
			const string cmdText = "select count(*) from [INFORMATION_SCHEMA].[TABLES] where [TABLE_NAME] = '{0}'";

			using (var cmd = _dataAccess.CreateCommand())
			{
				cmd.CommandText = string.Format(cmdText, tableName);
				var count = cmd.ExecuteScalar<int>();

				if (count == 1)
				{
					return true;
				}

				if (count > 1)
				{
					throw new SchemaException("More than one [" + tableName + "] table exists!");
				}

				return false;
			}
		}

		private List<ColumnDefinition> GetColumnsToAdd(string tableName)
		{
			string cmdText = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + tableName + "'";

			var existingColumns = new List<string>();

			using (var cmd = _dataAccess.CreateCommand())
			{
				cmd.CommandText = string.Format(cmdText, tableName);
				var reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					existingColumns.Add(reader.GetString(0));
				}


				return SchemaMigrationColumns.Where(c => !existingColumns.Contains(c.Name)).ToList();
			}

		}

	}

	public class ColumnDefinition
	{
		public string Name { get; set; }
		public string DataType { get; set; }

		public ColumnDefinition(string name, string dataType)
		{
			Name = name;
			DataType = dataType;

		}

	}
}