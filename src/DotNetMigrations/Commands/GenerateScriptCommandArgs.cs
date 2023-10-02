using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DotNetMigrations.Core;

namespace DotNetMigrations.Commands
{
	public class GenerateScriptCommandArgs : CommandArguments
	{
		[Required(ErrorMessage = "-name is required")]
		[Argument("name", "n", "Name of the migration script to generate",
				Position = 1,
				ValueName = "migration_name")]
		public string MigrationName { get; set; }

		[Required(ErrorMessage = "-database is required")]
		[Argument("database", "d", "Name of the database folder to place script in",
				Position = 0,
				ValueName = "DatabaseName")]
		public string DatabaseName { get; set; }

		[Argument("launch","l","Skips launching an Editor after creating the script",
			Position = 2,
			ValueName = "SkipLaunchScriptEditor"
			)]
		public bool SkipLaunchScriptEditor { get; set; }
	}
}