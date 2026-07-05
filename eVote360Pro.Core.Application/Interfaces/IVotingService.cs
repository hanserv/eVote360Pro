using eVote360Pro.Core.Application.DTOs.Voting;

namespace eVote360Pro.Core.Application.Interfaces
{
    public interface IVotingService
    {
        Task<Result> FinalizeVotingAsync(string citizenDocument);
        Task<Result<IEnumerable<AvailablePositionDto>>> GetAvailablePositionsForCitizenAsync(string citizenDocument);
        Task<Result<PositionCandidatesDto>> GetCandidatesForPositionAsync(int positionId, string citizenDocument);
        Task<Result> VoteAsync(int positionId, int selectedAssignmentId, string citizenDocument);
    }
}