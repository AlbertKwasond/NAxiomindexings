using DMS.Models;

namespace NAxiomindexings.ViewModel
{
	public class BlogPostViewModel
	{
		public BlogPost GetBlogPost { get; set; }
		public IEnumerable<BlogPost> ListBlogPosts { get; set; }
	}
}