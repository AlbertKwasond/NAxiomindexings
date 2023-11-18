using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;

namespace DMS.Models
{
	public class BoardOfDirectorsContext
	{
		[Key]
		public Guid Id { get; set; } = new Guid();

		[StringLength(100)]
		[Required(ErrorMessage = "Please enter the Full Name")]
		[Display(Name = "Full Name")]
		public string? FullName { get; set; }

		public string? ImageUrl { get; set; }

		[StringLength(150)]
		public string? Position { get; set; }

		[NotMapped]
		[DisplayName("Upload File")]
		public IFormFile? DirectorsPhoto { get; set; }

		public DateTime Date { get; set; } = new DateTime();
	}
}