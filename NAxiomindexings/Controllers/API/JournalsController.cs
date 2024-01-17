using DMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NAxiomindexings.Data;

namespace NAxiomindexings.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class JournalsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public JournalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Journals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Journals>>> GetJournals()
        {
            if (_context.Journals == null)
            {
                return NotFound();
            }
            return await _context.Journals.Where(x => x.Status == "Approved")
                .ToListAsync();
        }

        // GET: api/Journals/5
        [HttpGet("{search}")]
        public async Task<ActionResult<Journals>> GetJournals(string search)
        {
            if (_context.Journals == null)
            {
                return NotFound();
            }
            var journals = await _context.Journals
                .Where(x => x.Title == search || x.Authors.FullName == search && x.Status == "Approved")
                .FirstOrDefaultAsync();

            if (journals == null)
            {
                return NotFound();
            }

            return journals;
        }
    }
}