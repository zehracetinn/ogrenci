using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using ProjeOgrenciYonetim.Web.Data;
using ProjeOgrenciYonetim.Web.Models;

namespace ProjeOgrenciYonetim.Web.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _db;

        public StudentController(AppDbContext db)
        {
            _db = db;
        }

        // =================== REGISTER ==========================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Student student)
        {
            if (!ModelState.IsValid)
                return View(student);

            // Varsayılan durum Pending olacak
            student.Status = StudentStatus.Pending;

            _db.Students.Add(student);
            await _db.SaveChangesAsync();

            ViewBag.Message = "Kaydınız alındı. Admin onayından sonra giriş yapabileceksiniz.";
            return View();
        }

        // ==================== LOGIN ============================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var student = await _db.Students
                .FirstOrDefaultAsync(s => s.Email == email && s.Password == password);

            if (student == null)
            {
                ViewBag.Error = "Email veya şifre hatalı";
                return View();
            }

            if (student.Status != StudentStatus.Approved)
            {
                ViewBag.Error = "Hesabınız admin onayında.";
                return View();
            }

            // Cookie Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, student.FullName),
                new Claim("StudentId", student.Id.ToString()),
                new Claim("Role", "Student")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
