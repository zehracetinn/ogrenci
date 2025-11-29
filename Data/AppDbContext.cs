using Microsoft.EntityFrameworkCore;
using ProjeOgrenciYonetim.Web.Models;

namespace ProjeOgrenciYonetim.Web.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<ProjectApplication> ProjectApplications => Set<ProjectApplication>();
        public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

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

            // Admin user için benzersiz UserName
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
