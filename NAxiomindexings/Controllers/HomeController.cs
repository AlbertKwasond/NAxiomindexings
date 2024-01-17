using DMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NAxiomindexings.Data;
using NAxiomindexings.ViewModel;
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
            return RedirectToAction("Home");
        }

        [Route("home")]
        public async Task<IActionResult> Home()
        {
            var homeContextsList = await _context.HomeContexts.ToListAsync();
            var listBlog = await _context.BlogPosts
            .Where(j => j.Visible == true) // Exclude the selected blog posts
            .Take(3) // Take only the top 4 blog posts
            .ToListAsync();

            var viewModel = new HomeViewModel
            {
                ListHomeContexts = homeContextsList,
                ListBlogPosts = listBlog
            };

            return View(viewModel);
        }

        [Route("journals-details")]
        public async Task<IActionResult> Search(string category, string inputSearch)
        {
            if (category == "Journals")
            {
                var searchJournal = await _context.Journals
                    .Where(x => x.Title == inputSearch && x.Status == "Approved")
                    .Include(x => x.Authors)
                    .Include(x => x.Categories)
                    .FirstOrDefaultAsync();

                if (searchJournal == null)
                {
                    // Handle the case where the searched journal is not found
                    return RedirectToAction("NotFoundView");
                }
                var journals = await _context.Journals
                    .Where(j => j.Id != searchJournal.Id && j.Status == "Approved") // Exclude the selected journal
                    .Include(x => x.Authors)
                    .Include(x => x.Categories)
                    .Take(4) // Take only the top 4 journals
                    .ToListAsync();

                var viewModel = new JournalsViewModel
                {
                    GetJournals = searchJournal,
                    ListJournals = journals
                };

                return View("Details", viewModel);
            }
            else if (category == "Author")
            {
                var searchJournal = await _context.Journals
                    .Where(x => x.Authors.FullName == inputSearch && x.Status == "Approved")
                    .FirstOrDefaultAsync();

                // Handle the case where the searched journal by author is not found
                if (searchJournal == null)
                {
                    return View("NotFoundView");
                }

                var viewModel = new JournalsViewModel
                {
                    GetJournals = searchJournal,
                    ListJournals = null // You might want to set this to something meaningful for author search
                };

                return View("Details", viewModel);
            }

            return View("NotFoundView"); // Handle the case where the category is not recognized
        }

        // Action to view the PDF
        public IActionResult DownloadPdf(Guid journalId)
        {
            var journal = _context.Journals.Find(journalId);

            if (journal == null)
            {
                return NotFound();
            }

            var pdfPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "assets",
                "img",
                "journal",
                "pdf",
                $"{journal.BookPdfUrl}"
            );

            // Provide the PDF file for download
            return PhysicalFile(pdfPath, "application/pdf", $"{journal.BookPdfUrl}.pdf");
        }

        public IActionResult PdfView()
        {
            return View();
        }

        public async Task<IActionResult> Details(Guid Id)
        {
            var searchJournal = await _context.Journals
                .Where(x => x.Id == Id && x.Status == "Approved")
                .Include(x => x.Authors)
                .Include(x => x.Categories)
                .FirstOrDefaultAsync();

            if (searchJournal == null)
            {
                // Handle the case where the searched journal is not found
                return View("NotFoundView");
            }

            var journals = await _context.Journals
                .Where(j => j.Id != searchJournal.Id && j.Status == "Approved") // Exclude the selected journal
                .Include(x => x.Authors)
                .Include(x => x.Categories)
                .Take(4) // Take only the top 4 journals
                .ToListAsync();

            var viewModel = new JournalsViewModel
            {
                GetJournals = searchJournal,
                ListJournals = journals
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetAutoCompleteData(string term, string category)
        {
            if (string.IsNullOrEmpty(term))
            {
                // Handle the case where the term is null or empty
                return Json(new List<string>());
            }

            if (category == "Journals")
            {
                var autocompleteData = await _context.Journals
                    .Where(x => x.Title.Contains(term) && x.Status == "Approved")
                    .Select(x => x.Title)
                    .ToListAsync();

                return Json(autocompleteData);
            }

            if (category == "Author")
            {
                var autocompleteData = await _context.Authors
                    .Where(author => author.FirstName.Contains(term))
                    .Select(author => author.FullName)
                    .ToListAsync();

                return Json(autocompleteData);
            }

            // Handle the case where the category is not recognized
            return Json(new List<string>());
        }

        [HttpGet]
        [Route("find-journals")]
        public async Task<IActionResult> FindJournalsAsync(int page = 1, int pageSize = 10)
        {
            var totalItems = await _context.Journals.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var allJournal = await _context.Journals
                .Where(x => x.Status == "Approved")
                .Include(x => x.Authors)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var allCategories = await _context.Categories.Include(x => x.Journals).ToListAsync();

            var viewModel = new JournalsViewModel
            {
                ListJournals = allJournal,
                ListCategories = allCategories,
                PageNumber = page,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        public async Task<IActionResult> AuthorSearch(Guid id)

        {
            var searchJournal = await _context.Journals
                    .Where(x => x.AuthorId == id && x.Status == "Approved")
                    .FirstOrDefaultAsync();

            // Handle the case where the searched journal by author is not found
            if (searchJournal == null)
            {
                return RedirectToAction("NotFoundView");
            }

            var viewModel = new JournalsViewModel
            {
                GetJournals = searchJournal,
            };

            return View("FindJournalsAsync", viewModel);
        }

        public async Task<IActionResult> SearchJournals(string inputSearch)
        {
            var allCategories = await _context.Categories.Include(x => x.Journals).ToListAsync();

            var searchJournal = await _context.Journals
                .Where(x => x.Title == inputSearch && x.Status == "Approved")
                .Include(x => x.Authors)
                .Include(x => x.Categories)
                .ToListAsync();

            if (searchJournal == null)
            {
                // Handle the case where the searched journal is not found
                return RedirectToAction("NotFoundView");
            }

            var viewModel = new JournalsViewModel
            {
                ListJournals = searchJournal,
                ListCategories = allCategories
            };

            return View("FindJournals", viewModel);
        }

        public async Task<IActionResult> NotFoundView(int page = 1, int pageSize = 10)
        {
            var totalItems = await _context.Journals.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var allJournal = await _context.Journals
            .Include(x => x.Authors)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var allCategories = await _context.Categories.Include(x => x.Journals).ToListAsync();

            var viewModel = new JournalsViewModel
            {
                ListJournals = allJournal,
                ListCategories = allCategories,
                PageNumber = page,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        public async Task<IActionResult> CategoriesAsync(Guid Id)
        {
            var allCategories = await _context.Categories.Include(x => x.Journals).ToListAsync();

            var searchJournal = await _context.Journals
            .Where(x => x.CategoryId == Id && x.Status == "Approved")
            .Include(x => x.Authors)
            .Include(x => x.Categories)
            .ToListAsync();

            if (searchJournal == null)
            {
                // Handle the case where the searched journal is not found
                return RedirectToAction("NotFoundView");
            }

            var viewModel = new JournalsViewModel
            {
                ListJournals = searchJournal,
                ListCategories = allCategories
            };
            return View("FindJournals", viewModel);
        }

        [Route("about")]
        public IActionResult About()
        {
            return View();
        }

        [Route("our-team")]
        public async Task<IActionResult> TeamAsync()
        {
            return _context.TeamContexts != null ?
                View(await _context.TeamContexts.ToListAsync()) :
                Problem("Entity set 'ApplicationDbContext.TeamContexts'  is null.");
        }

        [Route("directors")]
        public async Task<IActionResult> BoardOfDirectorsAsync()
        {
            return _context.BoardOfDirectorsContexts != null ?
                View(await _context.BoardOfDirectorsContexts.ToListAsync()) :
                Problem("Entity set 'ApplicationDbContext.BoardOfDirectorsContexts'  is null.");
        }

        [Route("volunteers")]
        public async Task<IActionResult> VolunteersAsync()
        {
            return _context.Volunteers != null ?
                         View(await _context.Volunteers.ToListAsync()) :
                         Problem("Entity set 'ApplicationDbContext.Volunteers'  is null.");
        }

        [Route("evaluate-process")]
        public IActionResult EvaluateProcess()
        {
            return View();
        }

        [Route("editorial-selection-process")]
        public IActionResult EditorialSelectionProcess()
        {
            return View();
        }

        [Route("journal-inclusion-process")]
        public IActionResult JournalInclusionProcess()
        {
            return View();
        }

        [Route("book-inclusion-process")]
        public IActionResult BookInclusionProcess()
        {
            return View();
        }

        [Route("conference=precedings-inclusion")]
        public IActionResult ConferencePrecedingInclusionProcess()
        {
            return View();
        }

        [Route("general-policies")]
        public IActionResult GeneralPolicies()
        {
            return View();
        }

        [Route("appeals-process")]
        public IActionResult AppealsProcess()
        {
            return View();
        }

        [Route("journal-index")]
        public IActionResult JournalIndex()
        {
            return View();
        }

        [Route("citation-report")]
        public IActionResult CitationReport()
        {
            return View();
        }

        [Route("guide-to-Applying")]
        public IActionResult GuideToApplying()
        {
            return View();
        }

        [Route("api")]
        public IActionResult API()
        {
            return View();
        }

        [Route("why-index-your-journal-in-axiomindexing")]
        public IActionResult WhyIndexYourJournalInAxiomindexing()
        {
            return View();
        }

        [Route("faq")]
        public IActionResult FAQ()
        {
            return View();
        }

        [Route("xml")]
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