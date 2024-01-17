using DMS.Models;

namespace NAxiomindexings.ViewModel
{
	public class HomeViewModel
	{
		public HomeContext GetContext { get; set; }
		public IEnumerable<HomeContext> ListHomeContexts { get; set; }
		public IEnumerable<BlogPost> ListBlogPosts { get; set; }
	}
}