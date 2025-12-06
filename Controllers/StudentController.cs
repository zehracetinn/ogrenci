using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjeOgrenciYonetim.Web.Data;
using ProjeOgrenciYonetim.Web.Models;
using ProjeOgrenciYonetim.Web.Dto;
using System.Security.Claims;

namespace ProjeOgrenciYonetim.Web.Controllers
{
    [ApiController]
    [Route("api/student")]
    [Authorize(Roles = "student")]   // TÜM endpointler öğrenci rolü gerektirir
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public StudentsController(AppDbContext db)
        {
            _db = db;
        }

        // ================================
        // GET: api/student/profile
        // ================================
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var studentIdClaim = User.FindFirst("studentId");
            if (studentIdClaim == null)
                return Unauthorized("Token içinde 'studentId' claim'i bulunamadı.");

            var studentId = int.Parse(studentIdClaim.Value);

            var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
                return NotFound("Öğrenci bulunamadı.");

            return Ok(student);
        }

        // ================================
        // POST: api/student/apply-project
        // ================================
       [HttpPost("apply-project")]
public async Task<IActionResult> ApplyProject([FromBody] ProjectApplyDto dto)
{
    var studentId = int.Parse(User.FindFirst("studentId").Value);

    // 1) Öğrencinin profil durumu onaylanmış mı?
    var student = await _db.Students.FindAsync(studentId);
    if (student == null)
        return Unauthorized("Öğrenci bulunamadı.");

    if (student.Status != StudentStatus.Approved)
        return BadRequest("Hesabınız onaylanmadan proje başvurusu yapamazsınız.");

    // 2) LIMIT KONTROLÜ (MAX 3)
    var activeCount = await _db.ProjectApplications
        .Where(a => a.StudentId == studentId)
        .CountAsync();

    if (activeCount >= 3)
        return BadRequest("Aynı anda en fazla 3 projeye başvuru yapabilirsiniz.");

    // 3) Aynı projeye zaten başvurmuş mu?
    var exists = await _db.ProjectApplications
        .AnyAsync(a => a.StudentId == studentId && a.ProjectId == dto.ProjectId);

    if (exists)
        return BadRequest("Bu projeye zaten başvurdunuz.");

    // 4) Başvuruyu kaydet
    var app = new ProjectApplication
    {
        StudentId = studentId,
        ProjectId = dto.ProjectId,
        ApplyDate = DateTime.UtcNow,
        Status = ApplicationStatus.Pending
    };

    _db.ProjectApplications.Add(app);
    await _db.SaveChangesAsync();

    return Ok("Başvuru başarıyla gönderildi.");
}


        // ================================
        // GET: api/student/applications
        // Öğrencinin başvurduğu projeleri DTO ile döner
        // ================================
        [HttpGet("applications")]
        public async Task<IActionResult> GetMyApplications()
        {
            var studentId = int.Parse(User.FindFirst("studentId").Value);

            var apps = await _db.ProjectApplications
                .Include(p => p.Project)
                .Where(p => p.StudentId == studentId)
                .ToListAsync();

            var dto = apps.Select(a => new
            {
                id = a.Id,
                status = a.Status.ToString(),
                applyDate = a.ApplyDate,
                project = new
                {
                    id = a.Project.Id,
                    name = a.Project.Name,
                    description = a.Project.Description,
                    technologies = a.Project.Technologies,
                    durationWeeks = a.Project.DurationWeeks
                }
            });

            return Ok(dto);
        }

        // ================================
// GET: api/Students/my-applications
// Öğrencinin başvurduğu projelerin ID listesini döner
// ================================
[HttpGet("my-applications")]
[Authorize(Roles = "student")]
public async Task<IActionResult> GetMyApplicationIds()
{
    var studentIdClaim = User.FindFirst("studentId");
    if (studentIdClaim == null)
        return Unauthorized();

    int studentId = int.Parse(studentIdClaim.Value);

    var appliedProjectIds = await _db.ProjectApplications
        .Where(a => a.StudentId == studentId)
        .Select(a => a.ProjectId)
        .ToListAsync();

    return Ok(appliedProjectIds);
}



        

        // ================================
        // GET: api/student/my-projects
        // ================================
        [HttpGet("my-projects")]
        public async Task<IActionResult> MyProjects()
        {
            var studentId = int.Parse(User.FindFirst("studentId").Value);

            var list = await _db.ProjectApplications
                .Include(p => p.Project)
                .Where(p => p.StudentId == studentId)
                .ToListAsync();

            return Ok(list);
        }
    }
}
