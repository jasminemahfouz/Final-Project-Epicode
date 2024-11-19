using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WomenActivity.Models;

namespace WomenActivity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TypeController : Controller
    {
        private readonly WomenActivityDbContext _context;

        public TypeController(WomenActivityDbContext context)
        {
            _context = context;
        }

        // GET: Type
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var taskTypes = await _context.TaskItems
                .Select(t => t.Type)
                .Distinct()
                .ToListAsync();

            return View(taskTypes);
        }

        // GET: Type/TasksByType
        [HttpGet("TasksByType")]
        public async Task<IActionResult> TasksByType(string type)
        {
            var tasks = await _context.TaskItems
                .Where(t => t.Type == type)
                .ToListAsync();

            return View(tasks);
        }
    }
}
