using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WomenActivity.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace WomenActivity.Controllers
{
    [ApiController]
    [Route("DailyWellness")]
    public class DailyWellnessController : Controller
    {
        private readonly WomenActivityDbContext _context;

        public DailyWellnessController(WomenActivityDbContext context)
        {
            _context = context;
        }

        // GET: /DailyWellness
        [HttpGet]
        public async Task<IActionResult> Index(int? userId = null, string filter = "all")
        {
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var today = DateTime.UtcNow;

            // Recupera tutti i record per l'utente
            var wellnessRecords = _context.DailyWellnessRecords
                .Where(wr => wr.UserId == userId);

            // Applica il filtro
            switch (filter.ToLower())
            {
                case "week":
                    var weekStart = today.AddDays(-7);
                    wellnessRecords = wellnessRecords.Where(wr => wr.Date >= weekStart && wr.Date <= today);
                    break;
                case "month":
                    var monthStart = new DateTime(today.Year, today.Month, 1);
                    wellnessRecords = wellnessRecords.Where(wr => wr.Date >= monthStart && wr.Date <= today);
                    break;
                case "all":
                default:
                    // Nessun filtro: mostra tutti i record
                    break;
            }

            // Ordina i record per data
            var orderedRecords = await wellnessRecords
                .OrderByDescending(wr => wr.Date)
                .ToListAsync();

            // Passa il filtro corrente al ViewBag
            ViewBag.CurrentFilter = filter;

            return View(orderedRecords);
        }


        // GET: /DailyWellness/AddWellnessRecord
        [HttpGet("AddWellnessRecord")]
        public IActionResult AddWellnessRecord()
        {
            return View();
        }

        // POST: /DailyWellness/AddWellnessRecord
        [HttpPost("AddWellnessRecord")]
        public async Task<IActionResult> AddWellnessRecord([FromForm] DailyWellnessRecord wellnessRecord)
        {
            if (ModelState.IsValid)
            {
                _context.DailyWellnessRecords.Add(wellnessRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { userId = wellnessRecord.UserId });
            }
            return View(wellnessRecord);
        }

        // GET: /DailyWellness/EditWellnessRecord/{id}
        [HttpGet("EditWellnessRecord/{id}")]
        public async Task<IActionResult> EditWellnessRecord(int id)
        {
            var wellnessRecord = await _context.DailyWellnessRecords.FindAsync(id);
            if (wellnessRecord == null)
            {
                return NotFound();
            }
            return View(wellnessRecord);
        }
        // POST: /DailyWellness/EditWellnessRecord/{id}
        [HttpPost("EditWellnessRecord/{id}")]
        public async Task<IActionResult> EditWellnessRecord(int id, [FromForm] DailyWellnessRecord wellnessRecord)
        {
            if (id != wellnessRecord.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _context.Entry(wellnessRecord).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { userId = wellnessRecord.UserId });
            }
            return View(wellnessRecord);
        }

        // POST: /DailyWellness/DeleteWellnessRecord/{id}
        [HttpPost("DeleteWellnessRecord/{id}")]
        public async Task<IActionResult> DeleteWellnessRecord(int id)
        {
            var wellnessRecord = await _context.DailyWellnessRecords.FindAsync(id);
            if (wellnessRecord == null)
            {
                return NotFound();
            }

            _context.DailyWellnessRecords.Remove(wellnessRecord);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { userId = wellnessRecord.UserId });
        }
    }
}
