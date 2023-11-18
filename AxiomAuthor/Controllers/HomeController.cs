using AxiomAuthor.Data;
using AxiomAuthor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AxiomAuthor.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public string? Username { get; set; }

        private async Task LoadAsync(IdentityUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
            // Additional logic if needed
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                // Handle the case where the user is not found
                return NotFound();
            }

            // Load user data asynchronously
            await LoadAsync(user);

            // Find the author with the provided email (Username)
            var author = await _context.Authors.FirstOrDefaultAsync(x => x.Email == user.Email);

            if (author == null)
            {
                // Handle the case where the author is not found
                return NotFound();
            }

            var countJournals = await _context.Journals.Where(x => x.AuthorId == author.Id).CountAsync();
            var countApproveJournals = await _context.Journals.Where(x => x.AuthorId == author.Id && x.Status == "Approve").CountAsync();
            var countDeclineJournals = await _context.Journals.Where(x => x.AuthorId == author.Id && x.Status == "Decline").CountAsync();
            var countPendingJournals = await _context.Journals.Where(x => x.AuthorId == author.Id && x.Status == "Pending").CountAsync();

            ViewData["countJournals"] = countJournals;
            ViewData["countApproveJournals"] = countApproveJournals;
            ViewData["countDeclineJournals"] = countDeclineJournals;
            ViewData["countPendingJournals"] = countPendingJournals;

            return View();
        }

        public IActionResult Privacy()
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