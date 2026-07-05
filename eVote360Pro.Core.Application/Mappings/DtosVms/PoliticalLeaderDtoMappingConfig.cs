using eVote360Pro.Core.Application.DTOs.PoliticalLeaderAssignment;
using eVote360Pro.Core.Application.ViewModels.PoliticalLeaderAssignment;
using eVote360Pro.Core.Domain.Entities;
using Mapster;

namespace eVote360Pro.Core.Application.Mappings.EntitiesDtos
{
    public class PoliticalLeaderDtoMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PoliticalLeaderAssignmentDto, PoliticalLeaderAssignmentViewModel>();
        }
    }
}
