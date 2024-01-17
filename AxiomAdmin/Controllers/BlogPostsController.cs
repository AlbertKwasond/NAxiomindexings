using AxiomAdmin.Data;
using DMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AxiomAdmin.Controllers
{
    [Authorize(Roles = "Administrator,Super-Administration,Blogger")]
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly string hostLinktoken;

        public BlogPostsController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            hostLinktoken = _configuration["HostLink:URLlink"];
        }

        // GET: BlogPosts
        public async Task<IActionResult> Index()
        {
            return _context.BlogPosts != null ?
                        View(await _context.BlogPosts.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.BlogPost'  is null.");
        }

        // GET: BlogPosts/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(BlogPost blogPost)
        {
            if (ModelState.IsValid)
            {
                // Get the root directory of the NAxiomindexings project
                string rootDirectory = Path.Combine(_hostingEnvironment.ContentRootPath, hostLinktoken);

                // Define the path to the folder where you want to save the image in the NAxiomindexings project
                string uploadFolder = Path.Combine(rootDirectory, "wwwroot/assets/img/blog");

                // Create the folder if it doesn't exist
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                // Generate a unique filename to avoid overwriting existing files
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + blogPost.CoverPhoto.FileName;

                // Combine the upload folder and the unique filename to get the full path
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await blogPost.CoverPhoto.CopyToAsync(fileStream);
                }

                // Save the file path in the database
                blogPost.FeaturedImageUrl = uniqueFileName;
                blogPost.Id = Guid.NewGuid();
                _context.Add(blogPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, BlogPost blogPost)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (null == blogPost.CoverPhoto)
                    {
                        var blog = new BlogPost
                        {
                            Id = blogPost.Id,
                            Heading = blogPost.Heading,
                            Context = blogPost.Context,
                            Author = blogPost.Author,
                            FeaturedImageUrl = blogPost.FeaturedImageUrl,
                            Visible = blogPost.Visible,
                        };
                        _context.Update(blog);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        // Delete the old image file from the server's file system (if it exists)
                        if (!string.IsNullOrEmpty(blogPost.FeaturedImageUrl))
                        {
                            var oldFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, blogPost.FeaturedImageUrl);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                        // Get the root directory of the NAxiomindexings project
                        string rootDirectory = Path.Combine(_hostingEnvironment.ContentRootPath, hostLinktoken);
                        // Save the new image file to the server's file system
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + blogPost.CoverPhoto.FileName;
                        string uploadFolder = Path.Combine(rootDirectory, "wwwroot/assets/img/blog");
                        string newFilePath = Path.Combine(uploadFolder, uniqueFileName);

                        using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                        {
                            await blogPost.CoverPhoto.CopyToAsync(fileStream);
                        }

                        // Update the record in the database
                        var updateblog = new BlogPost
                        {
                            Id = blogPost.Id,
                            Heading = blogPost.Heading,
                            Context = blogPost.Context,
                            Author = blogPost.Author,
                            FeaturedImageUrl = uniqueFileName,
                            Visible = blogPost.Visible,
                        };
                        _context.Update(updateblog);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5

        public async Task<IActionResult> Delete(Guid id)
        {
            if (_context.BlogPosts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BlogPost'  is null.");
            }
            var blogPost = await _context.BlogPosts.FindAsync(id);
            // Delete the old image file from the server's file system (if it exists)
            if (!string.IsNullOrEmpty(blogPost.FeaturedImageUrl))
            {
                var rootDirectory = Path.Combine(_hostingEnvironment.ContentRootPath, hostLinktoken);
                var oldFilePath = Path.Combine(rootDirectory, "wwwroot/assets/img/blog", blogPost.FeaturedImageUrl);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
            if (blogPost != null)
            {
                _context.BlogPosts.Remove(blogPost);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(Guid id)
        {
            return (_context.BlogPosts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}