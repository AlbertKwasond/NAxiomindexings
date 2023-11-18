using DMS.Models;

namespace AxiomAdmin.ViewModel
{
    public class CategoryViewModel
    {
        public Category GetCategory { get; set; }
        public IEnumerable<Category> ListCategories { get; set; }
    }
}