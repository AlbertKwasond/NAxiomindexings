using AxiomAdmin.Data;
using DMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AxiomAdmin.Controllers
{
    [Authorize(Roles = "Administrator,Super-Administration,Validate-Manager")]
    public class ValidateController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ValidateController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: ValidateController
        public async Task<ActionResult> ValidateList()
        {
            var applicationDbContext = await _context.Journals
                .Include(j => j.Authors)
                .Include(j => j.Categories)
                .Where(x => x.Status == "Pending").ToListAsync();

            return View(applicationDbContext);
        }

        // GET: ValidateController/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null || _context.Journals == null)
            {
                return NotFound();
            }

            var journals = await _context.Journals
                .Include(j => j.Categories)
                .Include(j => j.Authors)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (journals == null)
            {
                return NotFound();
            }

            return View(journals);
        }

        // GET: ValidateController/ViewPDF
        public async Task<ActionResult> ViewPDFAsync(Guid? id)
        {
            if (id == null || _context.Journals == null)
            {
                return NotFound();
            }

            var journals = await _context.Journals
                .Include(j => j.Categories)
                .Include(j => j.Authors)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (journals == null)
            {
                return NotFound();
            }

            return View(journals);
        }

        // POST: ValidateController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Approved(Guid id, Journals journals)
        {
            try
            {
                if (id != journals.Id)
                {
                    return NotFound();
                }
                var Findjournal = await _context.Journals.Where(x => x.Id == id).FirstOrDefaultAsync();
                Findjournal.Status = "Approved";

                _context.Update(Findjournal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ValidateList));
            }
            catch
            {
                return View();
            }
        }

        // POST: ValidateController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Declined(Guid id, Journals journals)
        {
            try
            {
                if (id != journals.Id)
                {
                    return NotFound();
                }
                var Findjournal = await _context.Journals.Where(x => x.Id == id).FirstOrDefaultAsync();
                Findjournal.Status = "Declined";

                _context.Update(Findjournal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ValidateList));
            }
            catch
            {
                return View();
            }
        }

        // GET: ValidateController/ApprovedList
        public async Task<ActionResult> ApprovedList()
        {
            var applicationDbContext = await _context.Journals
               .Include(j => j.Authors)
               .Include(j => j.Categories)
               .Where(x => x.Status == "Approved").ToListAsync();

            return View(applicationDbContext);
        }

        // GET: ValidateController/DeclinedList
        public async Task<ActionResult> DeclinedList()
        {
            var applicationDbContext = await _context.Journals
               .Include(j => j.Authors)
               .Include(j => j.Categories)
               .Where(x => x.Status == "Declined").ToListAsync();

            return View(applicationDbContext);
        }

        // POST: ValidateController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ValidateController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ValidateController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}