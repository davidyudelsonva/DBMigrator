using DotNetMigrations.Core;
using DotNetMigrations.Core.Migrations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMigrations.Commands
{
	public class VersionAllCommand : CommandBase<VersionAllCommandArgs>
	{
		private readonly IMigrationDirectory _migrationDirectory;

		public VersionAllCommand() 
			: this(new MigrationDirectory("migrate"))
		{

		}

		public VersionAllCommand(IMigrationDirectory migrationDirectory)
		{
			_migrationDirectory = migrationDirectory;

		}

			

		public override string CommandName {
			get { return "versionAll"; }
		}

		public override string Description
		{
			get	{ return "Lists versions for all configured Databases";	}
		}

		protected override void Execute(VersionAllCommandArgs args)
		{
			var directories = Directory.EnumerateDirectories(_migrationDirectory.GetPath(null,Log));

			foreach(var dir in directories)
			{
				var databaseName = Path.GetFileName(dir);


				Log.WriteLine($"\nVersion for {databaseName} database\n");

				var command = new VersionCommand(new MigrationDirectory("migrate"));
				command.Log = Log;
				var versionArgs = new DatabaseCommandArguments()
				{
					Connection = databaseName
				};
				command.Run(versionArgs);
			}



		}
	}
}
