using eVote360Pro.Core.Application.DTOs.User;

namespace eVote360Pro.Core.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserDto>> LoginAsync(LoginDto loginDto);
        Task<Result<IEnumerable<UserDto>>> GetAllAsync();
        Task<Result<UserDto>> GetByIdAsync(int id);
        Task<Result<UserDto>> AddAsync(CreateUserDto createDto);
        Task<Result> UpdateAsync(UpdateUserDto updateDto, int currentUserId);
        Task<Result> ChangeStatusAsync(int id, bool active, int currentUserId);
        Task<Result<IEnumerable<UserDto>>> GetAllAvailableLeadersAsync();
        Task<bool> ExistsByUsernameAsync(string username);
    }
}
