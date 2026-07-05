using eVote360Pro.Core.Application.DTOs.Citizen;

namespace eVote360Pro.Core.Application.Interfaces
{
    public interface ICitizenService
    {
        Task<Result<IEnumerable<CitizenDto>>> GetAllAsync();
        Task<Result<CitizenDto>> GetByIdAsync(int id);
        Task<Result<CitizenDto>> AddAsync(CreateCitizenDto createDto);
        Task<Result> UpdateAsync(UpdateCitizenDto updateDto);
        Task<Result> ChangeStatusAsync(int id, bool active);
    }
}
