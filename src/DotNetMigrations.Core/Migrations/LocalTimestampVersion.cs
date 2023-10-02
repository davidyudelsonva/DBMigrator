using System;

namespace DotNetMigrations.Core.Migrations
{
    public class LocalTimestampVersion : IVersionStrategy
    {
        public long GetNewVersionNumber(IMigrationDirectory migrationDirectory)
        {
            var v = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
            return v;
        }
    }
}