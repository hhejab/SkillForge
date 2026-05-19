using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SkillForge.API.Models
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            string[] roles =
            {
                "TrainingCoordinator",
                "Instructor",
                "Trainee"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var coordinator = await CreateUserAsync(
                userManager,
                "coordinator@skillforge.com",
                "Admin@123",
                "Training Coordinator",
                "TrainingCoordinator"
            );

            var mainInstructorUser = await CreateUserAsync(
                userManager,
                "instructor@skillforge.com",
                "Admin@123",
                "Test Instructor",
                "Instructor"
            );

            var mainTraineeUser = await CreateUserAsync(
                userManager,
                "trainee@skillforge.com",
                "Admin@123",
                "Test Trainee",
                "Trainee"
            );

            if (!await context.Instructors.AnyAsync(i => i.UserId == mainInstructorUser.Id))
            {
                context.Instructors.Add(new Instructor
                {
                    UserId = mainInstructorUser.Id,
                    FullName = mainInstructorUser.FullName,
                    Email = mainInstructorUser.Email ?? "",
                    Phone = "39000000",
                    Specialization = "Programming"
                });
            }

            if (!await context.Trainees.AnyAsync(t => t.UserId == mainTraineeUser.Id))
            {
                context.Trainees.Add(new Trainee
                {
                    UserId = mainTraineeUser.Id,
                    FullName = mainTraineeUser.FullName,
                    Email = mainTraineeUser.Email ?? "",
                    Phone = "36000000"
                });
            }

            await context.SaveChangesAsync();

            if (!await context.Categories.AnyAsync())
            {
                context.Categories.AddRange(
                    new Category { CategoryName = "Programming" },
                    new Category { CategoryName = "Web Development" },
                    new Category { CategoryName = "Database" },
                    new Category { CategoryName = "Cybersecurity" },
                    new Category { CategoryName = "Cloud Computing" },
                    new Category { CategoryName = "AI and Machine Learning" },
                    new Category { CategoryName = "Mobile Development" },
                    new Category { CategoryName = "Software Engineering" },
                    new Category { CategoryName = "Data Analytics" },
                    new Category { CategoryName = "Networking" }
                );

                await context.SaveChangesAsync();
            }

            if (await context.Rooms.CountAsync() < 10)
            {
                context.Rooms.AddRange(
                    new Room { RoomName = "Lab 103", Location = "Building A", Capacity = 25 },
                    new Room { RoomName = "Lab 104", Location = "Building A", Capacity = 25 },
                    new Room { RoomName = "Room 202", Location = "Building B", Capacity = 30 },
                    new Room { RoomName = "Room 203", Location = "Building B", Capacity = 30 },
                    new Room { RoomName = "Lecture Hall 1", Location = "Building C", Capacity = 50 },
                    new Room { RoomName = "Lecture Hall 2", Location = "Building C", Capacity = 50 },
                    new Room { RoomName = "Computer Lab 301", Location = "Building D", Capacity = 35 }
                );

                await context.SaveChangesAsync();
            }

            for (int i = 1; i <= 10; i++)
            {
                var email = $"instructor{i}@skillforge.com";

                var instructorUser = await CreateUserAsync(
                    userManager,
                    email,
                    "Admin@123",
                    $"Instructor {i}",
                    "Instructor"
                );

                if (!await context.Instructors.AnyAsync(x => x.UserId == instructorUser.Id))
                {
                    context.Instructors.Add(new Instructor
                    {
                        UserId = instructorUser.Id,
                        FullName = instructorUser.FullName,
                        Email = instructorUser.Email ?? "",
                        Phone = $"390000{i:D2}",
                        Specialization = i % 2 == 0 ? "Web Development" : "Programming"
                    });
                }
            }

            for (int i = 1; i <= 10; i++)
            {
                var email = $"trainee{i}@skillforge.com";

                var traineeUser = await CreateUserAsync(
                    userManager,
                    email,
                    "Admin@123",
                    $"Trainee {i}",
                    "Trainee"
                );

                if (!await context.Trainees.AnyAsync(x => x.UserId == traineeUser.Id))
                {
                    context.Trainees.Add(new Trainee
                    {
                        UserId = traineeUser.Id,
                        FullName = traineeUser.FullName,
                        Email = traineeUser.Email ?? "",
                        Phone = $"360000{i:D2}"
                    });
                }
            }

            await context.SaveChangesAsync();

            if (!await context.Courses.AnyAsync())
            {
                var categories = await context.Categories.ToListAsync();
                var tracks = await context.Tracks.ToListAsync();

                context.Courses.AddRange(
                    new Course { Title = "C# Fundamentals", Description = "Introduction to C# programming.", CategoryId = categories[0].CategoryId, TrackId = tracks[0].TrackId, Duration = 20 },
                    new Course { Title = "ASP.NET Core MVC", Description = "Build web apps using MVC.", CategoryId = categories[1].CategoryId, TrackId = tracks[2].TrackId, Duration = 25 },
                    new Course { Title = "Entity Framework Core", Description = "Database access using EF Core.", CategoryId = categories[2].CategoryId, TrackId = tracks[1].TrackId, Duration = 18 },
                    new Course { Title = "SQL Server Basics", Description = "Relational database and SQL queries.", CategoryId = categories[2].CategoryId, TrackId = tracks[1].TrackId, Duration = 15 },
                    new Course { Title = "JavaScript Essentials", Description = "Client-side web programming.", CategoryId = categories[1].CategoryId, TrackId = tracks[2].TrackId, Duration = 16 },
                    new Course { Title = "Python Programming", Description = "Programming fundamentals using Python.", CategoryId = categories[0].CategoryId, TrackId = tracks[0].TrackId, Duration = 20 },
                    new Course { Title = "Azure Cloud Basics", Description = "Cloud deployment and Azure services.", CategoryId = categories[4].CategoryId, TrackId = tracks[2].TrackId, Duration = 12 },
                    new Course { Title = "Cybersecurity Basics", Description = "Security concepts and safe systems.", CategoryId = categories[3].CategoryId, TrackId = tracks[0].TrackId, Duration = 14 },
                    new Course { Title = "Data Analytics with Power BI", Description = "Analyze and visualize data.", CategoryId = categories[8].CategoryId, TrackId = tracks[1].TrackId, Duration = 18 },
                    new Course { Title = "Software Testing", Description = "Testing methods and quality assurance.", CategoryId = categories[7].CategoryId, TrackId = tracks[0].TrackId, Duration = 15 }
                );

                await context.SaveChangesAsync();
            }

            if (!await context.Sessions.AnyAsync())
            {
                var courses = await context.Courses.ToListAsync();
                var instructors = await context.Instructors.ToListAsync();
                var rooms = await context.Rooms.ToListAsync();

                for (int i = 0; i < 10; i++)
                {
                    context.Sessions.Add(new Session
                    {
                        CourseId = courses[i % courses.Count].CourseId,
                        InstructorId = instructors[i % instructors.Count].InstructorId,
                        RoomId = rooms[i % rooms.Count].RoomId,
                        StartDate = DateTime.Now.AddDays(i + 1).Date.AddHours(9),
                        EndDate = DateTime.Now.AddDays(i + 1).Date.AddHours(12),
                        Capacity = 20
                    });
                }

                await context.SaveChangesAsync();
            }

            if (!await context.Enrollments.AnyAsync())
            {
                var trainees = await context.Trainees.ToListAsync();
                var sessions = await context.Sessions.ToListAsync();

                for (int i = 0; i < 10; i++)
                {
                    context.Enrollments.Add(new Enrollment
                    {
                        TraineeId = trainees[i % trainees.Count].TraineeId,
                        SessionId = sessions[i % sessions.Count].SessionId,
                        EnrollmentDate = DateTime.Now.AddDays(-i),
                        EnrollmentStatusId = 2
                    });
                }

                await context.SaveChangesAsync();
            }

            if (!await context.Payments.AnyAsync())
            {
                var enrollments = await context.Enrollments.ToListAsync();

                for (int i = 0; i < enrollments.Count; i++)
                {
                    context.Payments.Add(new Payment
                    {
                        EnrollmentId = enrollments[i].EnrollmentId,
                        Amount = 75 + (i * 10),
                        PaymentDate = DateTime.Now.AddDays(-i),
                        PaymentMethod = i % 2 == 0 ? "Card" : "Cash",
                        PaymentStatusId = i % 3 == 0 ? 1 : 2
                    });
                }

                await context.SaveChangesAsync();
            }

            if (!await context.Results.AnyAsync())
            {
                var enrollments = await context.Enrollments.ToListAsync();
                var instructorProfile = await context.Instructors.FirstAsync();

                for (int i = 0; i < enrollments.Count; i++)
                {
                    var score = 60 + i;

                    context.Results.Add(new Result
                    {
                        EnrollmentId = enrollments[i].EnrollmentId,
                        GradeOrScore = score.ToString(),
                        PassFail = score >= 60 ? "Pass" : "Fail",
                        UpdatedByInstructorId = instructorProfile.InstructorId,
                        UpdatedAt = DateTime.Now.AddDays(-i)
                    });
                }

                await context.SaveChangesAsync();
            }

            if (!await context.Certificates.AnyAsync())
            {
                var passedResults = await context.Results
                    .Where(r => r.PassFail == "Pass")
                    .Include(r => r.Enrollment)
                    .ToListAsync();

                for (int i = 0; i < passedResults.Count && i < 10; i++)
                {
                    context.Certificates.Add(new Certificate
                    {
                        EnrollmentId = passedResults[i].EnrollmentId,
                        CertificateCode = $"SF-CERT-2026-{(i + 1):D4}",
                        IssueDate = DateTime.Now.AddDays(-i),
                        VerificationStatusId = 2
                    });
                }

                await context.SaveChangesAsync();
            }
        }

        private static async Task<ApplicationUser> CreateUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string fullName,
            string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create seed user: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }

            return user;
        }
    }
}