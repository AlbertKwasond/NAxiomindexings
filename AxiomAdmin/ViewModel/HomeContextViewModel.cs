using DMS.Models;

namespace AxiomAdmin.ViewModel
{
    public class HomeContextViewModel
    {
        public HomeContext HomeContexts { get; set; }
        public IEnumerable<HomeContext> homeContexts { get; set; }
    }
}