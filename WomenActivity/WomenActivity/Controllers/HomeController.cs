using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WomenActivity.Models;

namespace WomenActivity.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // If the user is authenticated, redirect them to the MainPage
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "MainPage");
        }

        // Otherwise, show the home page for unauthenticated users
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult AboutMe()
    {
        return View();
    }




}

