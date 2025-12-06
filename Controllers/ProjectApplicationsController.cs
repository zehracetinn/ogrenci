using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjeOgrenciYonetim.Web.Models;
using ProjeOgrenciYonetim.Web.Data;
using ProjeOgrenciYonetim.Web.Dto;
using System.Threading.Tasks;
using System.Linq;


namespace ProjeOgrenciYonetim.Web.Controllers
{
    [ApiController]
    [Route("api/Projects/applications")]   // ✔ FRONTEND ile TAM uyumlu
    public class ProjectApplicationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectApplicationsController(AppDbContext context)
        {
            _context = context;
        }

        // ================================
        // GET: api/Projects/applications/{projectId}
        // ================================

[HttpGet("{projectId}")]
public async Task<IActionResult> GetApplicants(int projectId)
{
    var apps = await _context.ProjectApplications
        .Include(a => a.Student)
        .Where(a => a.ProjectId == projectId)
        .ToListAsync();

    var dto = apps.Select(a => new ApplicantDto
    {
        Id = a.Id,
        Status = a.Status.ToString(),
        ApplyDate = a.ApplyDate.ToString("yyyy-MM-dd"),

        Student = new StudentDto
        {
            Id = a.Student.Id,
            FullName = a.Student.FullName,
            Email = a.Student.Email,
            KnownTechnologies = a.Student.KnownTechnologies,
            Status = a.Student.Status.ToString()
        }
    });

    return Ok(dto);
}



        // ================================
        // PUT: api/Projects/applications/{id}/approve
        // ================================
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var app = await _context.ProjectApplications
                .FirstOrDefaultAsync(a => a.Id == id);

            if (app == null)
                return NotFound("Başvuru bulunamadı.");

            app.Status = ApplicationStatus.Approved;
            await _context.SaveChangesAsync();

            return Ok("Başvuru onaylandı.");
        }

        // ================================
        // PUT: api/Projects/applications/{id}/reject
        // ================================
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            var app = await _context.ProjectApplications
                .FirstOrDefaultAsync(a => a.Id == id);

            if (app == null)
                return NotFound("Başvuru bulunamadı.");

            app.Status = ApplicationStatus.Rejected;
            await _context.SaveChangesAsync();

            return Ok("Başvuru reddedildi.");
        }
    }
}
