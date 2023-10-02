using System;
using System.Data.Common;
using System.Linq;
using DotNetMigrations.Core;
using DotNetMigrations.Core.Data;
using System.Configuration;
using System.Collections.Generic;
using DotNetMigrations.Core.Migrations;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace DotNetMigrations.Commands
{
	public class MigrateFolderCommandArgs : CommandArguments
	{
		[Required(ErrorMessage = "Migration Folder is Required")]
		[Argument("Folder","f","Folder where migrations to be processes are stored, searches recursively")]
		public string Folder { get; set; }

		[Argument("Version", "v", "Specific versions to be executed.  If not supplied, will run all commands found in the Folder")]
		public List<long> Versions { get; set; }

		[ValueSetValidator("Up", "Down", ErrorMessage = "direction must be 'Up' or 'Down'")]
		[Required(ErrorMessage = "Migration Direction is required.")]
		[Argument("Direction","d","Migration direction.  Up applies the setup portion of the script, Down applies the teardown portion.")]
		public MigrationDirection Direction { get; set; }
          
          [Argument("Is Release Mode", "rm", "True/False to describe if this is being run in Release Mode. If not supplied, will assume FALSE (or no).")]
          public bool IsReleaseMode { get; set; }

          [Argument("DbMigrator Version #", "vn", "Last known version of DbMigrator that was executed against this database.")]
          public string DbMigratorVersionNumber { get; set; }
     }


	public class MigrateFolderCommand : CommandBase<MigrateFolderCommandArgs>
	{
		private ConnectionStringFactory _connectionStringFactory;
          public bool WasMigrationSuccessful;

		public MigrateFolderCommand()
		{
			_connectionStringFactory = new ConnectionStringFactory();
		}

		public override string CommandName => "MigrateFolder";

		public override string Description => "Run all migration scripts for the given arguments";

		protected override void Execute(MigrateFolderCommandArgs args)
		{
			var scriptPreprocessors = Program.Current.ScriptPreprocessorRepository.GetActiveScriptPreprocessors();
               
			Log.WriteLine($"Migrating Scripts from {args.Folder} in the {args.Direction.ToString()} Direction");

			if (scriptPreprocessors.Any())
			{
				Log.WriteLine("Active Script Preprocessors:");
				foreach(var script in scriptPreprocessors)
				{
					Log.WriteLine($"--{script}");
				}
			}

               var wasSuccessfulRun = false;
			var migrationDirectory = new MigrationDirectory(args.Folder);

			foreach (var scripts in migrationDirectory.GetScripts(args.Versions).GroupBy(g => g.Database))
               {
                    Log.WriteLine(args.IsReleaseMode
                         ? $"Beginning RELEASE mode for scripts in database {scripts.Key}"
                         : $"Scripts for database {scripts.Key}");

                    var dataAccess = GetDataAccess(scripts.Key);
				var appliedMigrations = dataAccess.GetAppliedMigrations();
				var appliedMigrationVersions = appliedMigrations.Select(a => a.Version).ToList();

				var scriptsToApply =
						scripts
						.Where(s => args.Direction == MigrationDirection.Down 
							? appliedMigrationVersions.Contains(s.Version)
							: !appliedMigrationVersions.Contains(s.Version))
						.Select(s => 
								new Tuple<IMigrationScriptFile,string>(s,
									 args.Direction == MigrationDirection.Down ?
									s.Read().Teardown
									: s.Read().Setup
								)
						);

				if (args.Direction == MigrationDirection.Down)
					scriptsToApply = scriptsToApply.OrderByDescending(s => s.Item1.Version);
				else
					scriptsToApply = scriptsToApply.OrderBy(s => s.Item1.Version);

				foreach (var preprocessor in scriptPreprocessors)
				{
					scriptsToApply = scriptsToApply.Select(s=> new Tuple<IMigrationScriptFile,string>(s.Item1,preprocessor.ModifyScript(s.Item2)));
				}

                    if (args.IsReleaseMode)
                         wasSuccessfulRun = dataAccess.ExecuteMigrationScriptsInReleaseMode(scriptsToApply, dataAccess.GetDbConnection(), Log, args.Direction, args.DbMigratorVersionNumber);
                    else
                    {
                         wasSuccessfulRun = args.Direction == MigrationDirection.Down ? 
                              dataAccess.ExecuteMigrationScripts(scriptsToApply, DataAccess.UpdateSchemaVersionDown) : 
                              dataAccess.ExecuteMigrationScripts(scriptsToApply, DataAccess.UpdateSchemaVersionUp);
                    }


                    WasMigrationSuccessful = wasSuccessfulRun;
                    Log.WriteLine(wasSuccessfulRun
                         ? $"Migrating Scripts from {args.Folder} in the {args.Direction.ToString().ToUpper()} direction Completed for {scripts.Key} database."
                         : $"Attempt to migrate scripts from {args.Folder} in the {args.Direction.ToString().ToUpper()} direction for {scripts.Key} database was aborted.");
               }

               
          }

		private DataAccess GetDataAccess(string connectionStringName)
		{
			var connectionString = _connectionStringFactory.GetConnectionString(connectionStringName);

			var dataAccess = DataAccessFactory.Create(connectionString, Log);

			//  perform the database initialization
			dataAccess.OpenConnection();
			var dbInit = new DatabaseInitializer(dataAccess);
			dbInit.Initialize();

			return dataAccess;
		}


	}
}