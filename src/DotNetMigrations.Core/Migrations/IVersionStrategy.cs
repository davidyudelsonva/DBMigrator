using System;
using System.Linq;

namespace DotNetMigrations.Core.Migrations
{
    public interface IVersionStrategy
    {
        long GetNewVersionNumber(IMigrationDirectory migrationDirectory);
    }
}