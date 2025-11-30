using Microsoft.EntityFrameworkCore;
using ProjeOgrenciYonetim.Web.Models;

namespace ProjeOgrenciYonetim.Web.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectApplication> ProjectApplications { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Student için benzersiz alanlar
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.StudentNumber)
                .IsUnique();

            // Aynı öğrenci aynı projeye sadece 1 kez başvurabilsin
            modelBuilder.Entity<ProjectApplication>()
                .HasIndex(pa => new { pa.StudentId, pa.ProjectId })
                .IsUnique();

            // Admin User
            modelBuilder.Entity<AdminUser>()
                .HasIndex(a => a.UserName)
                .IsUnique();

            // Seed admin (admin / 1234)
            modelBuilder.Entity<AdminUser>().HasData(
                new AdminUser
                {
                    Id = 1,
                    UserName = "admin",
                    PasswordHash = "1234"
                }
            );
        }
    }
}
