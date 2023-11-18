using AxiomAdmin.Data;
using AxiomAdmin.ViewModel;
using DMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AxiomAdmin.Controllers
{
    public class HomeContextsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeContextsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HomeContexts
        public async Task<IActionResult> Index()
        {
            var homeContextList = await _context.HomeContexts.ToListAsync();

            var viewModel = new HomeContextViewModel
            {
                homeContexts = homeContextList,
            };
            return View(viewModel);
        }

        // GET: HomeContexts/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.HomeContexts == null)
            {
                return NotFound();
            }

            var homeContext = await _context.HomeContexts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (homeContext == null)
            {
                return NotFound();
            }

            return View(homeContext);
        }

        // GET: HomeContexts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HomeContexts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HomeContextViewModel homeContextViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(homeContextViewModel.HomeContexts);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return PartialView("Create", homeContextViewModel);
        }

        // GET: HomeContexts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.HomeContexts == null)
            {
                return NotFound();
            }

            var homeContext = await _context.HomeContexts.FindAsync(id);

            var viewModel = new HomeContextViewModel
            {
                HomeContexts = homeContext,
            };

            if (homeContext == null)
            {
                return NotFound();
            }
            return PartialView("Edit", viewModel);
        }

        // POST: HomeContexts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, HomeContextViewModel homeContext)
        {
            if (id != homeContext.HomeContexts.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(homeContext.HomeContexts);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HomeContextExists(homeContext.HomeContexts.Id))
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
            return View(homeContext);
        }

        // POST: HomeContexts/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            if (_context.HomeContexts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.HomeContexts'  is null.");
            }
            var homeContext = await _context.HomeContexts.FindAsync(id);
            if (homeContext != null)
            {
                _context.HomeContexts.Remove(homeContext);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HomeContextExists(Guid id)
        {
            return (_context.HomeContexts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}