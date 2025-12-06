using System;
using System.Text.Json.Serialization;

namespace ProjeOgrenciYonetim.Web.Models
{
    public class ProjectApplication
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        // ❗ Cycle oluşmasın diye JSON'a gönderilmeyecek
        [JsonIgnore]
        public Student Student { get; set; } = default!;

        public int ProjectId { get; set; }

        // ❗ Project → Applications → Project döngüsü var, JSON'dan çıkar
        [JsonIgnore]
        public Project Project { get; set; } = default!;
        
        public DateTime ApplyDate { get; set; } = DateTime.UtcNow;
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    }

    public enum ApplicationStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }
}
