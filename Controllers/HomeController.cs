using Microsoft.AspNetCore.Mvc;
using Portfolio.Models;
using portfolio.Data;
using System.Diagnostics;

namespace Portfolio.Controllers;

public class HomeController : Controller
{
    private readonly ProjectRepository _projects;
    private readonly CourseRepository _courses;
    public HomeController(ProjectRepository projects, CourseRepository courses)
    { _projects = projects; _courses = courses; }

    public async Task<IActionResult> Index()
    {
        ViewBag.CourseCount = (await _courses.GetAllAsync()).Count;
        return View(await _projects.GetAllAsync());
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
