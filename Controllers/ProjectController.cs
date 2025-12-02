using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjeOgrenciYonetim.Web.Data;
using ProjeOgrenciYonetim.Web.Models;

namespace ProjeOgrenciYonetim.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProjectsController(AppDbContext db)
    {
        _db = db;
    }

    // ============================================================
    // =====================  ADMIN  ===============================
    // ============================================================

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.Projects.ToListAsync();
        return Ok(list);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create(Project p)
    {
        _db.Projects.Add(p);
        await _db.SaveChangesAsync();
        return Ok(p);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, Project p)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project == null) return NotFound();

        project.Name = p.Name;
        project.Description = p.Description;
        project.DurationWeeks = p.DurationWeeks;
        project.Technologies = p.Technologies;

        await _db.SaveChangesAsync();
        return Ok(project);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project == null) return NotFound();

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
        return Ok("Proje silindi.");
    }

    [HttpGet("{id}/applicants")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Applicants(int id)
    {
        var applicants = await _db.ProjectApplications
            .Where(a => a.ProjectId == id)
            .Include(a => a.Student)
            .Select(a => new
            {
                a.Student.FullName,
                a.Student.Email,
                a.Student.StudentNumber,
                a.AppliedAt
            })
            .ToListAsync();

        return Ok(applicants);
    }

    // ============================================================
    // =====================  STUDENT  ============================
    // ============================================================

    [HttpGet("list")]
    [Authorize(Roles = "student")]
    public async Task<IActionResult> ListForStudents()
    {
        var list = await _db.Projects.ToListAsync();
        return Ok(list);
    }

    [HttpPost("apply")]
    [Authorize(Roles = "student")]
    public async Task<IActionResult> Apply([FromQuery] int projectId)
    {
        var studentId = int.Parse(User.FindFirst("studentId")!.Value);

        // Aynı projeye tekrar başvuru engelle
        bool alreadyApplied = await _db.ProjectApplications
            .AnyAsync(x => x.ProjectId == projectId && x.StudentId == studentId);

        if (alreadyApplied)
            return BadRequest("Bu projeye zaten başvurdunuz.");

        // Max 3 başvuru kontrolü
        int count = await _db.ProjectApplications
            .CountAsync(x => x.StudentId == studentId);

        if (count >= 3)
            return BadRequest("En fazla 3 projeye başvurabilirsiniz.");

        var app = new ProjectApplication
        {
            StudentId = studentId,
            ProjectId = projectId,
            AppliedAt = DateTime.UtcNow
        };

        _db.ProjectApplications.Add(app);
        await _db.SaveChangesAsync();

        return Ok("Başvurunuz alındı.");
    }

    [HttpGet("my")]
    [Authorize(Roles = "student")]
    public async Task<IActionResult> MyProjects()
    {
        var studentId = int.Parse(User.FindFirst("studentId")!.Value);

        var list = await _db.ProjectApplications
            .Where(a => a.StudentId == studentId)
            .Include(a => a.Project)
            .Select(a => new
            {
                a.Project.Name,
                a.Project.Description,
                a.Project.DurationWeeks,
                a.Project.Technologies,
                a.AppliedAt
            })
            .ToListAsync();

        return Ok(list);
    }
}
