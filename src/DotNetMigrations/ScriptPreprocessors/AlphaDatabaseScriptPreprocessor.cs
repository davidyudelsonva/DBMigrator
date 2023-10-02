using DotNetMigrations.Core.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMigrations.ScriptPreprocessors
{
	public class AlphaDatabaseScriptPreprocessor : ScriptPreprocessorBase
	{
		public override string Name { get { return "alphaDatabase";  } }

		public AlphaDatabaseScriptPreprocessor() :
			base( ReplacementMode.Regex,
				new Dictionary<string, string> {
					{ @"(\s?USE\s+)\[?(Epic|Cad|ReportData)\]?", "$1[$2_Alpha]" },
					{ @"((TABLE|INSERT|INTO|FOR|FROM|UPDATE|DELETE|JOIN|sp_rename)\s+'?)\[?(Cad|Epic|ReportData)\]?(\.[\w,\[,\]]+\.[\w,\[,\]]+)", "$1[$3_Alpha]$4" }
				}
			)
			{ }
	}
}
