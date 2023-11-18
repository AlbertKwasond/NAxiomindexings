using AxiomAuthor.Data;
using AxiomAuthor.ViewModel;
using DMS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AxiomAuthor.Controllers
{
    public class JournalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;

        public JournalsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }

        public string? Username { get; set; }

        private async Task LoadAsync(IdentityUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
        }

        // GET: Journals
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            // Load user data asynchronously
            await LoadAsync(user);

            // Find the authors member with the provided email (Username)
            var userNameId = await _context.Authors.FirstOrDefaultAsync(x => x.Email == user.Email);
            if (userNameId != null)
            {
                ViewBag.User = userNameId.FullName;
            }
            var applicationDbContext = await _context.Journals.Include(j => j.Authors).Include(j => j.Categories).ToListAsync();

            var viewModel = new JournalsViewModel
            {
                ListJournals = applicationDbContext
            };
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");

            return View(viewModel);
        }

        // GET: Journals/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Journals == null)
            {
                return NotFound();
            }

            var journals = await _context.Journals
                .Include(j => j.Authors)
                .Include(j => j.Categories)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (journals == null)
            {
                return NotFound();
            }

            return View(journals);
        }

        // GET: Journals/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Address");
            return View();
        }

        // POST: Journals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JournalsViewModel journals)
        {
            var agentInformations = User.FindFirstValue(ClaimTypes.Email);
            var authorsInfo = await _context.Authors.FirstOrDefaultAsync(si => si.Email == agentInformations);
            // Check the file size for BookPdf
            if (journals.GetJournals.BookPdf != null && journals.GetJournals.BookPdf.Length > 0)
            {
                var maxFileSize = 20 * 1024 * 1024; // 20MB
                if (journals.GetJournals.BookPdf.Length > maxFileSize)
                {
                    ModelState.AddModelError("GetJournals.BookPdf", "PDF file size exceeds the maximum allowed limit of 20MB.");
                    return View(journals);
                }

                // Process the PDF file as before
                // Save to disk or perform other operations

                // Get the root directory of the NAxiomindexings project
                string rootDirectory = Path.Combine(_hostingEnvironment.ContentRootPath, "../NAxiomindexings");

                // Define the path to the folder where you want to save the image in the NAxiomindexings project
                string uploadCoverFolder = Path.Combine(rootDirectory, "wwwroot/assets/img/journal/cover");

                string uploadBookPdfFolder = Path.Combine(rootDirectory, "wwwroot/assets/img/journal/pdf");

                // Create the folder if it doesn't exist
                if (!Directory.Exists(uploadCoverFolder))
                {
                    Directory.CreateDirectory(uploadCoverFolder);
                }

                // Generate a unique filename to avoid overwriting existing files
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + journals.GetJournals.CoverPhoto.FileName;

                // Combine the upload folder and the unique filename to get the full path
                string filePath = Path.Combine(uploadCoverFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await journals.GetJournals.CoverPhoto.CopyToAsync(fileStream);
                }

                // Create the folder if it doesn't exist
                if (!Directory.Exists(uploadBookPdfFolder))
                {
                    Directory.CreateDirectory(uploadBookPdfFolder);
                }

                // Generate a unique filename to avoid overwriting existing files
                string uniquePdfFileName = Guid.NewGuid().ToString() + "_" + journals.GetJournals.BookPdf.FileName;

                // Combine the upload folder and the unique filename to get the full path
                string filePdfPath = Path.Combine(uploadBookPdfFolder, uniquePdfFileName);

                using (var filePdfStream = new FileStream(filePdfPath, FileMode.Create))
                {
                    await journals.GetJournals.BookPdf.CopyToAsync(filePdfStream);
                }

                // Save the file path in the database

                var newBook = new Journals()
                {
                    Id = Guid.NewGuid(),
                    AuthorId = authorsInfo.Id,
                    CreatedOn = DateTime.UtcNow,
                    Description = journals.GetJournals.Description,
                    Title = journals.GetJournals.Title,
                    CategoryId = journals.GetJournals.CategoryId,
                    Status = "Pending",
                    TotalPages = journals.GetJournals.TotalPages,
                    BookPdfUrl = uniquePdfFileName,
                    CoverImageUrl = uniqueFileName,
                    LastUpdatedOn = DateTime.UtcNow,
                };
                _context.Add(newBook);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", journals.GetJournals.CategoryId);
            return View(journals);
        }

        // GET: Journals/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Journals == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);

            // Load user data asynchronously
            await LoadAsync(user);

            // Find the staff member with the provided email (Username)
            var userNameId = await _context.Authors.FirstOrDefaultAsync(x => x.Email == user.Email);
            if (userNameId != null)
            {
                ViewBag.User = userNameId.FullName;
            }

            var journals = await _context.Journals.FindAsync(id);
            var viewModel = new JournalsViewModel
            {
                GetJournals = journals,
            };

            if (journals == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", journals.CategoryId);
            return PartialView("_EditJournals", viewModel);
        }

        // POST: Journals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, JournalsViewModel journals)
        {
            //if (id != journals.GetJournals.Id)
            //{
            //    return NotFound();
            //}

            var agentInformations = User.FindFirstValue(ClaimTypes.Email);
            var authorsInfo = await _context.Authors.FirstOrDefaultAsync(si => si.Email == agentInformations);

            try
            {
                if (journals.GetJournals != null && journals.GetJournals.CoverPhoto != null)
                {
                    var updateCoverPhoto = journals.GetJournals.CoverPhoto;

                    // Retrieve the existing record from the database
                    var existingJournals = await _context.Journals
                        .Where(x => x.AuthorId == authorsInfo.Id && x.Id == id)
                        .FirstOrDefaultAsync();

                    if (existingJournals != null)
                    {
                        // Delete the old image file from the server's file system (if it exists)
                        if (!string.IsNullOrEmpty(existingJournals.CoverImageUrl))
                        {
                            var rootDirectory = Path.Combine(_hostingEnvironment.ContentRootPath, "../NAxiomindexings");
                            var oldFilePath = Path.Combine(rootDirectory, "wwwroot/assets/img/journal/cover", existingJournals.CoverImageUrl);

                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Save the new image file to the server's file system
                        string uniqueUpdateFileName = Guid.NewGuid().ToString() + "_" + updateCoverPhoto.FileName;
                        string uploadFolder = Path.Combine(_hostingEnvironment.ContentRootPath, "../NAxiomindexings/wwwroot/assets/img/journal/cover");
                        string newFilePath = Path.Combine(uploadFolder, uniqueUpdateFileName);

                        using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                        {
                            await updateCoverPhoto.CopyToAsync(fileStream);
                        }

                        // Update the modified fields
                        existingJournals.Description = journals.GetJournals.Description;
                        existingJournals.CategoryId = journals.GetJournals.CategoryId;
                        existingJournals.Title = journals.GetJournals.Title;
                        existingJournals.TotalPages = journals.GetJournals.TotalPages;
                        existingJournals.CoverImageUrl = uniqueUpdateFileName;
                        existingJournals.LastUpdatedOn = DateTime.UtcNow;

                        // Save the changes to the database
                        _context.Update(existingJournals);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return NotFound(); // Handle the case where the record is not found
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JournalsExists(journals.GetJournals.Id))
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

        // POST: Journals/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            if (_context.Journals == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Journals'  is null.");
            }
            var journals = await _context.Journals.FindAsync(id);

            // Delete the old image file from the server's file system (if it exists)
            if (!string.IsNullOrEmpty(journals.CoverImageUrl))
            {
                var rootDirectory = Path.Combine(_hostingEnvironment.ContentRootPath, "../NAxiomindexings");
                var oldFilePath = Path.Combine(rootDirectory, "wwwroot/assets/img/journal/cover", journals.CoverImageUrl);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // Delete the old pdf file from the server's file system (if it exists)
            if (!string.IsNullOrEmpty(journals.BookPdfUrl))
            {
                var rootDirectory = Path.Combine(_hostingEnvironment.ContentRootPath, "../NAxiomindexings");
                var oldPdfFilePath = Path.Combine(rootDirectory, "wwwroot/assets/img/journal/pdf", journals.BookPdfUrl);

                if (System.IO.File.Exists(oldPdfFilePath))
                {
                    System.IO.File.Delete(oldPdfFilePath);
                }
            }

            if (journals != null)
            {
                _context.Journals.Remove(journals);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JournalsExists(Guid id)
        {
            return (_context.Journals?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}