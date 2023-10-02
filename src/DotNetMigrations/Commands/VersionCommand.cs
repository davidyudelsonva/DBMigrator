using System;
using System.Linq;
using DotNetMigrations.Core;
using DotNetMigrations.Core.Migrations;
using System.Collections.Generic;
using System.Configuration;

namespace DotNetMigrations.Commands
{
	public class VersionCommand : DatabaseCommandBase<DatabaseCommandArguments>
	{
		private readonly IMigrationDirectory _migrationDirectory;

		public VersionCommand()
				: this(new MigrationDirectory("migrate"))
		{
		}

		public VersionCommand(IMigrationDirectory migrationDirectory)
		{
			_migrationDirectory = migrationDirectory;
		}

		/// <summary>
		/// The name of the command that is typed as a command line argument.
		/// </summary>
		public override string CommandName
		{
			get { return "version"; }
		}

		/// <summary>
		/// The help text information for the command.
		/// </summary>
		public override string Description
		{
			get { return "Displays the latest version of the database and the migration scripts."; }
		}

		/// <summary>
		/// Executes the Command's logic.
		/// </summary>
		protected override void Execute(DatabaseCommandArguments args)
		{
			// Obtain Latest Script Version
			var scripts = _migrationDirectory.GetScripts(args.Connection);
			long scriptVersion = GetLatestScriptVersion(scripts);
			
			// Obtain Latest Database Version
			var appliedMigrations = GetDatabaseVersion();

			var unappliedScripts = scripts.Where(s => !appliedMigrations.Contains(s.Version));

			long databaseVersion = appliedMigrations.Max();


			if(!string.IsNullOrEmpty(ConfigurationManager.AppSettings["OutputSql"]))
				Log.WriteLine($"Using Connection String {Database.ConnectionString} ");

			Log.WriteLine("Database is at version:".PadRight(30) + databaseVersion);
			Log.WriteLine("Scripts are at version:".PadRight(30) + scriptVersion);
			Log.WriteLine("Pending Scripts:".PadRight(30) + unappliedScripts.Count());

			if (databaseVersion == scriptVersion && !unappliedScripts.Any())
			{
				Log.WriteLine(string.Empty);
				Log.WriteLine("Your database is up-to-date!");
			}
			else
			{
				foreach(var script in unappliedScripts)
				{
					Log.WriteLine("--" + script.FilePath);
				}
			}
		}

		/// <summary>
		/// Retrieves the latest migration script version from the migration directory.
		/// </summary>
		/// <returns>The latest script version</returns>
		private long GetLatestScriptVersion(IEnumerable<IMigrationScriptFile> files)
		{
			var orderedfiles = files.OrderByDescending(x => x.Version);

			IMigrationScriptFile latestFile = orderedfiles.FirstOrDefault();

			if (latestFile != null)
			{
				return latestFile.Version;
			}

			return 0;
		}
	}
}