using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Models
{
    public class BlogPost
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Please enter the heading")]
        [StringLength(100, MinimumLength = 5)]
        public string Heading { get; set; }

        [Required(ErrorMessage = "Please enter the context")]
        public string Context { get; set; }

        public string? FeaturedImageUrl { get; set; }

        [DisplayFormat(DataFormatString = "{0:d MMM, yyyy}")]
        public DateTime PublishedDate { get; set; } = DateTime.Now;

        public string Author { get; set; } = "Unknown";

        public bool Visible { get; set; }

        [NotMapped]
        [Display(Name = "Choose the cover photo of your blog")]
        public IFormFile? CoverPhoto { get; set; }

        public ICollection<Tag>? Tags { get; set; }
    }
}