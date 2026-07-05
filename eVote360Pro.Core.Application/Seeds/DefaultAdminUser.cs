using eVote360Pro.Core.Application.DTOs.User;
using eVote360Pro.Core.Application.Interfaces;

namespace eVote360Pro.Core.Application.Seeds
{
    public static class DefaultAdminUser
    {
        public static async Task SeedAsync(IUserService userService)
        {
            if(!await userService.ExistsByUsernameAsync("admin"))
            {
                await userService.AddAsync(new CreateUserDto
                {
                    Name = "Admin",
                    LastName = "Leo",
                    Email = "admin@evote360.com",
                    Username = "admin",
                    Role = Domain.Common.Enums.UserRole.Admin,
                    Password = "admin123"
                });
            }
        }
    }
}
