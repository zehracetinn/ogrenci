namespace ProjeOgrenciYonetim.Web.Models
{
    public class Student
    {
        public int Id { get; set; }

        // Ad Soyad
        public string FullName { get; set; } = default!;

        // Okul numarası
        public string StudentNumber { get; set; } = default!;

        public string PasswordHash { get; set; } = string.Empty;


        public string Email { get; set; } = default!;

        // Şimdilik basit tutuyoruz (ileride gerçek hash’e çeviririz)
        

        // "C#, React, SQL" gibi
        public string KnownTechnologies { get; set; } = default!;

        public StudentStatus Status { get; set; } = StudentStatus.Pending;

        public ICollection<ProjectApplication> Applications { get; set; }
            = new List<ProjectApplication>();
    }
}
