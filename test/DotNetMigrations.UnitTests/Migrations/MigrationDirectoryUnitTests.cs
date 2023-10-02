using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNetMigrations.Core;
using DotNetMigrations.Core.Migrations;
using DotNetMigrations.UnitTests.Mocks;
using DotNetMigrations.UnitTests.Stubs;
using NUnit.Framework;

namespace DotNetMigrations.UnitTests.Migrations
{
	[TestFixture]
	public class MigrationDirectoryUnitTests
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			_configManager = new InMemoryConfigurationManager();
			_subject = new MigrationDirectory(_configManager, Path.GetTempPath());
		}

		#endregion

		private MigrationDirectory _subject;
		private InMemoryConfigurationManager _configManager;

		[Test]
		public void CreateBlankScript_file_name_should_include_migration_name_with_invalid_chars_replaced_with_a_dash()
		{
			//  arrange
			_configManager.AppSettings[AppSettingKeys.VersionStrategy] = VersionStrategyFactory.UtcTime;

			char[] invalidChars = Path.GetInvalidFileNameChars();
			string migrationName = "my first script";
			foreach (char c in invalidChars)
			{
				migrationName += c;
			}
			int totalInvalidCharsInName = invalidChars.Length;

			//  act
			string path = _subject.CreateBlankScript(migrationName, "CAD");

			using (DisposableFile file = DisposableFile.Watch(path))
			{
				//  assert
				int countOfDashes = file.NameWithoutExtension.Count(x => x == '-');
				Assert.AreEqual(countOfDashes, totalInvalidCharsInName);
			}
		}

		[Test]
		public void
				CreateBlankScript_file_name_should_include_migration_name_with_whitespace_replaced_with_an_underscore()
		{
			//  arrange
			_configManager.AppSettings[AppSettingKeys.VersionStrategy] = VersionStrategyFactory.UtcTime;
			const string migrationName = "my first     script";

			//  act
			string path = _subject.CreateBlankScript(migrationName, "CAD");

			using (DisposableFile file = DisposableFile.Watch(path))
			{
				//  assert
				string nameMinusVersion = file.NameWithoutExtension.Substring(file.NameWithoutExtension.IndexOf('_') + 1);
				const string expectedName = "my_first_script";
				Assert.AreEqual(expectedName, nameMinusVersion);
			}
		}

		[Test]
		public void CreateBlankScript_file_name_should_include_migration_name_with_whitespace_trimmed()
		{
			//  arrange
			_configManager.AppSettings[AppSettingKeys.VersionStrategy] = VersionStrategyFactory.UtcTime;
			const string migrationName = "    my first script    ";

			//  act
			string path = _subject.CreateBlankScript(migrationName, "CAD");

			using (DisposableFile file = DisposableFile.Watch(path))
			{
				//  assert
				string nameMinusVersion = file.NameWithoutExtension.Substring(file.NameWithoutExtension.IndexOf('_') + 1);
				const string expectedName = "my_first_script";
				Assert.AreEqual(expectedName, nameMinusVersion);
			}
		}

		[Test]
		public void CreateBlankScript_file_name_should_start_with_version_number_followed_by_an_underscore()
		{
			//  arrange
			_configManager.AppSettings[AppSettingKeys.VersionStrategy] = VersionStrategyFactory.UtcTime;
			const string migrationName = "my first script";

			//  act
			string path = _subject.CreateBlankScript(migrationName, "CAD");

			using (DisposableFile file = DisposableFile.Watch(path))
			{
				//  assert
				string[] parts = file.NameWithoutExtension.Split('_');
				long version;
				if (!long.TryParse(parts[0], out version))
				{
					Assert.Fail("File name didn't start with a version number.");
				}
			}
		}

		[Test]
		public void CreateBlankScript_file_should_contain_template_tags_in_correct_order()
		{
			//  arrange
			_configManager.AppSettings[AppSettingKeys.VersionStrategy] = VersionStrategyFactory.UtcTime;
			const string migrationName = "my first script";

			const string setupStartTag = "BEGIN_SETUP:";
			const string setupEndTag = "END_SETUP:";
			const string teardownStartTag = "BEGIN_TEARDOWN:";
			const string teardownEndTag = "END_TEARDOWN:";

			//  act
			string path = _subject.CreateBlankScript(migrationName, "CAD");

			using (DisposableFile file = DisposableFile.Watch(path))
			{
				//  assert
				string contents = File.ReadAllText(file.FullName);
				int index1 = contents.IndexOf(setupStartTag, StringComparison.Ordinal);
				int index2 = contents.IndexOf(setupEndTag, StringComparison.Ordinal);
				int index3 = contents.IndexOf(teardownStartTag, StringComparison.Ordinal);
				int index4 = contents.IndexOf(teardownEndTag, StringComparison.Ordinal);

				Assert.IsTrue(index1 != -1 && index1 < index2);
				Assert.IsTrue(index2 != -1 && index2 < index3);
				Assert.IsTrue(index3 != -1 && index3 < index4);
				Assert.IsTrue(index4 != -1);
			}
		}

		[Test]
		public void CreateBlankScript_should_create_file()
		{
			//  arrange
			_configManager.AppSettings[AppSettingKeys.VersionStrategy] = VersionStrategyFactory.UtcTime;
			const string migrationName = "my first script";

			//  act
			string path = _subject.CreateBlankScript(migrationName, "CAD");

			using (DisposableFile.Watch(path))
			{
				//  assert
				Assert.IsTrue(File.Exists(path));
			}
		}

		[Test]
		public void GetPath_should_create_path_if_it_doesnt_exist()
		{
			//  arrange
			string guid = Guid.NewGuid().ToString();
			string path = Path.Combine(Path.GetTempPath(), guid);
			using (DisposableDirectory.Watch(path))
			{

				//  act
				_subject.GetPath(guid, null);

				//  assert
				bool pathExists = Directory.Exists(path);
				Assert.IsTrue(pathExists);
			}
		}

		[Test]
		[Ignore("Logger is currently not configured, needs reimplemented")]
		public void GetPath_should_log_warning_if_migrateFolder_appSetting_isnt_found()
		{
			//  arrange
			var logger = new MockLog1();

			//  act
			string path = _subject.GetPath(null, logger);
			using (DisposableDirectory.Watch(path))
			{
				//  assert
				Assert.IsTrue(logger.Output.StartsWith("WARNING"));
			}
		}

		[Test]
		[Ignore("Default Paths determination has changed")]
		public void GetPath_should_return_default_path_if_migrateFolder_appSetting_isnt_found()
		{
			//  act
			string path = _subject.GetPath(null, null);
			using (DisposableDirectory.Watch(path))
			{
				//  assert
				bool pathExists = Directory.Exists(path);
				Assert.IsTrue(pathExists);
			}
		}

		[Test]
		public void GetScripts_should_return_all_SQL_scripts_in_folder()
		{
			//  arrange
			string guid = Guid.NewGuid().ToString();
			string path = Path.Combine(Path.GetTempPath(), guid);
			using (DisposableDirectory.Create(path))
			{
				FileHelper.Touch(Path.Combine(path, "1_script_one.sql"));
				FileHelper.Touch(Path.Combine(path, "2_script_two.sql"));

				//  act
				IEnumerable<IMigrationScriptFile> files = _subject.GetScripts(guid);

				//  assert
				const int expectedCount = 2;
				Assert.AreEqual(expectedCount, files.Count());
			}
		}

		[Test]
		public void GetScripts_should_return_an_empty_enumerable_if_folder_is_empty()
		{
			//  arrange
			string guid = Guid.NewGuid().ToString();
			string pathToEmptyDir = Path.Combine(Path.GetTempPath(), guid);
			using (DisposableDirectory emptyDir = DisposableDirectory.Create(pathToEmptyDir))
			{
				//  act
				IEnumerable<IMigrationScriptFile> files = _subject.GetScripts(guid);

				//  assert
				const int expectedCount = 0;
				Assert.AreEqual(expectedCount, files.Count());
			}
		}

		[Test]
		public void GetScripts_should_return_SQL_scripts_sorted_by_version()
		{
			//  arrange
			string guid = Guid.NewGuid().ToString();
			string path = Path.Combine(Path.GetTempPath(), guid);
			using (DisposableDirectory.Create(path))
			{

				FileHelper.Touch(Path.Combine(path, "1_script_one.sql"));
				FileHelper.Touch(Path.Combine(path, "2_script_two.sql"));
				FileHelper.Touch(Path.Combine(path, "10_script_ten.sql"));

				//  act
				IEnumerable<IMigrationScriptFile> files = _subject.GetScripts((string)guid);

				//  assert
				Assert.IsTrue(files.First().Version == 1);
				Assert.IsTrue(files.Last().Version == 10);
			}
		}
	}
}