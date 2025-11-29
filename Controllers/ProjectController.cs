using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjeOgrenciYonetim.Web.Data;
using ProjeOgrenciYonetim.Web.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Distributed;

namespace ProjeOgrenciYonetim.Web.Controllers
{
    public class ProjectController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IDistributedCache _cache;

        public ProjectController(AppDbContext db, IDistributedCache cache)
        {
            _db = db;
            _cache = cache;
        }

        private bool IsAdmin()
        {
            return User.Claims.Any(c => c.Type == "Role" && c.Value == "Admin");
        }

        // ==================== ADMIN PROJECT LIST =====================
        public async Task<IActionResult> AdminList()
        {
            if (!IsAdmin()) return Forbid();

            var projects = await _db.Projects.ToListAsync();
            return View(projects);
        }

        // ==================== CREATE =====================
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin()) return Forbid();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Project p)
        {
            if (!IsAdmin()) return Forbid();

            _db.Projects.Add(p);
            await _db.SaveChangesAsync();

            // Redis Cache invalidate
            await _cache.RemoveAsync("projects_all");

            return RedirectToAction("AdminList");
        }

        // ==================== EDIT =====================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin()) return Forbid();
            var p = await _db.Projects.FindAsync(id);
            return View(p);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Project p)
        {
            if (!IsAdmin()) return Forbid();

            _db.Projects.Update(p);
            await _db.SaveChangesAsync();

            // Redis Cache invalidate
            await _cache.RemoveAsync("projects_all");

            return RedirectToAction("AdminList");
        }

        // ==================== DELETE =====================
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin()) return Forbid();

            var p = await _db.Projects.FindAsync(id);
            _db.Projects.Remove(p);

            await _db.SaveChangesAsync();

            // Redis Cache invalidate
            await _cache.RemoveAsync("projects_all");

            return RedirectToAction("AdminList");
        }

        // ==================== STUDENT PROJECT LIST (REDIS CACHE) =====================
        public async Task<IActionResult> List()
        {
            string cacheKey = "projects_all";
            List<Project> projects;

            var cacheData = await _cache.GetAsync(cacheKey);

            if (cacheData != null)
            {
                projects = System.Text.Json.JsonSerializer.Deserialize<List<Project>>(cacheData);
            }
            else
            {
                projects = await _db.Projects.ToListAsync();

                var bytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(projects);

                await _cache.SetAsync(
                    cacheKey,
                    bytes,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                    }
                );
            }

            return View(projects);
        }

        // ==================== APPLY (REDIS MAX 3 + BLOCK REAPPLY) =====================
        [HttpPost]
        public async Task<IActionResult> Apply(int projectId)
        {
            var studentIdClaim = User.Claims.FirstOrDefault(c => c.Type == "StudentId");
            if (studentIdClaim == null) return RedirectToAction("Login", "Student");

            int studentId = int.Parse(studentIdClaim.Value);

            string countKey = $"student:{studentId}:application_count";
            string projectKey = $"student:{studentId}:project:{projectId}";

            // REDIS: Başvuru sayısını al
            int currentCount = 0;

            var countBytes = await _cache.GetAsync(countKey);
            if (countBytes != null)
            {
                currentCount = BitConverter.ToInt32(countBytes);
            }
            else
            {
                currentCount = await _db.ProjectApplications.CountAsync(a => a.StudentId == studentId);
                await _cache.SetAsync(countKey, BitConverter.GetBytes(currentCount));
            }

            // Max 3 hakkı geçti mi?
            if (currentCount >= 3)
            {
                TempData["Error"] = "En fazla 3 projeye başvurabilirsiniz.";
                return RedirectToAction("List");
            }

            // REDIS: Aynı projeye daha önce başvurmuş mu?
            var existsRedis = await _cache.GetStringAsync(projectKey);
            if (existsRedis == "1")
            {
                TempData["Error"] = "Bu projeye daha önce başvurdunuz.";
                return RedirectToAction("List");
            }

            // DB fallback check
            bool existsDb = await _db.ProjectApplications
                .AnyAsync(a => a.ProjectId == projectId && a.StudentId == studentId);

            if (existsDb)
            {
                await _cache.SetStringAsync(projectKey, "1");
                TempData["Error"] = "Bu projeye daha önce başvurdunuz.";
                return RedirectToAction("List");
            }

            // Başvuru ekle
            var app = new ProjectApplication
            {
                StudentId = studentId,
                ProjectId = projectId
            };

            _db.ProjectApplications.Add(app);
            await _db.SaveChangesAsync();

            // REDIS: başvuru count +1
            currentCount++;
            await _cache.SetAsync(countKey, BitConverter.GetBytes(currentCount));

            // REDIS: aynı projeye tekrar başvuramasın
            await _cache.SetStringAsync(projectKey, "1");

            TempData["Success"] = "Başvurunuz alındı!";
            return RedirectToAction("List");
        }

        // ==================== MY PROJECTS =====================
        public async Task<IActionResult> MyProjects()
        {
            var studentIdClaim = User.Claims.FirstOrDefault(c => c.Type == "StudentId");
            if (studentIdClaim == null) return RedirectToAction("Login", "Student");

            int studentId = int.Parse(studentIdClaim.Value);

            var apps = await _db.ProjectApplications
                .Include(a => a.Project)
                .Where(a => a.StudentId == studentId)
                .ToListAsync();

            return View(apps);
        }
    }
}
