using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjeOgrenciYonetim.Web.Data;
using ProjeOgrenciYonetim.Web.Models;

namespace ProjeOgrenciYonetim.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")] // 🔥 SADECE ADMIN GİRER
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        // =====================================================================
        //  TÜM ÖĞRENCİLERİ GETİR
        // =====================================================================
        [HttpGet("students")]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _db.Students
                                    .OrderBy(s => s.Status)
                                    .ToListAsync();

            return Ok(students);
        }

        // =====================================================================
        //  ÖĞRENCİ ONAYLA
        // =====================================================================
        [HttpPut("students/{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var student = await _db.Students.FindAsync(id);

            if (student == null)
                return NotFound("Öğrenci bulunamadı.");

            student.Status = StudentStatus.Approved;
            await _db.SaveChangesAsync();

            return Ok(student);
        }

        // =====================================================================
        //  ÖĞRENCİ REDDET
        // =====================================================================
        [HttpPut("students/{id}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            var student = await _db.Students.FindAsync(id);

            if (student == null)
                return NotFound("Öğrenci bulunamadı.");

            student.Status = StudentStatus.Rejected;
            await _db.SaveChangesAsync();

            return Ok(student);
        }
    }
}
