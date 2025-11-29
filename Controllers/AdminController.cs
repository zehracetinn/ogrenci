using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using ProjeOgrenciYonetim.Web.Data;
using ProjeOgrenciYonetim.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjeOgrenciYonetim.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        // ======================= ADMIN LOGIN =============================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Sabit admin hesabı (istersen sonraki adımda DB’ye alırız)
            if (username == "admin" && password == "1234")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Admin"),
                    new Claim("Role", "Admin")
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));

                return RedirectToAction("Students");
            }

            ViewBag.Error = "Admin kullanıcı adı veya şifre hatalı.";
            return View();
        }

        // ======================= STUDENT LIST =============================
        [HttpGet]
        public async Task<IActionResult> Students()
        {
            // Sadece Admin görebilsin
            if (!User.Claims.Any(c => c.Type == "Role" && c.Value == "Admin"))
                return Forbid();

            var students = await _db.Students
                .OrderBy(s => s.Status)
                .ToListAsync();

            return View(students);
        }

        // ======================= APPROVE =============================
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var s = await _db.Students.FindAsync(id);

            if (s == null)
                return NotFound();

            s.Status = StudentStatus.Approved;
            await _db.SaveChangesAsync();

            return RedirectToAction("Students");
        }

        // ======================= REJECT =============================
        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var s = await _db.Students.FindAsync(id);

            if (s == null)
                return NotFound();

            s.Status = StudentStatus.Rejected;
            await _db.SaveChangesAsync();

            return RedirectToAction("Students");
        }
    }
}
