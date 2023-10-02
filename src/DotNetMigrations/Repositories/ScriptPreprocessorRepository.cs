using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using DotNetMigrations.Core;
using DotNetMigrations.ScriptPreprocessors;

namespace DotNetMigrations.Repositories
{
	public class ScriptPreprocessorRepository
	{
		private readonly AggregateCatalog catalog;
		private readonly CompositionContainer container;

		/// <summary>
		/// A collection of the preprocessors in the system.
		/// </summary>
		[ImportMany("ScriptPreprocessors", typeof(IScriptPreprocessor))]
		public IList<IScriptPreprocessor> ScriptPreprocessors { get; set; }

		/// <summary>
		/// Instantiates a new instance of the ScriptPreprocessorRepository class.
		/// </summary>
		public ScriptPreprocessorRepository()
		{
			var pluginDirectory = ConfigurationManager.AppSettings[AppSettingKeys.PluginFolder];

			ScriptPreprocessors = new List<IScriptPreprocessor>();

			catalog = new AggregateCatalog();
			catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetCallingAssembly()));

			if (Directory.Exists(pluginDirectory))
			{
				catalog.Catalogs.Add(new DirectoryCatalog(pluginDirectory));
			}

			container = new CompositionContainer(catalog);
			container.ComposeParts(this);
		}

		/// <summary>
		/// Return Script Preprocessors that are defined as active in the config file.
		/// </summary>
		/// <returns></returns>
		public List<IScriptPreprocessor> GetActiveScriptPreprocessors()
		{
			var activepreprocessors = new List<IScriptPreprocessor>();

			activepreprocessors.Add(ScriptPreprocessors.FirstOrDefault(sm => sm.Name == "environment"));

			var configValue = ConfigurationManager.AppSettings[AppSettingKeys.ActiveScriptPreprocessors];

			if (string.IsNullOrEmpty(configValue))
				return activepreprocessors;

			var names = configValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => s.ToLower())
				.ToList();

			return ScriptPreprocessors.Where(sm => names.Contains(sm.Name.ToLower())).ToList();
			
		}


		/// <summary>
		/// Retrieves the command based on the command name.
		/// </summary>
		/// <param name="commandName">The name of the command to retrieve.</param>
		/// <returns>An instance of the command or null if not found.</returns>
		public IScriptPreprocessor GetScriptPreprocessor(string scriptPreprocessorName)
		{
			var scriptPreprocessor = (from c in ScriptPreprocessors
											where c.Name.ToLowerInvariant() == scriptPreprocessorName.ToLowerInvariant()
											select c).FirstOrDefault();

			return scriptPreprocessor;
		}
	}
}
