using System.ComponentModel.DataAnnotations;

namespace DMS.Models
{
	public class dbConcurrencyStamp
	{
		public string Id { get; set; }

		[MaxLength(1000)]
		public string ConcurrencyStamp { get; set; }
	}
}