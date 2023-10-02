using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMigrations.Core
{
	[InheritedExport("ScriptPreprocessors", typeof(IScriptPreprocessor))]
	public interface IScriptPreprocessor
	{
		string Name { get; }

		string ModifyScript(string script);
	}
}
