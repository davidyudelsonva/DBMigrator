using DotNetMigrations.Core;
using DotNetMigrations.Core.Data;
using DotNetMigrations.Core.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMigrations.Commands
{
	public class ApplyCommandArgs : CommandArguments
	{
	}


	public class ApplyCommand : CommandBase<ApplyCommandArgs>
	{

		public ApplyCommand()
		{
		}

		public override string CommandName => "Apply";

		public override string Description => "Run rollbacks and migrations as indicated in the respective directories";

		protected override void Execute(ApplyCommandArgs args)
		{
			var migrateFolderCommand = new MigrateFolderCommand();
			migrateFolderCommand.Log = Log;

			migrateFolderCommand.Run(new MigrateFolderCommandArgs
			{
				Folder = "rollback",
				Direction = MigrationDirection.Down
			});

			migrateFolderCommand.Run(new MigrateFolderCommandArgs
			{
				Folder = "migrate",
				Direction = MigrationDirection.Up
			});

		}

	}
}
