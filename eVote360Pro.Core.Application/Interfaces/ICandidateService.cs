using eVote360Pro.Core.Application.DTOs.Candidates;

namespace eVote360Pro.Core.Application.Interfaces
{
    public interface ICandidateService
    {
        Task<Result<CandidateDto>> AddAsync(CreateCandidateDto createDto, int currentUserId);
        Task<Result> ChangeStatusAsync(int id, bool active, int currentUserId);
        Task<Result<IEnumerable<CandidateDto>>> GetAllAsync(int currentUserId);
        Task<Result<CandidateDto>> GetByIdAsync(int id);
        Task<Result> UpdateAsync(UpdateCandidateDto updateDto, int currentUserId);
    }
}