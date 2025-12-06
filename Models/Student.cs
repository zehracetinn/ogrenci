using System.Text.Json.Serialization;

namespace ProjeOgrenciYonetim.Web.Models
{
    public class Student
    {
        public int Id { get; set; }

        public string FullName { get; set; } = default!;
        public string StudentNumber { get; set; } = default!;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = default!;
        public string KnownTechnologies { get; set; } = default!;
        public StudentStatus Status { get; set; } = StudentStatus.Pending;

        // ❗ Cycle burada oluşuyor → JSON'a gitmemeli
        [JsonIgnore]
        public ICollection<ProjectApplication> Applications { get; set; }
            = new List<ProjectApplication>();
    }
}
