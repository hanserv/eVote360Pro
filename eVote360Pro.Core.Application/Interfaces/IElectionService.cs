using eVote360Pro.Core.Application.DTOs.Election;

namespace eVote360Pro.Core.Application.Interfaces
{
    public interface IElectionService
    {
        Task<Result> ActivateAsync(int id);
        Task<Result<ElectionDto>> AddAsync(CreateElectionDto createDto);
        Task<Result> FinalizeAsync(int id);
        Task<Result<IEnumerable<ElectionDto>>> GetAllElectionsAsync();
        Task<Result<IEnumerable<int>>> GetAvailableElectionYearsAsync();
        Task<Result<ElectionDto>> GetByIdAsync(int id);
        Task<Result<ElectionResultsDto>> GetElectionResultsAsync(int electionId);
        Task<Result<IEnumerable<ElectionSummaryDto>>> GetElectoralSummaryByYearAsync(int year);
        Task<bool> HasActiveElectionAsync();
    }
}