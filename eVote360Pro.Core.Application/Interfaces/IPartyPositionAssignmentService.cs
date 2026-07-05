using eVote360Pro.Core.Application.DTOs.Candidates;
using eVote360Pro.Core.Application.DTOs.ElectivePosition;
using eVote360Pro.Core.Application.DTOs.PartyPositionAssignment;

namespace eVote360Pro.Core.Application.Interfaces
{
    public interface IPartyPositionAssignmentService
    {
        Task<Result<PartyPositionAssignmentDto>> AddAsync(CreatePartyPositionAssignmentDto createDto, int currentUserId);
        Task<Result> DeleteAsync(int assignmentId, int currentUserId);
        Task<Result<IEnumerable<AssignmentSummaryDto>>> GetAllSummaryAsync(int currentUserId);
        Task<Result<IEnumerable<CandidateDto>>> GetAvailableCandidatesForAssignmentAsync(int currentUserId);
        Task<Result<IEnumerable<ElectivePositionDto>>> GetAvailablePositionsForAssignmentAsync(int currentUserId);
        Task<Result<PartyPositionAssignmentDto>> GetByIdAsync(int id, int currentUserId);
    }
}
