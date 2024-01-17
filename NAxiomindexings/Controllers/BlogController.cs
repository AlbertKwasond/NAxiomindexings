using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NAxiomindexings.Data;
using NAxiomindexings.ViewModel;

namespace NAxiomindexings.Controllers
{
	public class BlogController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment _hostingEnvironment;

		public BlogController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
		{
			_context = context;
			_hostingEnvironment = hostingEnvironment;
		}

		// GET: BlogController
		public async Task<ActionResult> IndexAsync()
		{
			return _context.BlogPosts != null ?
						View(await _context.BlogPosts.ToListAsync()) :
						Problem("Entity set 'ApplicationDbContext.BlogPost'  is null.");
		}

		// GET: BlogController/Details/5
		public async Task<IActionResult> Details(Guid Id)
		{
			var blogDetails = await _context.BlogPosts
				.Where(x => x.Id == Id && x.Visible == true)
				.FirstOrDefaultAsync();

			var listBlog = await _context.BlogPosts
				.Where(j => j.Id != blogDetails.Id && j.Visible == true) // Exclude the selected blog posts
				.Take(4) // Take only the top 4 blog posts
				.ToListAsync();

			var viewModel = new BlogPostViewModel
			{
				GetBlogPost = blogDetails,
				ListBlogPosts = listBlog
			};

			return View(viewModel);
		}
	}
}