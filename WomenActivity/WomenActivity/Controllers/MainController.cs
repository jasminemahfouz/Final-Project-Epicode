using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using WomenActivity.Models;

namespace WomenActivity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MainPageController : Controller
    {
        private readonly WomenActivityDbContext _context;

        public MainPageController(WomenActivityDbContext context)
        {
            _context = context;
        }

        // GET: MainPage
        [HttpGet("Index")]
        public async Task<IActionResult> Index(int year = 0, int month = 0)
        {
            // Ensure user is logged in and get user ID
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Users");
            }
            var userId = int.Parse(User.FindFirst("UserId").Value);

            var currentYear = year == 0 ? DateTime.Today.Year : year;
            var currentMonth = month == 0 ? DateTime.Today.Month : month;

            // Get tasks for today
            var tasksForToday = await _context.TaskItems
                .Where(t => t.UserId == userId && t.Time.Date == DateTime.Today)
                .ToListAsync();

            // Get task counts per day for the calendar
            var tasksPerDay = await _context.TaskItems
                .Where(t => t.UserId == userId)
                .GroupBy(t => t.Time.Date)
                .Select(g => new { Date = g.Key, TaskCount = g.Count() })
                .ToDictionaryAsync(g => g.Date, g => g.TaskCount);

            // Initialize the view model
            var viewModel = new HomePageViewModel
            {
                CurrentDate = DateTime.Today,
                TaskItems = tasksForToday,
                ShowCalendar = true,
                Calendar = new CalendarViewModel
                {
                    CurrentYear = currentYear,
                    CurrentMonth = currentMonth,
                    DaysInMonth = DateTime.DaysInMonth(currentYear, currentMonth)
                },
                TasksPerDay = tasksPerDay
            };

            return View(viewModel);
        }

        // GET: MainPage/GetTasksForDate
        [HttpGet("GetTasksForDate/{date}")]
        public async Task<IActionResult> GetTasksForDate(DateTime date)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Users");
            }
            var userId = int.Parse(User.FindFirst("UserId").Value);

            var tasksForDate = await _context.TaskItems
                .Where(t => t.UserId == userId && t.Time.Date == date.Date)
                .ToListAsync();

            var tasksPerDay = await _context.TaskItems
                .Where(t => t.UserId == userId)
                .GroupBy(t => t.Time.Date)
                .Select(g => new { Date = g.Key, TaskCount = g.Count() })
                .ToDictionaryAsync(g => g.Date, g => g.TaskCount);

            var viewModel = new HomePageViewModel
            {
                CurrentDate = date,
                TaskItems = tasksForDate,
                ShowCalendar = true,
                Calendar = new CalendarViewModel
                {
                    CurrentYear = date.Year,
                    CurrentMonth = date.Month,
                    DaysInMonth = DateTime.DaysInMonth(date.Year, date.Month)
                },
                TasksPerDay = tasksPerDay
            };

            return View("Index", viewModel);
        }

        // GET: MainPage/ChangeCalendarDate
        [HttpGet("ChangeCalendarDate/{year:int}/{month:int}")]
        public async Task<IActionResult> ChangeCalendarDate(int year, int month)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Users");
            }

            var userId = int.Parse(User.FindFirst("UserId").Value);

            // Adjust year and month if the next/previous month goes out of range
            if (month > 12)
            {
                month = 1;
                year += 1;
            }
            else if (month < 1)
            {
                month = 12;
                year -= 1;
            }

            var tasksPerDay = await _context.TaskItems
                .Where(t => t.UserId == userId)
                .GroupBy(t => t.Time.Date)
                .Select(g => new { Date = g.Key, TaskCount = g.Count() })
                .ToDictionaryAsync(g => g.Date, g => g.TaskCount);

            var viewModel = new HomePageViewModel
            {
                CurrentDate = new DateTime(year, month, 1),
                TaskItems = new List<TaskItem>(),
                ShowCalendar = true,
                Calendar = new CalendarViewModel
                {
                    CurrentYear = year,
                    CurrentMonth = month,
                    DaysInMonth = DateTime.DaysInMonth(year, month)
                },
                TasksPerDay = tasksPerDay
            };

            return View("Index", viewModel);
        }

    }
}
