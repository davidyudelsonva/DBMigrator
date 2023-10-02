using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMigrations.Core.Data
{
	public class SchemaMigration
	{
		public int Id { get; set; }
		public long Version { get; set; }
		public string Name { get; set; }
		public string Author { get; set; }
		public string Ticket_Number { get; set; }
		public string Description { get; set; }
		public DateTime? CreatedDate { get; set; }
		public DateTime? AppliedDate { get; set; }
		public string AppliedByUser { get; set; }

		public string[] ToStringArray()
		{
			return new string[] {
				Version.ToString(),
				Name,
				Author,
				Ticket_Number,
				CreatedDate?.ToShortDateString(),
				AppliedDate?.ToShortDateString(),
				AppliedByUser
			};
		}
	}
}
