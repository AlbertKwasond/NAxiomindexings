using DMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NAxiomindexings.Data;
using NAxiomindexings.ViewModel;
using System.Diagnostics;
using Rotativa.AspNetCore;
using System.IO;
using System.Threading.Tasks;

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

		public async Task<IActionResult> Search(string category, string inputSearch)
		{
			if (category == "Journals")
			{
				var searchJournal = await _context.Journals
					.Where(x => x.Title == inputSearch)
					.Include(x => x.Authors)
					.Include(x => x.Categories)
					.FirstOrDefaultAsync();

				if (searchJournal == null)
				{
					// Handle the case where the searched journal is not found
					return View("NotFoundView");
				}
				var journals = await _context.Journals
					.Where(j => j.Id != searchJournal.Id) // Exclude the selected journal
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
					.Where(x => x.Authors.FullName == inputSearch)
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
				.Where(x => x.Id == Id)
				.Include(x => x.Authors)
				.Include(x => x.Categories)
				.FirstOrDefaultAsync();

			if (searchJournal == null)
			{
				// Handle the case where the searched journal is not found
				return View("NotFoundView");
			}

			var journals = await _context.Journals
				.Where(j => j.Id != searchJournal.Id) // Exclude the selected journal
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
					.Where(x => x.Title.Contains(term))
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