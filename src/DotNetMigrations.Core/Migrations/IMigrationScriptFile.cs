using System;
using System.Linq;

namespace DotNetMigrations.Core.Migrations
{
	public interface IMigrationScriptFile
	{
		string Database { get; }
		string FilePath { get; }
		long Version { get; }
		string Name { get; }
		string Author { get; }
		string Ticket_Number { get; set; }
		string Description { get; set; }
		string CreatedDate { get; set; }


		MigrationScriptContents Read();
		void Write(MigrationScriptContents contents);
	}
}