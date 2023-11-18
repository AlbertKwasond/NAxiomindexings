using DMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NAxiomindexings.Data;
using System.Diagnostics;

namespace NAxiomindexings.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> IndexAsync()
        {
            return _context.HomeContexts != null ?
                View(await _context.HomeContexts.ToListAsync()) :
                Problem("Entity set 'ApplicationDbContext.HomeContexts'  is null.");
        }

        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> TeamAsync()
        {
            return _context.TeamContexts != null ?
                View(await _context.TeamContexts.ToListAsync()) :
                Problem("Entity set 'ApplicationDbContext.TeamContexts'  is null.");
        }

        public async Task<IActionResult> BoardOfDirectorsAsync()
        {
            return _context.BoardOfDirectorsContexts != null ?
                View(await _context.BoardOfDirectorsContexts.ToListAsync()) :
                Problem("Entity set 'ApplicationDbContext.BoardOfDirectorsContexts'  is null.");
        }

        public IActionResult EvaluateProcess()
        {
            return View();
        }

        public IActionResult EditorialSelectionProcess()
        {
            return View();
        }

        public IActionResult JournalInclusionProcess()
        {
            return View();
        }

        public IActionResult BookInclusionProcess()
        {
            return View();
        }

        public IActionResult ConferencePrecedingInclusionProcess()
        {
            return View();
        }

        public IActionResult GeneralPolicies()
        {
            return View();
        }

        public IActionResult AppealsProcess()
        {
            return View();
        }

        public IActionResult JournalIndex()
        {
            return View();
        }

        public IActionResult CitationReport()
        {
            return View();
        }

        public IActionResult GuideToApplying()
        {
            return View();
        }

        public IActionResult API()
        {
            return View();
        }

        public IActionResult WhyIndexYourJournalInAxiomindexing()
        {
            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }

        public IActionResult XML()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}