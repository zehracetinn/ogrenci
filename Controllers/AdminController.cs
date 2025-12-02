using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjeOgrenciYonetim.Web.Data;
using ProjeOgrenciYonetim.Web.Models;

namespace ProjeOgrenciYonetim.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    // 🔥 TÜM METHODLAR SADECE ADMIN TOKEN İLE ÇALIŞIR
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        // ======================= Tüm Öğrenciler =============================
        [HttpGet("students")]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _db.Students
                .OrderBy(s => s.Status)
                .ToListAsync();

            return Ok(students);
        }

        // ======================= Onayla =============================
        [HttpPut("students/{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var s = await _db.Students.FindAsync(id);
            if (s == null) return NotFound();

            s.Status = StudentStatus.Approved;
            await _db.SaveChangesAsync();

            return Ok(s);
        }

        // ======================= Reddet =============================
        [HttpPut("students/{id}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            var s = await _db.Students.FindAsync(id);
            if (s == null) return NotFound();

            s.Status = StudentStatus.Rejected;
            await _db.SaveChangesAsync();

            return Ok(s);
        }
    }
}
