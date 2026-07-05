using eVote360Pro.Core.Application.DTOs.Candidates;
using eVote360Pro.Core.Domain.Entities;
using Mapster;

namespace eVote360Pro.Core.Application.Mappings.EntitiesDtos
{
    public class CandidateMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Candidate, CandidateDto>()
            .Map(dest => dest.ElectivePositionName,
                 src => src.PositionAssignments == null || !src.PositionAssignments.Any() ? "No associated position"
                                                     : src.PositionAssignments
                                                     .Where(pa => pa.ElectivePosition != null && pa.ElectivePosition.IsActive)
                                                     .Select(pa => pa.ElectivePosition!.Name)
                                                     .FirstOrDefault() ?? "No associated position");
        }
    }
}
