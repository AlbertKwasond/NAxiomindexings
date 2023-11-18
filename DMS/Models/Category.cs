using System.ComponentModel.DataAnnotations;

namespace DMS.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        public ICollection<Journals> Journals { get; set; }
    }
}