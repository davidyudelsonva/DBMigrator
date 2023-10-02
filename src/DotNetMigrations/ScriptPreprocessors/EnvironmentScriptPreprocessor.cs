using System;
using DotNetMigrations.Core.BaseClasses;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DotNetMigrations.Core;

namespace DotNetMigrations.ScriptPreprocessors
{
	public class EnvironmentScriptPreprocessor : ScriptPreprocessorBase
	{
		public override string Name => "environment";

		public EnvironmentScriptPreprocessor(IConfigurationManager configurationManager) :
			base(configurationManager,
				ReplacementMode.Regex,
				new Dictionary<string, string>(),
				RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace
			)
		{
			SetupReplacements();
		}


		public EnvironmentScriptPreprocessor() :
			base(ReplacementMode.Regex,
				new Dictionary<string, string>(),
				RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline |RegexOptions.IgnorePatternWhitespace
			)
		{
			SetupReplacements();
		}

		private void SetupReplacements()
		{
			var environments = new List<string>
				{"CentevaDev", "DevAlpha", "DevBeta", "Local", "Test", "iFAMS", "Train", "Preprod", "Production"};

			var currentEnv = _configManager.AppSettings[AppSettingKeys.Environment] ?? "Local";

			foreach (var env in environments)
			{
				_replacements.Add(
					@"^\s*" + Regex.Escape($"BEGIN_{env}_ENV:") + @"\s*$  (.*?)  ^\s*" + Regex.Escape($"END_{env}_ENV:") + @"\s*$",
					currentEnv.Equals(env, StringComparison.OrdinalIgnoreCase) ? "$&" : ""
				);
			}

			//Remove environment tags after using them
			_replacements.Add(Regex.Escape($"BEGIN_{currentEnv}_ENV:"), "");
			_replacements.Add(Regex.Escape($"END_{currentEnv}_ENV:"), "");
		}

	}
}
