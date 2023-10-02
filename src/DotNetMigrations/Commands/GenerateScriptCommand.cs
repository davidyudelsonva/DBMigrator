using System.IO;
using System.Linq;
using DotNetMigrations.Core;
using DotNetMigrations.Core.Migrations;

namespace DotNetMigrations.Commands
{
	public class GenerateScriptCommand : CommandBase<GenerateScriptCommandArgs>
	{
		private readonly IMigrationDirectory _migrationDirectory;

		public GenerateScriptCommand()
				: this(new MigrationDirectory("migrate"))
		{
		}

		public GenerateScriptCommand(IMigrationDirectory migrationDirectory)
		{
			_migrationDirectory = migrationDirectory;
		}

		/// <summary>
		/// The name of the command that is typed as a command line argument.
		/// </summary>
		public override string CommandName
		{
			get { return "generate"; }
		}

		/// <summary>
		/// The help text information for the command.
		/// </summary>
		public override string Description
		{
			get { return "Generates a new migration script in the migration directory."; }
		}

		/// <summary>
		/// Creates the .sql file and sends the final message.
		/// </summary>
		protected override void Execute(GenerateScriptCommandArgs args)
		{
			string path = _migrationDirectory.CreateBlankScript(args.MigrationName, args.DatabaseName);

			Log.WriteLine("The new migration script " + Path.GetFileName(path) + " was created successfully!");

			if(!args.SkipLaunchScriptEditor)
				System.Diagnostics.Process.Start(path);

		}
	}
}