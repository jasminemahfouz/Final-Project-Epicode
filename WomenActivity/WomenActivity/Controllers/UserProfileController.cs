
using Microsoft.AspNetCore.Mvc;
using WomenActivity.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace WomenActivity.Controllers
{

    public class UserProfileController : Controller
    {
        private readonly WomenActivityDbContext _context;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(WomenActivityDbContext context, ILogger<UserProfileController> logger)
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


        // Metodo GET per visualizzare il profilo utente per la modifica
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserId == userId.Value);
            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            var viewModel = new EditUserProfileViewModel
            {
                UserProfile = userProfile
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserProfileViewModel model)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var existingProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserId == userId.Value);
            if (existingProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Update the profile fields, including the ProfilePhoto
            existingProfile.Username = model.UserProfile.Username;
            existingProfile.Email = model.UserProfile.Email;
            existingProfile.Age = model.UserProfile.Age;
            existingProfile.Location = model.UserProfile.Location;
            existingProfile.ProfilePhoto = model.UserProfile.ProfilePhoto;

            await _context.SaveChangesAsync();

            // Immediately update session with the new profile photo path
            HttpContext.Session.SetString("ProfilePhoto", existingProfile.ProfilePhoto);

            return RedirectToAction("Index");
        }


        // Display user profile with related data (goals, memories, books)
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();

            if (userId == null)
            {
                _logger.LogInformation("UserId non trovato. Reindirizzamento alla pagina di login.");
                return RedirectToAction("Login", "Users");
            }

            var userProfile = await _context.UserProfiles
                .Include(u => u.Goals)
                .Include(u => u.Memories)
                .Include(u => u.BooksToRead)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (userProfile == null)
            {
                _logger.LogInformation("Profilo utente non trovato.");
                return NotFound("Profilo utente non trovato.");
            }

            _logger.LogInformation("Profilo utente trovato: " + userProfile.Username);

            ViewData["ProfilePhoto"] = string.IsNullOrEmpty(userProfile.ProfilePhoto)
        ? "/images/profiles/photo1.jpg" // Default image path
        : userProfile.ProfilePhoto;

            return View(userProfile);
        }

        // Display list of goals
        public async Task<IActionResult> Goals()
        {
            var userId = GetUserId(); // Assicurati che GetUserId() restituisca l'ID corretto
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Recupera il profilo utente corrispondente all'ID
            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(u => u.UserId == userId.Value);

            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            // Recupera tutti i goals associati al profilo utente
            var goals = await _context.Goals
                .Where(g => g.UserProfileId == userProfile.Id) // Filtra per UserProfileId
                .ToListAsync();

            return View(goals); // Passa i goals alla vista
        }
        // Render form for adding a new goal GET
        [HttpGet]
        public IActionResult AddGoal()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var userProfile = _context.UserProfiles.FirstOrDefault(u => u.UserId == userId.Value);
            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            var goal = new Goal
            {
                UserProfileId = userProfile.Id // Questo dovrebbe essere popolato correttamente
            };

            // Verifica che il valore sia impostato correttamente
            Console.WriteLine($"UserProfileId: {goal.UserProfileId}");

            return View(goal);
        }
        [HttpPost]
        public async Task<IActionResult> AddGoal(Goal goal)
        {
            var userId = GetUserId(); // Questo dovrebbe restituire il valore UserId = 5
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Recupera il profilo utente corretto basato su UserId
            var userProfile = _context.UserProfiles.FirstOrDefault(u => u.UserId == userId.Value);
            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            // Stampa il valore di UserProfileId per assicurarti che sia corretto
            Console.WriteLine($"UserProfileId: {userProfile.Id}");

            goal.UserProfileId = userProfile.Id; // Associa correttamente UserProfileId al Goal

            if (ModelState.IsValid)
            {
                _context.Goals.Add(goal);
                await _context.SaveChangesAsync();
                return RedirectToAction("Goals");
            }

            return View(goal);
        }
        [HttpGet]
        public async Task<IActionResult> EditGoal(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Recupera il profilo utente corrispondente al `UserId`
            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId.Value);

            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            // Ora usa l'`UserProfileId` per cercare il goal
            var goal = await _context.Goals.FirstOrDefaultAsync(g => g.Id == id && g.UserProfileId == userProfile.Id);

            if (goal == null)
            {
                Console.WriteLine("Goal non trovato o non appartiene all'utente corrente.");
                return NotFound("Goal non trovato.");
            }

            return View(goal);
        }
        [HttpPost]
        public async Task<IActionResult> EditGoal(Goal goal)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Recupera il profilo utente
            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId.Value);
            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            // Trova il goal esistente associato all'utente corrente
            var existingGoal = await _context.Goals.FirstOrDefaultAsync(g => g.Id == goal.Id && g.UserProfileId == userProfile.Id);

            if (existingGoal == null)
            {
                return NotFound("Goal non trovato.");
            }

            if (ModelState.IsValid)
            {
                // Aggiorna i dati del goal esistente
                existingGoal.Title = goal.Title;
                existingGoal.Description = goal.Description;

                // Salva le modifiche nel database
                await _context.SaveChangesAsync();

                return RedirectToAction("Goals");
            }

            // Se il ModelState non è valido, ritorna alla vista con i dati esistenti
            return View(goal);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteGoal(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Recupera il profilo utente
            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId.Value);
            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            // Trova il goal associato all'utente corrente
            var goal = await _context.Goals.FirstOrDefaultAsync(g => g.Id == id && g.UserProfileId == userProfile.Id);
            if (goal == null)
            {
                Console.WriteLine("Goal non trovato o non appartiene all'utente corrente.");
                return NotFound("Goal non trovato.");
            }

            return View(goal); // Restituisce la vista di conferma
        }

        [HttpPost, ActionName("DeleteGoal")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Recupera il profilo utente
            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId.Value);
            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            // Trova il goal associato all'utente corrente
            var goal = await _context.Goals.FirstOrDefaultAsync(g => g.Id == id && g.UserProfileId == userProfile.Id);
            if (goal == null)
            {
                Console.WriteLine("Goal non trovato o non appartiene all'utente corrente.");
                return NotFound("Goal non trovato.");
            }

            // Rimuovi il goal
            _context.Goals.Remove(goal);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante l'eliminazione: {ex.Message}");
                return NotFound("Si è verificato un errore durante l'eliminazione del goal.");
            }

            return RedirectToAction("Goals");
        }




        /////////// BBOOOKS
        // Display list of books
        // Display list of memories
        public async Task<IActionResult> Memories()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Recupera il profilo utente corrispondente all'ID
            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(u => u.UserId == userId.Value);

            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            // Recupera tutte le memories associate al profilo utente
            var memories = await _context.Memories
                .Where(m => m.UserProfileId == userProfile.Id) // Filtra per UserProfileId
                .ToListAsync();

            return View(memories); // Passa le memories alla vista
        }

        // Render form for adding a new memory GET
        [HttpGet]
        public IActionResult AddMemory()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var userProfile = _context.UserProfiles.FirstOrDefault(u => u.UserId == userId.Value);
            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            var memory = new Memory
            {
                UserProfileId = userProfile.Id // Associa correttamente l'ID profilo utente
            };

            return View(memory);
        }

        // Handle memory addition POST
        [HttpPost]
        public async Task<IActionResult> AddMemory(Memory memory)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Recupera il profilo utente basato su UserId
            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserId == userId.Value);
            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            // Assegna il corretto UserProfileId al memory
            memory.UserProfileId = userProfile.Id;

            // Verifica che il ModelState sia valido
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState non valido");
                return View(memory);
            }

            try
            {
                _context.Memories.Add(memory);
                await _context.SaveChangesAsync();
                return RedirectToAction("Memories");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante il salvataggio: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Si è verificato un errore durante il salvataggio. Riprova.");
                return View(memory);
            }
        }


        // Edit an existing memory GET
        [HttpGet]
        public async Task<IActionResult> EditMemory(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId.Value);
            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            var memory = await _context.Memories.FirstOrDefaultAsync(m => m.Id == id && m.UserProfileId == userProfile.Id);
            if (memory == null)
            {
                return NotFound("Memory non trovata.");
            }

            return View(memory);
        }

        // Handle memory editing POST
        [HttpPost]
        public async Task<IActionResult> EditMemory(Memory memory)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId.Value);
            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            var existingMemory = await _context.Memories.FirstOrDefaultAsync(m => m.Id == memory.Id && m.UserProfileId == userProfile.Id);
            if (existingMemory == null)
            {
                return NotFound("Memory non trovata.");
            }

            if (ModelState.IsValid)
            {
                existingMemory.Title = memory.Title;
                existingMemory.Description = memory.Description;
                existingMemory.Date = memory.Date;
                existingMemory.ImageUrl = memory.ImageUrl;

                await _context.SaveChangesAsync();
                return RedirectToAction("Memories");
            }

            return View(memory);
        }

        // Delete a memory GET
        [HttpGet]
        public async Task<IActionResult> DeleteMemory(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId.Value);
            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            var memory = await _context.Memories.FirstOrDefaultAsync(m => m.Id == id && m.UserProfileId == userProfile.Id);
            if (memory == null)
            {
                return NotFound("Memory non trovata.");
            }

            return View(memory); // Restituisce la vista di conferma
        }

        // Handle memory deletion POST
        [HttpPost, ActionName("DeleteMemory")]
        public async Task<IActionResult> DeleteMemoryConfirmed(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId.Value);
            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            var memory = await _context.Memories.FirstOrDefaultAsync(m => m.Id == id && m.UserProfileId == userProfile.Id);
            if (memory == null)
            {
                return NotFound("Memory non trovata.");
            }

            _context.Memories.Remove(memory);
            await _context.SaveChangesAsync();

            return RedirectToAction("Memories");
        }



        // --- LISTA DI LIBRI ---
        public async Task<IActionResult> Books()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserId == userId.Value);
            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            var books = await _context.BooksToRead
                .Where(b => b.UserProfileId == userProfile.Id)
                .ToListAsync();

            return View(books);
        }

        // --- AGGIUNGI LIBRO (GET) ---
        [HttpGet]
        public IActionResult AddBook()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var userProfile = _context.UserProfiles.FirstOrDefault(u => u.UserId == userId.Value);
            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            var book = new Book
            {
                UserProfileId = userProfile.Id
            };

            return View(book);
        }

        // --- AGGIUNGI LIBRO (POST) ---
        [HttpPost]
        public async Task<IActionResult> AddBook(Book book)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var userProfile = _context.UserProfiles.FirstOrDefault(u => u.UserId == userId.Value);
            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            book.UserProfileId = userProfile.Id;

            if (ModelState.IsValid)
            {
                _context.BooksToRead.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction("Books");
            }

            return View(book);
        }

        // --- MODIFICA LIBRO (GET) ---
        [HttpGet]
        public async Task<IActionResult> EditBook(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserId == userId.Value);
            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            var book = await _context.BooksToRead.FirstOrDefaultAsync(b => b.Id == id && b.UserProfileId == userProfile.Id);
            if (book == null)
            {
                return NotFound("Libro non trovato.");
            }

            return View(book);
        }

        // --- MODIFICA LIBRO (POST) ---
        [HttpPost]
        public async Task<IActionResult> EditBook(Book book)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserId == userId.Value);
            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            var existingBook = await _context.BooksToRead.FirstOrDefaultAsync(b => b.Id == book.Id && b.UserProfileId == userProfile.Id);
            if (existingBook == null)
            {
                return NotFound("Libro non trovato.");
            }

            if (ModelState.IsValid)
            {
                existingBook.Title = book.Title;
                existingBook.Author = book.Author;
                existingBook.IsRead = book.IsRead;

                await _context.SaveChangesAsync();
                return RedirectToAction("Books");
            }

            return View(book);
        }

        // --- CANCELLA LIBRO (GET) ---
        [HttpGet]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserId == userId.Value);
            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            var book = await _context.BooksToRead.FirstOrDefaultAsync(b => b.Id == id && b.UserProfileId == userProfile.Id);
            if (book == null)
            {
                return NotFound("Libro non trovato.");
            }

            return View(book);
        }

        // --- CANCELLA LIBRO (POST) ---
        [HttpPost, ActionName("DeleteBook")]
        public async Task<IActionResult> DeleteBookConfirmed(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserId == userId.Value);
            if (userProfile == null)
            {
                return NotFound("Profilo utente non trovato.");
            }

            var book = await _context.BooksToRead.FirstOrDefaultAsync(b => b.Id == id && b.UserProfileId == userProfile.Id);
            if (book == null)
            {
                return NotFound("Libro non trovato.");
            }

            _context.BooksToRead.Remove(book);
            await _context.SaveChangesAsync();

            return RedirectToAction("Books");
        }

     
    }
}
