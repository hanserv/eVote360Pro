using eVote360Pro.Core.Domain.Common.Enums;

namespace eVote360Pro.Core.Application.DTOs.User
{
    public class UserDto : BaseDto<int>
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required UserRole Role { get; set; }
        public required bool IsActive { get; set; }
    }
}
