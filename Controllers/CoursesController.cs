using Microsoft.AspNetCore.Mvc;
using portfolio.Data;
using portfolio.Models;

namespace Portfolio.Controllers;

public class CoursesController : Controller
{
    private readonly CourseRepository _courses;
    public CoursesController(CourseRepository courses) => _courses = courses;

    public async Task<IActionResult> Index() => View(await _courses.GetAllAsync());

    public IActionResult Create() => View(new Course());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Course course)
    {
        if (!ModelState.IsValid) return View(course);
        try { await _courses.AddAsync(course); return RedirectToAction(nameof(Index)); }
        catch (Exception ex) { ModelState.AddModelError("", ex.Message); return View(course); }
    }

    public async Task<IActionResult> Edit(long id)
    {
        var course = await _courses.GetByIdAsync(id);
        return course is null ? NotFound() : View(course);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, Course course)
    {
        if (id != course.CourseId) return BadRequest();
        if (!ModelState.IsValid) return View(course);
        try { await _courses.UpdateAsync(course); return RedirectToAction(nameof(Index)); }
        catch (Exception ex) { ModelState.AddModelError("", ex.Message); return View(course); }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
        try { await _courses.DeleteAsync(id); }
        catch (Exception ex) { TempData["Error"] = "Cannot delete this course. It may still have projects. " + ex.Message; }
        return RedirectToAction(nameof(Index));
    }
}
