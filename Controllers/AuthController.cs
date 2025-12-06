using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjeOgrenciYonetim.Web.Data;
using ProjeOgrenciYonetim.Web.Dtos;
using ProjeOgrenciYonetim.Web.Models;
using ProjeOgrenciYonetim.Web.Services;

namespace ProjeOgrenciYonetim.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;

    public AuthController(AppDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    // ============================
    // STUDENT REGISTER
    // ============================
    [HttpPost("student-register")]
    public async Task<IActionResult> StudentRegister(StudentRegisterDto dto)
    {
        if (await _db.Students.AnyAsync(s => s.Email == dto.Email))
            return BadRequest("Email zaten kullanılıyor.");

        if (await _db.Students.AnyAsync(s => s.StudentNumber == dto.StudentNumber))
            return BadRequest("Öğrenci numarası zaten kullanılıyor.");

        var student = new Student
        {
            FullName = dto.FullName,
            Email = dto.Email,
            StudentNumber = dto.StudentNumber,
            KnownTechnologies = dto.KnownTechnologies,
            PasswordHash = dto.Password, // TODO: Hash yapılacak
            Status = StudentStatus.Pending
        };

        _db.Students.Add(student);
        await _db.SaveChangesAsync();

        return Ok("Kayıt başarılı. Admin onayından sonra giriş yapabilirsiniz.");
    }

    // ============================
    // STUDENT LOGIN (JWT)
    // ============================
    [HttpPost("student-login")]
    public async Task<IActionResult> StudentLogin(StudentLoginDto dto)
    {
        var student = await _db.Students
            .FirstOrDefaultAsync(s => s.Email == dto.Email && s.PasswordHash == dto.Password);

        if (student == null)
            return Unauthorized("Email veya şifre hatalı.");

        if (student.Status != StudentStatus.Approved)
            return Unauthorized("Hesabınız admin onayında.");

        var token = _tokenService.CreateStudentToken(student);

        return Ok(new { token });
    }

    // ============================
    // ADMIN LOGIN (JWT)
    // ============================
    [HttpPost("admin-login")]
    public async Task<IActionResult> AdminLogin(AdminLoginDto dto)
    {
        var admin = await _db.AdminUsers
            .FirstOrDefaultAsync(a => a.UserName == dto.UserName && a.PasswordHash == dto.Password);

        if (admin == null)
            return Unauthorized("Geçersiz admin bilgisi.");

        var token = _tokenService.CreateAdminToken(admin);

        return Ok(new { token });
    }


    // ============================
    // ADMIN REGISTER (Geçici olarak admin oluşturmak için)
    // ============================
    [HttpPost("admin-register")]
    public async Task<IActionResult> AdminRegister([FromBody] AdminRegisterDto dto) 
    {
        // Eğer AdminRegisterDto yoksa, bu parametreyi class içinde tanımlayabilirsin 
        // ya da mevcut bir DTO kullanabilirsin.
        
        if (await _db.AdminUsers.AnyAsync(x => x.Email == dto.Email))
            return BadRequest("Bu email zaten kayıtlı.");

        var newAdmin = new AdminUser
        {
            UserName = dto.UserName,
            Email = dto.Email, 
            PasswordHash = dto.Password // Şifreleme yapmıyorsan direkt ata
        };

        _db.AdminUsers.Add(newAdmin);
        await _db.SaveChangesAsync();

        return Ok("Admin başarıyla oluşturuldu.");
    }
}
