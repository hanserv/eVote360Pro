using eVote360Pro.Core.Application.DTOs.PoliticalParty;

namespace eVote360Pro.Core.Application.Interfaces
{
    public interface IPoliticalPartyService
    {
        Task<Result<IEnumerable<PoliticalPartyDto>>> GetAllAsync();
        Task<Result<IEnumerable<PoliticalPartyDto>>> GetAllActivesAsync(int? includeId = null); // TODO: remove
        Task<Result<PoliticalPartyDto>> AddAsync(CreatePoliticalPartyDto createDto);
        Task<Result> ChangeStatusAsync(int id, bool active);
        Task<Result<PoliticalPartyDto>> GetByIdAsync(int id);
        Task<Result> UpdateAsync(UpdatePoliticalPartyDto updateDto);
        Task<Result<IEnumerable<PoliticalPartyDto>>> GetAllAvailablePartiesAsync();
        Task<Result<IEnumerable<PoliticalPartyDto>>> GetAvailablePartiesForAllianceAsync(int currentUserId);
    }
}
