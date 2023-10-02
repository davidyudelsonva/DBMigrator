using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMigrations.Core
{
	[Serializable]
	public class SqlExecutionException : Exception
	{
		public string AttemptedSQL { get; protected set; }

		public SqlExecutionException(string attemptedSQL, Exception ex ) : base("Error occurred while attempting to run sql batch:\n" + attemptedSQL,ex)
		{
			AttemptedSQL = attemptedSQL;
		}

	}
}
