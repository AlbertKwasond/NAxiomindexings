using AxiomAdmin.Data;
using AxiomAdmin.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AxiomAdmin.Controllers
{
    [Authorize(Roles = "Administrator,Super-Administration")]
    public class VolunteersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<TeamContextsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string hostLinktoken;

        public VolunteersController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment, ILogger<TeamContextsController> logger, IConfiguration configuration)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _configuration = configuration;
            hostLinktoken = _configuration["HostLink:URLlink"];
        }

        // GET: Volunteers
        public async Task<IActionResult> Index()
        {
            var listVolunteers = await _context.Volunteers.ToListAsync();
            var viewModel = new VolunteersViewModel()
            {
                ListVolunteers = listVolunteers,
            };

            return View(viewModel);
        }

        // POST: Volunteers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(VolunteersViewModel volunteers, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return View(volunteers);
            }

            try
            {
                // Get the root directory of the NAxiomindexings project
                string rootDirectory = Path.Combine(_hostingEnvironment.ContentRootPath, hostLinktoken);

                // Define the path to the folder where you want to save the image in the NAxiomindexings project
                string uploadFolder = Path.Combine(rootDirectory, "wwwroot/assets/img/volunteers");

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
                volunteers.GetVolunteers.ImageUrl = uniqueFileName;
                volunteers.GetVolunteers.Id = Guid.NewGuid();
                _context.Add(volunteers.GetVolunteers);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "An error occurred while uploading the file.");

                // Show an error message to the user
                ModelState.AddModelError(string.Empty, "An error occurred while uploading the file. Please try again.");
                return PartialView("_SaveVolunteers", volunteers);
            }
        }

        // GET: Volunteers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Volunteers == null)
            {
                return NotFound();
            }

            var volunteers = await _context.Volunteers.FindAsync(id);
            var viewModel = new VolunteersViewModel
            {
                GetVolunteers = volunteers,
            };

            if (volunteers == null)
            {
                return NotFound();
            }
            return PartialView("_EditVolunteers", viewModel);
        }

        // POST: Volunteers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, VolunteersViewModel volunteers, IFormFile files)
        {
            var existingTeamContext = await _context.TeamContexts.FindAsync(volunteers.GetVolunteers.Id);

            try
            {
                // Retrieve the existing record from the database

                if (id != volunteers.GetVolunteers.Id)
                {
                    return NotFound();
                }

                if (files == null || files.Length == 0)
                {
                    ModelState.AddModelError("Image", "Please select an image file.");
                    return View(volunteers.GetVolunteers);
                }

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
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + files.FileName;
                string uploadFolder = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot/assets/img/volunteers");
                string newFilePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                {
                    await files.CopyToAsync(fileStream);
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
                return View(volunteers.GetVolunteers);
            }

            existingTeamContext.Name = volunteers.GetVolunteers.Name;

            _context.Update(existingTeamContext);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Volunteers/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            if (_context.Volunteers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Volunteers'  is null.");
            }
            var volunteers = await _context.Volunteers.FindAsync(id);

            // Delete the old image file from the server's file system (if it exists)
            if (!string.IsNullOrEmpty(volunteers.ImageUrl))
            {
                var rootDirectory = Path.Combine(_hostingEnvironment.ContentRootPath, "../https://axiomindexing.com");
                var oldFilePath = Path.Combine(rootDirectory, "wwwroot/assets/img/volunteers", volunteers.ImageUrl);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            if (volunteers != null)
            {
                _context.Volunteers.Remove(volunteers);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VolunteersExists(Guid id)
        {
            return (_context.Volunteers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}