using Microsoft.AspNetCore.Mvc;
using WomenActivity.Models;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Authorization;

namespace WomenActivity.Controllers
{
    [ApiController]
    [Route("Routine")]
    public class RoutineController : Controller
    {
        private readonly WomenActivityDbContext _context;
        private readonly ILogger<RoutineController> _logger;

        public RoutineController(WomenActivityDbContext context, ILogger<RoutineController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private int? GetUserId()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int parsedUserId))
            {
                return parsedUserId;
            }
            return null;
        }

        // Visualizza tutte le routine
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var routines = await _context.Routines
                .Where(r => r.UserId == userId.Value)
                .ToListAsync();

            return View(routines);
        }

        // Aggiungi una nuova routine
        [HttpGet("AddRoutine")]

        public IActionResult AddRoutine()
        {
            return View();
        }

        [HttpPost("AddRoutine")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRoutine([FromForm] Routine routine)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Dati non validi." });
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "Utente non autenticato." });
            }

            try
            {
                routine.UserId = userId.Value;

                // Genera le occorrenze
                var recurringRoutines = GenerateRecurringRoutines(routine);

                // Salva tutte le occorrenze
                _context.Routines.AddRange(recurringRoutines);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Routine aggiunta con successo." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Errore durante l'aggiunta della routine: {ex.Message}");
                return Json(new { success = false, message = "Errore durante l'aggiunta della routine." });
            }
        }

        [HttpGet("EditRoutine/{id:int}")]
        public async Task<IActionResult> EditRoutine(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var routine = await _context.Routines
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId.Value);

            if (routine == null)
            {
                return NotFound("Routine non trovata.");
            }

            return View(routine);
        }

        [HttpPost("EditRoutine/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoutine(int id, [FromForm] Routine routine)
        {
            if (id != routine.Id)
            {
                return NotFound(); // Se l'ID non corrisponde, ritorna 404
            }

            if (!ModelState.IsValid)
            {
                return View(routine); // Torna alla vista per correggere gli errori
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            try
            {
                var existingRoutines = _context.Routines
                    .Where(r => r.UserId == userId && r.Name == routine.Name)
                    .ToList();

                // Rimuove le occorrenze esistenti
                _context.Routines.RemoveRange(existingRoutines);

                // Genera nuove occorrenze
                routine.UserId = userId.Value;
                var updatedRoutines = GenerateRecurringRoutines(routine);

                // Aggiungi le nuove occorrenze
                _context.Routines.AddRange(updatedRoutines);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Errore durante la modifica della routine: {ex.Message}");
                return View(routine); // Torna alla vista con un errore
            }
        }

        [HttpGet("DeleteRoutine/{id:int}")]
        public async Task<IActionResult> DeleteRoutine(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var routine = await _context.Routines
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId.Value);

            if (routine == null)
            {
                return NotFound("Routine non trovata.");
            }

            return View(routine);
        }

        [HttpPost, ActionName("DeleteRoutine")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var routine = await _context.Routines.FindAsync(id);

            if (routine == null)
            {
                return NotFound(); // Routine non trovata
            }

            try
            {
                // Trova tutte le occorrenze basate su nome e utente
                var recurringRoutines = _context.Routines
                    .Where(r => r.UserId == routine.UserId && r.Name == routine.Name)
                    .ToList();

                // Rimuove tutte le occorrenze
                _context.Routines.RemoveRange(recurringRoutines);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Routine con ID {id} eliminata con successo.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Errore durante l'eliminazione della routine: {ex.Message}");
                return RedirectToAction(nameof(Index)); // Ritorna alla pagina principale anche in caso di errore
            }
        }


        private List<Routine> GenerateRecurringRoutines(Routine routine)
        {
            var recurringRoutines = new List<Routine>();

            DateTime startDate = routine.Date;
            DateTime endDate = startDate.AddMonths(6); // Limite a 6 mesi

            DateTime currentDate = startDate;

            // Genera le occorrenze in base alla frequenza
            while (currentDate <= endDate)
            {
                recurringRoutines.Add(new Routine
                {
                    Name = routine.Name,
                    UserId = routine.UserId,
                    Time = routine.Time,
                    Frequency = routine.Frequency,
                    RoutineType = routine.RoutineType,
                    Date = currentDate
                });

                // Incrementa la data in base alla frequenza
                currentDate = routine.Frequency switch
                {
                    FrequencyType.Daily => currentDate.AddDays(1),
                    FrequencyType.Weekly => currentDate.AddDays(7),
                    FrequencyType.Monthly => currentDate.AddMonths(1),
                    _ => currentDate
                };
            }

            return recurringRoutines;
        }

    }
}