namespace ProjeOgrenciYonetim.Web.Models
{
    public class AdminUser
    {
        public int Id { get; set; }

        public string UserName { get; set; } = default!;
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = default!;
    }
}
