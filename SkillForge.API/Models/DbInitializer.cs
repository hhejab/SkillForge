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

            var instructor = await CreateUserAsync(
                userManager,
                "instructor@skillforge.com",
                "Admin@123",
                "Test Instructor",
                "Instructor"
            );

            var trainee = await CreateUserAsync(
                userManager,
                "trainee@skillforge.com",
                "Admin@123",
                "Test Trainee",
                "Trainee"
            );

            if (!await context.Instructors.AnyAsync(i => i.UserId == instructor.Id))
            {
                context.Instructors.Add(new Instructor
                {
                    UserId = instructor.Id,
                    FullName = instructor.FullName,
                    Email = instructor.Email ?? "",
                    
                });
            }

            if (!await context.Trainees.AnyAsync(t => t.UserId == trainee.Id))
            {
                context.Trainees.Add(new Trainee
                {
                    UserId = trainee.Id,
                    FullName = trainee.FullName,
                    Email = trainee.Email ?? "",
                    Phone = "00000000"
                });
            }

            await context.SaveChangesAsync();
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