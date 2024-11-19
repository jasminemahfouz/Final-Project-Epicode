using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WomenActivity.Models;

namespace WomenActivity.Controllers
{
    [ApiController]
    [Route("CYcleTracker")]
    public class CycleTrackerController : Controller
    {
        private readonly WomenActivityDbContext _context;

        public CycleTrackerController(WomenActivityDbContext context)
        {
            _context = context;
        }

        // GET: CycleTracker
        // Aggiornamento del metodo Index per supportare filtro e ultimi 4 cicli
        [HttpGet]
        public async Task<IActionResult> Index(int? userId = null, string monthFilter = null)
        {
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Recupera tutti i cicli per l'utente
            var allCycles = _context.CycleRecords
                .Where(cr => cr.UserId == userId)
                .OrderByDescending(cr => cr.StartDate) // Ordina prima
                .AsQueryable(); // Assicurati che rimanga un IQueryable per ulteriori manipolazioni

            // Filtra per mese, se specificato
            if (!string.IsNullOrEmpty(monthFilter))
            {
                var selectedMonth = DateTime.ParseExact(monthFilter, "yyyy-MM", CultureInfo.InvariantCulture);
                allCycles = allCycles.Where(cr => cr.StartDate.Month == selectedMonth.Month && cr.StartDate.Year == selectedMonth.Year);
            }

            // Mostra solo gli ultimi 4 cicli se non è stato applicato un filtro
            var cycleRecords = string.IsNullOrEmpty(monthFilter)
                ? await allCycles.Take(4).ToListAsync() // Prendi gli ultimi 4 cicli
                : await allCycles.ToListAsync(); // Mostra tutto per il mese filtrato

            // Predizione del prossimo ciclo
            var prediction = PredictNextCycle(cycleRecords);
            ViewBag.Prediction = prediction;

            // Lista dei mesi disponibili
            ViewBag.AvailableMonths = _context.CycleRecords
     .Where(cr => cr.UserId == userId)
     .Select(cr => new { Year = cr.StartDate.Year, Month = cr.StartDate.Month })
     .AsEnumerable() // Sposta l'elaborazione lato client
     .Distinct()
     .OrderByDescending(cr => new DateTime(cr.Year, cr.Month, 1))
     .Select(cr => new DateTime(cr.Year, cr.Month, 1).ToString("yyyy-MM"))
     .ToList();


            ViewBag.CurrentMonthFilter = monthFilter;

            return View(cycleRecords);
        }


        // Function to predict the next cycle
        private CyclePrediction PredictNextCycle(List<CycleRecord> cycleRecords)
        {
            if (cycleRecords.Count < 2)
            {
                return null;
            }

            var lastCycle = cycleRecords[0];
            var previousCycle = cycleRecords[1];

            var averageCycleLength = (lastCycle.StartDate - previousCycle.StartDate).Days;
            var averageDuration = (lastCycle.EndDate - lastCycle.StartDate).Days;

            var predictedStartDate = lastCycle.StartDate.AddDays(averageCycleLength);
            var predictedEndDate = predictedStartDate.AddDays(averageDuration);

            return new CyclePrediction
            {
                PredictedStartDate = predictedStartDate,
                PredictedEndDate = predictedEndDate
            };
        }

        // GET: CycleTracker/AddCycleRecord
        [HttpGet("AddCycleRecord")]
        public IActionResult AddCycleRecord()
        {
            return View();
        }

        // POST: CycleTracker/AddCycleRecord
        [HttpPost("AddCycleRecord")]
        public async Task<IActionResult> AddCycleRecord([FromForm] CycleRecord cycleRecord)
        {
            if (ModelState.IsValid)
            {
                _context.CycleRecords.Add(cycleRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { userId = cycleRecord.UserId });
            }
            return View(cycleRecord);
        }
        // GET: CycleTracker/EditCycleRecord/{id}
        [HttpGet("EditCycleRecord/{id}")]
        public async Task<IActionResult> EditCycleRecord(int id)
        {
            var cycleRecord = await _context.CycleRecords.FindAsync(id);
            if (cycleRecord == null)
            {
                return NotFound();
            }
            return View(cycleRecord);
        }

        // POST: CycleTracker/EditCycleRecord/{id}
        [HttpPost("EditCycleRecord/{id}")]
        public async Task<IActionResult> EditCycleRecord(int id, [FromForm] CycleRecord cycleRecord)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(cycleRecord).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", new { userId = cycleRecord.UserId });
                }
                catch (DbUpdateException)
                {
                    return BadRequest("Errore durante l'aggiornamento del database.");
                }
            }

            return View(cycleRecord);
        }



        // POST: CycleTracker/DeleteCycleRecord/{id}
        [HttpPost("DeleteCycleRecord/{id}")]
        public async Task<IActionResult> DeleteCycleRecord(int id)
        {
            var cycleRecord = await _context.CycleRecords.FindAsync(id);
            if (cycleRecord == null)
            {
                return NotFound();
            }

            _context.CycleRecords.Remove(cycleRecord);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { userId = cycleRecord.UserId });
        }
    }
}
