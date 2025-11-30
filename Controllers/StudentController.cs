using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjeOgrenciYonetim.Web.Data;
using ProjeOgrenciYonetim.Web.Models;

namespace ProjeOgrenciYonetim.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Student")]
public class StudentsController : ControllerBase
{
    private readonly AppDbContext _db;

    public StudentsController(AppDbContext db)
    {
        _db = db;
    }

    // ================================
    // GET: api/Students/profile
    // ================================
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var studentId = int.Parse(User.FindFirst("StudentId")!.Value);
        var student = await _db.Students.FindAsync(studentId);

        if (student == null)
            return NotFound();

        return Ok(student);
    }

    // ================================
    // POST: api/Students/apply-project
    // ================================
    [HttpPost("apply-project")]
    public async Task<IActionResult> ApplyProject([FromBody] int projectId)
    {
        var studentId = int.Parse(User.FindFirst("StudentId")!.Value);

        var application = new ProjectApplication
        {
            StudentId = studentId,
            ProjectId = projectId,
            ApplyDate = DateTime.UtcNow
        };

        _db.ProjectApplications.Add(application);
        await _db.SaveChangesAsync();

        return Ok("Başvuru alındı.");
    }

    // ================================
    // GET: api/Students/my-projects
    // ================================
    [HttpGet("my-projects")]
    public async Task<IActionResult> MyProjects()
    {
        var studentId = int.Parse(User.FindFirst("StudentId")!.Value);

        var list = await _db.ProjectApplications
            .Include(p => p.Project)
            .Where(p => p.StudentId == studentId)
            .ToListAsync();

        return Ok(list);
    }
}
