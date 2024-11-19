using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WomenActivity.Models;
using static WomenActivity.Models.TaskItem;

namespace WomenActivity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskItemsController : Controller
    {
        private readonly WomenActivityDbContext _context;

        public TaskItemsController(WomenActivityDbContext context)
        {
            _context = context;
        }

        // GET: TaskItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTaskItems(int userId)
        {
            return await _context.TaskItems.Where(t => t.UserId == userId).ToListAsync();
        }

        // GET: TaskItems
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTaskItem(int id)
        {
            var taskItem = await _context.TaskItems.FindAsync(id);

            if (taskItem == null)
            {
                return NotFound();
            }

            return taskItem;
        }

        // POST:TaskItems/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] TaskItem taskItem)
        {
            if (ModelState.IsValid)
            {
                _context.TaskItems.Add(taskItem);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "MainPage", new { userId = taskItem.UserId });
            }
            return BadRequest("Invalid task data.");
        }// PUT: TaskItems
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaskItem(int id, TaskItem taskItem)
        {
            if (id != taskItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(taskItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: TaskItems
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItem(int id)
        {
            var taskItem = await _context.TaskItems.FindAsync(id);
            if (taskItem == null)
            {
                return NotFound();
            }

            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskItemExists(int id)
        {
            return _context.TaskItems.Any(e => e.Id == id);
        }
    }
}