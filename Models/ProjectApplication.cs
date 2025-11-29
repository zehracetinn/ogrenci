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

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    }
}
