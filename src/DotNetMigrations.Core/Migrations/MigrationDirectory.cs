using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotNetMigrations.Core.Migrations
{
	public class MigrationDirectory : IMigrationDirectory
	{
		private const string DefaultMigrationScriptPath = @".\migrate\";
		private const string ScriptFileNamePattern = "*.sql";
		private readonly IConfigurationManager _configurationManager = new ConfigurationManagerWrapper();
		public string _path;

		public MigrationDirectory()
		{
			_path = _configurationManager.AppSettings[AppSettingKeys.MigrateFolder];
			//TODO - figure out how to handle logging here.
			//if (string.IsNullOrEmpty(_path))
			//{
			//	if (log != null)
			//	{
			//		log.WriteWarning(
			//				"The " + AppSettingKeys.MigrateFolder + " setting was not present in the configuration file so the default " +
			//				DefaultMigrationScriptPath + " folder will be used instead.");
			//	}
			//	path = DefaultMigrationScriptPath;
			//}

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path">Base folder for this migration directory</param>
		public MigrationDirectory(string path) 
		{
			_path = Path.Combine(_configurationManager.AppSettings[AppSettingKeys.MigrateFolder], path);
		}
    
		public MigrationDirectory(IConfigurationManager configurationManager, string path)
		{
			_configurationManager = configurationManager;
			_path = path;
		}

		/// <summary>
		/// Verify the path exists and creates it if it's missing.
		/// </summary>
		/// <param name="path">The path to verify.</param>
		private static void VerifyAndCreatePath(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		/// <summary>
		/// Returns the migration script path from the
		/// config file (if available) or the default path.
		/// </summary>
		public string GetPath(string subfolder, ILogger log)
		{
			string path = _path;
			if (!string.IsNullOrEmpty(subfolder))
				path = Path.Combine(_path, subfolder);

			VerifyAndCreatePath(path);

			return path;
		}

		/// <summary>
		/// Returns a list of all the migration script file paths
		/// sorted by version number (ascending).
		/// </summary>
		public IEnumerable<IMigrationScriptFile> GetScripts(string subfolder)
		{
			string[] files = Directory.GetFiles(GetPath(subfolder, null), ScriptFileNamePattern, SearchOption.AllDirectories);

			return files.Select(x => (IMigrationScriptFile)new MigrationScriptFile(x)).OrderBy(x => x.Version);
		}

		/// <summary>
		/// Returns a list of all the migration script file paths
		/// sorted by version number (ascending).
		/// </summary>
		public IEnumerable<IMigrationScriptFile> GetScripts(List<long> scriptsToUse)
		{
			string[] files = Directory.GetFiles(GetPath(null, null), ScriptFileNamePattern, SearchOption.AllDirectories);

			return files.
				Select(x => (IMigrationScriptFile)new MigrationScriptFile(x))
				.Where(x => scriptsToUse == null || scriptsToUse.Contains(x.Version))
				.OrderBy(x => x.Version);
		}
		/// <summary>
		/// Creates a blank migration script with the given name.
		/// </summary>
		/// <param name="migrationName">name of the migration script</param>
		/// <returns>The path to the new migration script.</returns>
		public string CreateBlankScript(string migrationName, string subfolder)
		{
			long version = GetNewVersionNumber();
			var path = GetPath(subfolder,null);

			path = Path.Combine(path, version + "_" + SanitizeMigrationName(migrationName) + ".sql");



			var contents = new MigrationScriptContents($"", $"");

			var file = new MigrationScriptFile(path);
			file.Write(contents);

			return path;
		}

		/// <summary>
		/// Returns a file name friendly version of the given
		/// migration name.
		/// </summary>
		private static string SanitizeMigrationName(string migrationName)
		{
			const char invalidCharReplacement = '-';

			//  replace the invalid characters
			var invalidChars = Path.GetInvalidFileNameChars();
			foreach (var c in invalidChars)
			{
				migrationName = migrationName.Replace(c, invalidCharReplacement);
			}

			//  trim whitespace
			migrationName = migrationName.Trim();

			//  replace whitespace with an underscore
			const string whitespaceReplacement = "_";
			migrationName = Regex.Replace(migrationName, @"\s+", whitespaceReplacement, RegexOptions.Compiled);

			return migrationName;
		}

		/// <summary>
		/// Generates a new version number for assignment.
		/// </summary>
		private long GetNewVersionNumber()
		{
			var factory = new VersionStrategyFactory(_configurationManager);
			IVersionStrategy strategy = factory.GetStrategy();
			long version = strategy.GetNewVersionNumber(this);
			return version;
		}
	}
}