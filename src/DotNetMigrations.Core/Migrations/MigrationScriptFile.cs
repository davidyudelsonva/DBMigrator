using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNetMigrations.Core.Migrations
{
	public class MigrationScriptFile : IMigrationScriptFile
	{
		private const string SetupStartTag = "BEGIN_SETUP:";
		private const string SetupEndTag = "END_SETUP:";
		private const string TeardownStartTag = "BEGIN_TEARDOWN:";
		private const string TeardownEndTag = "END_TEARDOWN:";

		private static readonly Regex SetupRegex;
		private static readonly Regex TeardownRegex;

		private static readonly List<MigrationScriptVariable> _migrationVariables;

		static MigrationScriptFile()
		{
			_migrationVariables = new List<MigrationScriptVariable>()
			{
				new MigrationScriptVariable("Author",(f,s) => f.Author = s),
				new MigrationScriptVariable("TicketNumber",(f,s) => f.Ticket_Number = s),
				new MigrationScriptVariable("Description",(f,s) => f.Description = s),
				new MigrationScriptVariable("CreatedDate",(f,s) => f.CreatedDate = s, DateTime.Now.ToShortDateString())
			};

			SetupRegex =
					new Regex(
							@"^\s*" + Regex.Escape(SetupStartTag) + @"\s*$  (.*)  ^\s*" + Regex.Escape(SetupEndTag) + @"\s*$",
							RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline |
							RegexOptions.IgnorePatternWhitespace |
							RegexOptions.Compiled);

			TeardownRegex =
					new Regex(
							@"^\s*" + Regex.Escape(TeardownStartTag) + @"\s*$  (.*)  ^\s*" + Regex.Escape(TeardownEndTag) +
							@"\s*$",
							RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline |
							RegexOptions.IgnorePatternWhitespace |
							RegexOptions.Compiled);
		}

		public MigrationScriptFile(string filePath)
		{
			FilePath = filePath;
			ParseVersion();
		}

		#region IMigrationScriptFile Members

		public string FilePath { get; private set; }
		public long Version { get; private set; }
		public string Name { get; private set; }
		public string Author { get; private set; }
		public string Ticket_Number { get; set; }
		public string Description { get; set; }
		public string CreatedDate { get; set; }
		public string Database { get; private set; }

		/// <summary>
		/// Parses and returns the contents of the migration script file.
		/// </summary>
		public MigrationScriptContents Read()
		{
			string setupScript = string.Empty;
			string teardownScript = string.Empty;

			string allLines = File.ReadAllText(FilePath);


			foreach(var variable in _migrationVariables)
			{
				var match = variable.Regex.Match(allLines);
				if (match.Success)
				{
                    variable.UpdateValue(this, match.Groups[1].Value.Trim());
				}

			}

			Match setupMatch = SetupRegex.Match(allLines);
			if (setupMatch.Success)
			{
				setupScript = setupMatch.Groups[1].Value;
			}

			// don't include the setup portion of the script
			// when matching the teardown
			Match teardownMatch = TeardownRegex.Match(allLines, setupMatch.Length);
			if (teardownMatch.Success)
			{
				teardownScript = teardownMatch.Groups[1].Value;
			}

			if (!setupMatch.Success && !teardownMatch.Success)
			{
				// assume entire file is the setup and there is no teardown
				setupScript = allLines;
			}

			return new MigrationScriptContents(setupScript, teardownScript);
		}

		/// <summary>
		/// Writes the given contents into the migration script file.
		/// </summary>
		public void Write(MigrationScriptContents contents)
		{
			var sb = new StringBuilder();
			foreach(var variables in _migrationVariables)
			{
				sb.AppendLine(variables.Comment + " " + variables.Value);
			}
			sb.AppendLine();
			sb.AppendLine(SetupStartTag);
			sb.AppendLine();
			sb.AppendLine(contents.Setup);
			sb.AppendLine();
			sb.AppendLine(SetupEndTag);
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine(TeardownStartTag);
			sb.AppendLine();
			sb.AppendLine(contents.Teardown);
			sb.AppendLine();
			sb.Append(TeardownEndTag);

			File.WriteAllText(FilePath, sb.ToString());
		}

		#endregion

		private void ParseVersion()
		{
			Database = Directory.GetParent(FilePath).Name;

			var parts = Path.GetFileName(FilePath).Split('_');
			string sVersion = parts.FirstOrDefault();
			long v;
			long.TryParse(sVersion, out v);
			Version = v;
			
			Name = string.Join("_", parts.Skip(1)).Replace(".sql","");
		}

		public bool Equals(MigrationScriptFile other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.Version == Version;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(MigrationScriptFile)) return false;
			return Equals((MigrationScriptFile)obj);
		}

		public override int GetHashCode()
		{
			return Version.GetHashCode();
		}

		public static bool operator ==(MigrationScriptFile left, MigrationScriptFile right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(MigrationScriptFile left, MigrationScriptFile right)
		{
			return !Equals(left, right);
		}
	}

	public class MigrationScriptVariable
	{
		public string Comment { get; set; }
		public string Value { get; set; }
		public Regex Regex { get; private set; }
		public Action<MigrationScriptFile,string> UpdateValue { get; set; }

		public MigrationScriptVariable(string varName, Action<MigrationScriptFile,string> action, string value = "")
		{
			Comment = $"--{varName}:";
			Value = value;
			Regex = new Regex(Comment + "(.+)\n", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			UpdateValue = action;
		}
	}
}