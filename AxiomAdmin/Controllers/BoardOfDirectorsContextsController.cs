using AxiomAdmin.Data;
using AxiomAdmin.ViewModel;
using DMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AxiomAdmin.Controllers
{
    public class BoardOfDirectorsContextsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TeamContextsController> _logger;

        public BoardOfDirectorsContextsController(ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment hostingEnvironment, ILogger<TeamContextsController> logger)
        {
            _context = context;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        // GET: BoardOfDirectorsContexts
        public async Task<IActionResult> Index()
        {
            var directorsContextList = await _context.BoardOfDirectorsContexts.ToListAsync();

            var viewModel = new DirectorsContextsViewModel
            {
                ListBoardOfDirectors = directorsContextList,
            };
            return View(viewModel);
        }

        // GET: BoardOfDirectorsContexts/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.BoardOfDirectorsContexts == null)
            {
                return NotFound();
            }

            var boardOfDirectorsContext = await _context.BoardOfDirectorsContexts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boardOfDirectorsContext == null)
            {
                return NotFound();
            }

            return View(boardOfDirectorsContext);
        }

        // GET: BoardOfDirectorsContexts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BoardOfDirectorsContexts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DirectorsContextsViewModel boardOfDirectorsContext, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return View(boardOfDirectorsContext);
            }

            try
            {
                // Get the root directory of the NAxiomindexings project
                string rootDirectory = Path.Combine(_hostingEnvironment.ContentRootPath, "../NAxiomindexings");

                // Define the path to the folder where you want to save the image in the NAxiomindexings project
                string uploadFolder = Path.Combine(rootDirectory, "wwwroot/assets/img/directors");

                // Create the folder if it doesn't exist
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                // Generate a unique filename to avoid overwriting existing files
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

                // Combine the upload folder and the unique filename to get the full path
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Save the file path in the database
                boardOfDirectorsContext.BoardOfDirectors.ImageUrl = uniqueFileName;
                boardOfDirectorsContext.BoardOfDirectors.Id = Guid.NewGuid();
                _context.Add(boardOfDirectorsContext.BoardOfDirectors);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "An error occurred while uploading the file.");

                // Show an error message to the user
                ModelState.AddModelError(string.Empty, "An error occurred while uploading the file. Please try again.");
                return View(boardOfDirectorsContext);
            }
        }

        // GET: BoardOfDirectorsContexts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.BoardOfDirectorsContexts == null)
            {
                return NotFound();
            }

            var boardOfDirectorsContext = await _context.BoardOfDirectorsContexts.FindAsync(id);
            if (boardOfDirectorsContext == null)
            {
                return NotFound();
            }
            return View(boardOfDirectorsContext);
        }

        // POST: BoardOfDirectorsContexts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FullName,ImageUrl,Position,Date")] BoardOfDirectorsContext boardOfDirectorsContext)
        {
            if (id != boardOfDirectorsContext.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boardOfDirectorsContext);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoardOfDirectorsContextExists(boardOfDirectorsContext.Id))
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
            return View(boardOfDirectorsContext);
        }

        // GET: BoardOfDirectorsContexts/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.BoardOfDirectorsContexts == null)
            {
                return NotFound();
            }

            var boardOfDirectorsContext = await _context.BoardOfDirectorsContexts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boardOfDirectorsContext == null)
            {
                return NotFound();
            }

            return View(boardOfDirectorsContext);
        }

        // POST: BoardOfDirectorsContexts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.BoardOfDirectorsContexts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BoardOfDirectorsContexts'  is null.");
            }
            var boardOfDirectorsContext = await _context.BoardOfDirectorsContexts.FindAsync(id);
            if (boardOfDirectorsContext != null)
            {
                _context.BoardOfDirectorsContexts.Remove(boardOfDirectorsContext);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoardOfDirectorsContextExists(Guid id)
        {
            return (_context.BoardOfDirectorsContexts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}