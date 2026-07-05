using eVote360Pro.Core.Application.DTOs.ElectivePosition;

namespace eVote360Pro.Core.Application.Interfaces
{
    public interface IElectivePositionService
    {
        Task<Result<IEnumerable<ElectivePositionDto>>> GetAllAsync();
        Task<Result<IEnumerable<ElectivePositionDto>>> GetAllActivesAsync(int? includeId = null);
        Task<Result<ElectivePositionDto>> GetByIdAsync(int id);
        Task<Result<ElectivePositionDto>> AddAsync(CreateElectivePositionDto createDto);
        Task<Result> UpdateAsync(UpdateElectivePositionDto updateDto);
        Task<Result> ChangeStatusAsync(int id,bool active);
    }
}