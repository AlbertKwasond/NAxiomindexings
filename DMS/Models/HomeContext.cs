using System.ComponentModel.DataAnnotations;

namespace DMS.Models
{
	public class HomeContext
	{
		[Key]
		public Guid Id { get; set; } = new Guid();

		[Required]
		[StringLength(50)]
		public string? Title { get; set; }
        [Required]
        [StringLength(50)]
		public string? Name { get; set; }

        [Required]
        [StringLength(120)]
		public string? Messages { get; set; }

		public DateTime Date { get; set; } = new DateTime();
	}
}