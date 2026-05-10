using Microsoft.EntityFrameworkCore;

namespace SkillForge.API.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Certificate> Certificates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Session -> Course (NoAction prevents cascade issues; FK exists as a column only, no DB constraint)
            modelBuilder.Entity<Session>()
                .HasOne(s => s.Course)
                .WithMany()
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.NoAction);

            // Session -> User (Instructor) — InstructorId maps to User.UserId
            modelBuilder.Entity<Session>()
                .HasOne(s => s.Instructor)
                .WithMany()
                .HasForeignKey(s => s.InstructorId)
                .OnDelete(DeleteBehavior.NoAction);

            // User -> Role
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
