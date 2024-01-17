using AxiomAdmin.Data;
using DMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AxiomAdmin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AdminDbContext _contextAdmin;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, AdminDbContext contextAdmin, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _contextAdmin = contextAdmin;
            _context = context;
            _userManager = userManager;
        }

        private async Task LoadAsync(IdentityUser user)
        {
            await _userManager.GetUserNameAsync(user);
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Administrator"))
            {
                return RedirectToAction("AdminDashboard");
            }
            else if (User.IsInRole("Validate-Manager"))
            {
                return RedirectToAction("ValidateDashboard");
            }
            else if (User.IsInRole("Blogger"))
            {
                // Redirect to the SalesController's POS action
                return RedirectToAction("POS", "Sales");
            }
            else if (User.IsInRole("Super-Administration"))
            {
                return RedirectToAction("AdminDashboard");
            }
            else
            {
                // Redirect the user to the login page or show an error message
                return RedirectToPage("/Identity/Account");
            }
        }

        [Route("dashboard")]
        [Authorize(Roles = "Administrator,Super-Administration")]
        public async Task<IActionResult> AdminDashboard()
        {
            var user = await _userManager.GetUserAsync(User);

            await LoadAsync(user);
            // Find the staff member with the provided email (Username)
            var userNameId = await _contextAdmin.UsersAccounts.FirstOrDefaultAsync(x => x.Email == user.Email);
            if (userNameId != null)
            {
                ViewBag.User = userNameId.FullName;
            }

            var totalTeams = await _context.TeamContexts.CountAsync();
            var totalDirectors = await _context.BoardOfDirectorsContexts.CountAsync();
            var totalVolunteers = await _context.Volunteers.CountAsync();
            var totalBlog = await _context.BlogPosts.Where(x => x.Visible == true).CountAsync();
            var totalValidateJournals = await _context.Journals.Where(x => x.Status == "Pending").CountAsync();
            var totalApproveJournals = await _context.Journals.Where(x => x.Status == "Approved").CountAsync();
            var totalDeclinedJournals = await _context.Journals.Where(x => x.Status == "Declined").CountAsync();
            var totalStaff = await _contextAdmin.UsersAccounts.CountAsync();

            // Set the time zone to Greenwich Standard Time (GMT)
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Greenwich Standard Time");
            var currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, timeZone);

            var greeting = "Good Morning";

            if (currentTime.Hour >= 12 && currentTime.Hour < 17)
            {
                greeting = "Good Afternoon";
            }
            else if (currentTime.Hour >= 17)
            {
                greeting = "Good Evening";
            }

            ViewData["TotalTeams"] = totalTeams;
            ViewData["TotalDirectors"] = totalDirectors;
            ViewData["TotalVolunteers"] = totalVolunteers;
            ViewData["TotalBlog"] = totalBlog;
            ViewData["TotalValidateJournals"] = totalValidateJournals;
            ViewData["TotalApproveJournals"] = totalApproveJournals;
            ViewData["TotalDeclinedJournals"] = totalDeclinedJournals;
            ViewData["TotalStaff"] = totalStaff;
            ViewData["Greeting"] = greeting; // Changed to start with an uppercase letter (convention)

            return View();
        }

        [Route("validate-dashboard")]
        [Authorize(Roles = "Validate-Manager")]
        public async Task<IActionResult> ValidateDashboard()
        {
            //var user = await _userManager.GetUserAsync(User);

            //await LoadAsync(user);

            var totalValidateJournals = await _context.Journals.Where(x => x.Status == "Pending").CountAsync();
            var totalApproveJournals = await _context.Journals.Where(x => x.Status == "Approved").CountAsync();
            var totalDeclinedJournals = await _context.Journals.Where(x => x.Status == "Declined").CountAsync();

            ViewData["TotalValidateJournals"] = totalValidateJournals;
            ViewData["TotalApproveJournals"] = totalApproveJournals;
            ViewData["TotalDeclinedJournals"] = totalDeclinedJournals;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}