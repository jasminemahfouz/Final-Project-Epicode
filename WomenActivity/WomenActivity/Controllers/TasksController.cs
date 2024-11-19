using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WomenActivity.Models;

namespace WomenActivity.Controllers
{
   
    [Authorize]  // Assicura che solo utenti autenticati possano accedere
    public class TasksController : Controller
    {
        private readonly WomenActivityDbContext _context;

        public TasksController(WomenActivityDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string color = null, string type = null, DateTime? time = null)
        {
            var userId = User.FindFirstValue("UserId");
            if (userId == null)
            {
                return Unauthorized();
            }

            // Query di base per ottenere le task dell'utente autenticato
            var tasksQuery = _context.TaskItems
                .Where(t => t.UserId == int.Parse(userId))
                .AsQueryable();

            // Applicazione dei filtri
            if (!string.IsNullOrEmpty(color))
            {
                tasksQuery = tasksQuery.Where(t => t.Color == color);
            }

            if (!string.IsNullOrEmpty(type))
            {
                tasksQuery = tasksQuery.Where(t => t.Type == type);
            }

            if (time.HasValue)
            {
                tasksQuery = tasksQuery.Where(t => t.Time.Date == time.Value.Date);
            }

            var tasks = await tasksQuery.ToListAsync();

            // Popoliamo i ViewBag con i valori unici per i dropdown (Color e Type)
            ViewBag.Colors = await _context.TaskItems
                .Where(t => t.UserId == int.Parse(userId))
                .Select(t => t.Color)
                .Distinct()
                .ToListAsync();

            ViewBag.Types = await _context.TaskItems
                .Where(t => t.UserId == int.Parse(userId))
                .Select(t => t.Type)
                .Distinct()
                .ToListAsync();

            return View(tasks);
        }
    
    // GET: Tasks
   

        // GET: Tasks/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Time,Color,Type")] TaskItem task)
        {
            var userId = User.FindFirstValue("UserId");
            if (userId == null)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                // Associa la task all'utente autenticato
                task.UserId = int.Parse(userId);
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(task);
        }

        // GET: Tasks/Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue("UserId");
            if (userId == null)
            {
                return Unauthorized();
            }

            // Trova la task che appartiene all'utente autenticato
            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == int.Parse(userId));

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Time,Color,Type")] TaskItem task)
        {
            var userId = User.FindFirstValue("UserId");
            if (userId == null)
            {
                return Unauthorized();
            }

            if (id != task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verifica che la task appartenga all'utente prima di modificarla
                    var existingTask = await _context.TaskItems
                        .FirstOrDefaultAsync(t => t.Id == id && t.UserId == int.Parse(userId));

                    if (existingTask == null)
                    {
                        return NotFound();
                    }

                    // Aggiorna la task
                    existingTask.Name = task.Name;
                    existingTask.Description = task.Description;
                    existingTask.Time = task.Time;
                    existingTask.Color = task.Color;
                    existingTask.Type = task.Type;

                    _context.Update(existingTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskItemExists(task.Id))
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
            return View(task);
        }

        // GET: Tasks/Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue("UserId");
            if (userId == null)
            {
                return Unauthorized();
            }

            // Trova la task che appartiene all'utente autenticato
            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == int.Parse(userId));

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue("UserId");
            if (userId == null)
            {
                return Unauthorized();
            }

            // Trova la task che appartiene all'utente autenticato
            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == int.Parse(userId));

            if (task == null)
            {
                return NotFound();
            }

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskItemExists(int id)
        {
            return _context.TaskItems.Any(e => e.Id == id);
        }
    }
}
