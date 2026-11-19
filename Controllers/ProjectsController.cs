using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using portfolio.Data;
using portfolio.Models;

namespace Portfolio.Controllers;

public class ProjectsController : Controller
{
    private readonly ProjectRepository _projects;
    private readonly CourseRepository _courses;
    public ProjectsController(ProjectRepository projects, CourseRepository courses)
    { _projects = projects; _courses = courses; }

    public async Task<IActionResult> Index() => View(await _projects.GetAllAsync());

    public async Task<IActionResult> Create()
    {
        await LoadCourses();
        return View(new Project());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Project project)
    {
        if (!ModelState.IsValid) { await LoadCourses(project.CourseId); return View(project); }
        try
        {
            await _projects.AddWithImagesAsync(project, SplitUrls(project.ImageUrls));
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            await LoadCourses(project.CourseId);
            return View(project);
        }
    }

    public async Task<IActionResult> Edit(long id)
    {
        var project = await _projects.GetByIdAsync(id);
        if (project is null) return NotFound();
        project.ImageUrls = string.Join(Environment.NewLine, project.Images.Select(i => i.PublicUrl));
        await LoadCourses(project.CourseId);
        return View(project);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, Project project)
    {
        if (id != project.ProjectId) return BadRequest();
        if (!ModelState.IsValid) { await LoadCourses(project.CourseId); return View(project); }
        try
        {
            await _projects.UpdateWithImagesAsync(project, SplitUrls(project.ImageUrls));
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            await LoadCourses(project.CourseId);
            return View(project);
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
        await _projects.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadCourses(long? selected = null)
    {
        ViewBag.Courses = new SelectList(await _courses.GetAllAsync(), "CourseId", "Code", selected);
    }

    private static List<string> SplitUrls(string? urls) =>
        (urls ?? "").Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim()).Where(x => x.Length > 0).ToList();
}
