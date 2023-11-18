using AxiomAdmin.Data;
using AxiomAdmin.ViewModel;
using DMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;

namespace AxiomAdmin.Controllers
{
    public class TeamContextsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TeamContextsController> _logger;

        public TeamContextsController(ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment hostingEnvironment, ILogger<TeamContextsController> logger)
        {
            _context = context;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        // GET: TeamContexts
        public async Task<IActionResult> Index()
        {
            var teamContextList = await _context.TeamContexts.ToListAsync();

            var viewModel = new TeamContextViewModel
            {
                ListTeamContexts = teamContextList,
            };
            return View(viewModel);
        }

        // POST: TeamContexts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(TeamContextViewModel teamContext, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return View(teamContext);
            }

            try
            {
                // Get the root directory of the NAxiomindexings project
                string rootDirectory = Path.Combine(_hostingEnvironment.ContentRootPath, "../NAxiomindexings");

                // Define the path to the folder where you want to save the image in the NAxiomindexings project
                string uploadFolder = Path.Combine(rootDirectory, "wwwroot/assets/img/team");

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
                teamContext.TeamContexts.ImageUrl = uniqueFileName;
                teamContext.TeamContexts.Id = Guid.NewGuid();
                _context.Add(teamContext.TeamContexts);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "An error occurred while uploading the file.");

                // Show an error message to the user
                ModelState.AddModelError(string.Empty, "An error occurred while uploading the file. Please try again.");
                return View(teamContext);
            }
        }

        // GET: TeamContexts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.TeamContexts == null)
            {
                return NotFound();
            }

            var teamContext = await _context.TeamContexts.FindAsync(id);
            var viewModel = new TeamContextViewModel
            {
                TeamContexts = teamContext,
            };

            if (teamContext == null)
            {
                return NotFound();
            }
            return PartialView("_EditTeamContexts", viewModel);
        }

        // POST: TeamContexts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, TeamContextViewModel teamContext, IFormFile file)
        {
            if (id != teamContext.TeamContexts.Id)
            {
                return NotFound();
            }

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("Image", "Please select an image file.");
                return View(teamContext.TeamContexts);
            }

            try
            {
                // Retrieve the existing record from the database
                var existingTeamContext = await _context.TeamContexts.FindAsync(teamContext.TeamContexts.Id);
                if (existingTeamContext == null)
                {
                    return NotFound();
                }

                // Delete the old image file from the server's file system (if it exists)
                if (!string.IsNullOrEmpty(existingTeamContext.ImageUrl))
                {
                    var oldFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, existingTeamContext.ImageUrl);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Save the new image file to the server's file system
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string uploadFolder = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot/assets/img/team");
                string newFilePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Update the record in the database
                existingTeamContext.ImageUrl = newFilePath;
                // ...

                // Save the changes to the database
                _context.Update(existingTeamContext);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the image.");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the image. Please try again.");
                return View(teamContext.TeamContexts);
            }
        }

        // POST: TeamContexts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.TeamContexts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.TeamContexts'  is null.");
            }
            var teamContext = await _context.TeamContexts.FindAsync(id);
            if (teamContext != null)
            {
                _context.TeamContexts.Remove(teamContext);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamContextExists(Guid id)
        {
            return (_context.TeamContexts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}