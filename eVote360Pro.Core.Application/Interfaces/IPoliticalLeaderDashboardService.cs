using eVote360Pro.Core.Application.DTOs.User;

namespace eVote360Pro.Core.Application.Interfaces
{
    public interface IPoliticalLeaderDashboardService
    {
        Task<Result<PoliticalLeaderDashboardDto>> GetDashboardDataAsync(int currentUserId);
    }
}
