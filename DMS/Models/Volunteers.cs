using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS.Models
{
    public class Volunteers
    {
        [Key]
        public Guid Id { get; set; } = new Guid();

        [StringLength(100)]
        [Required(ErrorMessage = "Please enter the Full Name")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        public string ImageUrl { get; set; }

        [NotMapped]
        [DisplayName("Upload File")]
        public IFormFile? VolunteersPhoto { get; set; }

        public DateTime Date { get; set; } = new DateTime();
    }
}