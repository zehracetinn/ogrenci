using System;

namespace ProjeOgrenciYonetim.Web.Models
{
    public class ProjectApplication
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = default!;

        public int ProjectId { get; set; }
        public Project Project { get; set; } = default!;
        
        public DateTime ApplyDate { get; set; } = DateTime.UtcNow;
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        // --- EKLENEN KISIM BAŞLANGIÇ ---
        // Başvuru durumunu tutacak özellik
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
        // --- EKLENEN KISIM BİTİŞ ---
    }

    // --- ENUM TANIMI (Dosyanın en altına, class dışına ekliyoruz) ---
    public enum ApplicationStatus
    {
        Pending = 0,   // Bekliyor
        Approved = 1,  // Onaylandı
        Rejected = 2   // Reddedildi
    }
}