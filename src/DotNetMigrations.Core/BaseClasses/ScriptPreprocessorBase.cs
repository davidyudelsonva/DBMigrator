using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DotNetMigrations.Core.BaseClasses
{
	public enum ReplacementMode {
		String,
		Regex
	};

	public abstract class ScriptPreprocessorBase : IScriptPreprocessor
	{
		protected ReplacementMode _replacementMode;
		protected Dictionary<string, string> _replacements;
		protected RegexOptions _options;
		protected readonly IConfigurationManager _configManager;

		protected ScriptPreprocessorBase(ReplacementMode mode, 
			Dictionary<string,string> reps, 
			RegexOptions options = RegexOptions.IgnoreCase)
		{
			_configManager = new ConfigurationManagerWrapper();
			_replacements = reps;
			_replacementMode = mode;
			_options = options;
		}

		protected ScriptPreprocessorBase(IConfigurationManager configManager,
			ReplacementMode mode,
			Dictionary<string, string> reps,
			RegexOptions options = RegexOptions.IgnoreCase)
		{
			_configManager = configManager;
			_replacements = reps;
			_replacementMode = mode;
			_options = options;
		}


		public abstract string Name {	get; }

		public string ModifyScript(string script)
		{
			var scriptResult = script;
			foreach(var rep in _replacements)
			{
				switch (_replacementMode)
				{
					case ReplacementMode.String:
						scriptResult = scriptResult.Replace(rep.Key, rep.Value);
						break;
					case ReplacementMode.Regex:
						scriptResult = Regex.Replace(scriptResult, rep.Key, rep.Value,_options);
						break;
				}
			}

			return scriptResult;
		}
	}
}
