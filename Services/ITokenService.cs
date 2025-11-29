using ProjeOgrenciYonetim.Web.Models;

namespace ProjeOgrenciYonetim.Web.Services
{
    public interface ITokenService
    {
        string CreateStudentToken(Student student);
        string CreateAdminToken(AdminUser admin);
    }
}
