using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjeOgrenciYonetim.Web.Models;  
using ProjeOgrenciYonetim.Web.Data; // AppDbContext ve ProjectApplication burada ise
// Eğer AppDbContext başka namespace'te ise, ProjectsController.cs'deki using'i kopyala

namespace ProjeOgrenciYonetim.Web.Controllers
{
    [ApiController]
    [Route("api/Projects/applications")]
    public class ProjectApplicationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectApplicationsController(AppDbContext context)
        {
            _context = context;
        }

        // ================== BAŞVURUYU ONAYLA ==================
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var app = await _context.ProjectApplications
                .FirstOrDefaultAsync(a => a.Id == id);

            if (app == null)
                return NotFound();

            app.Status = ApplicationStatus.Approved; // 1
            await _context.SaveChangesAsync();

            return Ok();
        }

        // ================== BAŞVURUYU REDDET ==================
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            var app = await _context.ProjectApplications
                .FirstOrDefaultAsync(a => a.Id == id);

            if (app == null)
                return NotFound();

            app.Status = ApplicationStatus.Rejected; // 2
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
