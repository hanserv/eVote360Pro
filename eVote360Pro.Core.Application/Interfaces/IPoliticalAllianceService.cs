using eVote360Pro.Core.Application.DTOs.PoliticalAlliance;

namespace eVote360Pro.Core.Application.Interfaces
{
    public interface IPoliticalAllianceService
    {
        Task<Result> AcceptAllianceRequestAsync(int allianceId, int currentUserId);
        Task<Result<PoliticalAllianceDto>> CreateAllianceRequestAsync(CreatePoliticalAllianceDto createDto, int currentUserId);
        Task<Result> DeleteAllianceAsync(int allianceId, int currentUserId);
        Task<Result> DeleteAllianceRequestAsync(int allianceId, int currentUserId);
        Task<Result<PoliticalAllianceDto>> GetByIdAsync(int id, int currentUserId);
        Task<Result<IEnumerable<CurrentAllianceDto>>> GetCurrentAlliancesAsync(int currentUserId);
        Task<Result<IEnumerable<PoliticalAllianceDto>>> GetPendingAlliancesAsync(int currentUserId);
        Task<Result<IEnumerable<PoliticalAllianceDto>>> GetSentAlliancesAsync(int currentUserId);
        Task<Result> RejectAllianceRequestAsync(int allianceId, int currentUserId);
    }
}
