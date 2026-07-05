using eVote360Pro.Core.Application.DTOs.PoliticalLeaderAssignment;

namespace eVote360Pro.Core.Application.Interfaces
{
    public interface IPoliticalLeaderAssignmentService
    {
        Task<Result<IEnumerable<PoliticalLeaderAssignmentDto>>> GetAllAsync();
        Task<Result> AsyncPoliticalLeaderAsync(CreatePoliticalLeaderAssignmentDto createDto);
        Task<Result> ExistRelationAsync(int userId, int politicalPartyId);
        Task<Result> DeleteAssignmentAsync(int userId);
    }
}
