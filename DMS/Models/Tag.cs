using System.ComponentModel.DataAnnotations;

namespace DMS.Models
{
    public class Tag
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public ICollection<BlogPost> blogPosts { get; set; }
    }
}