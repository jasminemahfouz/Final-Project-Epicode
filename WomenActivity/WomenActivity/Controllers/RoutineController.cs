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
    [Route("api/[controller]")]
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
        [HttpGet("Add")]

        public IActionResult AddRoutine()
        {
            return View();
        }

        [HttpPost("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRoutine(Routine routine)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "Utente non autenticato" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    routine.UserId = userId.Value;
                    DateTime startDate = routine.Date;
                    DateTime endDate = startDate.AddMonths(6); // Limite a 6 mesi per le occorrenze

                    var recurringRoutines = new System.Collections.Generic.List<Routine>();

                    // Genera le occorrenze in base alla frequenza
                    DateTime currentDate = startDate;
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

                    // Aggiungi tutte le routine ricorrenti
                    _context.Routines.AddRange(recurringRoutines);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Routine aggiunta con successo" });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Errore durante l'aggiunta della routine: {ex.Message}");
                    return Json(new { success = false, message = "Errore durante l'aggiunta della routine" });
                }
            }

            return Json(new { success = false, message = "Dati della routine non validi" });
        }

        // GET: Routine/EditRoutine
        // Modifica una routine
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> EditRoutine(int id)
        {
            var routine = await _context.Routines.FindAsync(id);
            if (routine == null)
            {
                return NotFound();
            }
            return View(routine);
        }

        // POST: Routine/EditRoutine
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoutine(int id, Routine routine)
        {
            if (id != routine.Id)
            {
                return NotFound();
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Trova tutte le occorrenze esistenti della routine
                    var existingRoutines = _context.Routines
                        .Where(r => r.UserId == userId && r.Name == routine.Name)
                        .ToList();

                    // Rimuove tutte le occorrenze esistenti
                    _context.Routines.RemoveRange(existingRoutines);

                    // Imposta i nuovi valori della routine
                    routine.UserId = userId.Value;

                    // Crea le nuove occorrenze in base alla nuova frequenza
                    DateTime startDate = routine.Date;
                    DateTime endDate = startDate.AddMonths(6); // Limite di 6 mesi per le occorrenze
                    List<Routine> updatedRoutines = new List<Routine>();
                    DateTime currentDate = startDate;

                    while (currentDate <= endDate)
                    {
                        updatedRoutines.Add(new Routine
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

                    // Aggiungi le nuove routine ricorrenti
                    _context.Routines.AddRange(updatedRoutines);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Routine con ID {id} modificata correttamente e occorrenze rigenerate.");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError($"Errore durante la modifica della routine: {ex.Message}");
                    if (!_context.Routines.Any(e => e.Id == routine.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(routine);
        }


        // GET: Routine/DeleteRoutine
        [HttpGet("Delete/{id:int}")]
        public async Task<IActionResult> DeleteRoutine(int id)
        {
            var routine = await _context.Routines.FindAsync(id);
            if (routine == null)
            {
                _logger.LogWarning($"Routine con ID {id} non trovata per l'eliminazione.");
                return NotFound();
            }

            return View(routine); // Passa il modello alla vista
        }


        // POST: Routine/DeleteRoutine
        [HttpPost, ActionName("DeleteRoutine")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var routine = await _context.Routines.FindAsync(id);
            if (routine != null)
            {
                try
                {
                    // Trova tutte le occorrenze della routine basate su UserId e Name
                    var recurringRoutines = _context.Routines
                        .Where(r => r.UserId == routine.UserId && r.Name == routine.Name)
                        .ToList();

                    // Rimuovi la routine principale e tutte le occorrenze
                    _context.Routines.RemoveRange(recurringRoutines);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Routine con ID {id} eliminata correttamente.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Errore durante l'eliminazione della routine: {ex.Message}");
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
