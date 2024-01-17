using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Models
{
	public class Journals
	{
		// Unique identifier for the journal
		public Guid Id { get; set; } = Guid.NewGuid();

		// Title of the journal
		[Required(ErrorMessage = "Please enter the title of your book")]
		[StringLength(100, MinimumLength = 5)]
		public string Title { get; set; }

		// ID of the author of the journal
		[Required(ErrorMessage = "Please enter the author name")]
		public Guid? AuthorId { get; set; }

		// Navigation property to the author of the journal
		public Author? Authors { get; set; }

		// Description of the journal
		[StringLength(500)]
		public string Description { get; set; }

		// ID of the category to which the journal belongs
		[Display(Name = "Category")]
		public Guid CategoryId { get; set; }

		// Navigation property to the category of the journal
		public Category Categories { get; set; }

		// Total number of pages in the journal
		[Required(ErrorMessage = "Please enter the total pages")]
		[Display(Name = "Total pages of book")]
		public int TotalPages { get; set; }

		// URL of the cover image of the journal
		public string CoverImageUrl { get; set; }

		// URL of the PDF file of the journal
		public string BookPdfUrl { get; set; }

		// File for the cover photo of the journal (used for uploading)
		[NotMapped]
		[Display(Name = "Choose the cover photo of your journals")]
		public IFormFile CoverPhoto { get; set; }

		// File for the PDF of the journal (used for uploading)
		[NotMapped]
		[Display(Name = "Upload your book in pdf format")]
		[Required]
		public IFormFile BookPdf { get; set; }

		// Date when the journal was created
		[DisplayFormat(DataFormatString = "{0:d MMM, yyyy}")]
		public DateTime CreatedOn { get; set; }

		// Date when the journal was last updated
		public DateTime LastUpdatedOn { get; set; }

		// Status of the journal (e.g. published, draft)
		public string Status { get; set; }
	}
}