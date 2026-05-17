using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SkillForge.API.Models
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Certificate> Certificates { get; set; }

        public DbSet<Trainee> Trainees { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentStatus> PaymentStatuses { get; set; }
        public DbSet<EnrollmentStatus> EnrollmentStatuses { get; set; }
        public DbSet<VerificationStatus> VerificationStatuses { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Room> Rooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.Instructor)
                .WithMany(i => i.Sessions)
                .HasForeignKey(s => s.InstructorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.UpdatedByInstructor)
                .WithMany(i => i.UpdatedResults)
                .HasForeignKey(r => r.UpdatedByInstructorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PaymentStatus>().HasData(
             new PaymentStatus { PaymentStatusId = 1, StatusName = "Pending" },
             new PaymentStatus { PaymentStatusId = 2, StatusName = "Paid" },
             new PaymentStatus { PaymentStatusId = 3, StatusName = "Failed" },
             new PaymentStatus { PaymentStatusId = 4, StatusName = "Refunded" });

           modelBuilder.Entity<EnrollmentStatus>().HasData(
            new EnrollmentStatus { EnrollmentStatusId = 1, StatusName = "Pending" },
            new EnrollmentStatus { EnrollmentStatusId = 2, StatusName = "Approved" },
            new EnrollmentStatus { EnrollmentStatusId = 3, StatusName = "Rejected" },
            new EnrollmentStatus { EnrollmentStatusId = 4, StatusName = "Completed" },
            new EnrollmentStatus { EnrollmentStatusId = 5, StatusName = "Cancelled" } );

            modelBuilder.Entity<VerificationStatus>().HasData(
             new VerificationStatus { VerificationStatusId = 1, StatusName = "Pending" },
             new VerificationStatus { VerificationStatusId = 2, StatusName = "Valid" },
             new VerificationStatus { VerificationStatusId = 3, StatusName = "Revoked" },
             new VerificationStatus { VerificationStatusId = 4, StatusName = "Expired" }
            );
            modelBuilder.Entity<Track>().HasData(
             new Track { TrackId = 1, TrackName = "Programming", Description = "Programming and software development courses" },
             new Track { TrackId = 2, TrackName = "Database", Description = "Database design and SQL courses" },
             new Track { TrackId = 3, TrackName = "Web Development", Description = "Web application development courses" }
            );

            modelBuilder.Entity<Room>().HasData(
             new Room { RoomId = 1, RoomName = "Lab 101", Location = "Building A", Capacity = 25 },
             new Room { RoomId = 2, RoomName = "Lab 102", Location = "Building A", Capacity = 25 },
             new Room { RoomId = 3, RoomName = "Room 201", Location = "Building B", Capacity = 30 }
            );

        }

    }
}
