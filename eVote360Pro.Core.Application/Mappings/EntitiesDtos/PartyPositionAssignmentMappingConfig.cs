using eVote360Pro.Core.Application.DTOs.PartyPositionAssignment;
using eVote360Pro.Core.Domain.Entities;
using Mapster;

namespace eVote360Pro.Core.Application.Mappings.EntitiesDtos
{
    public class PartyPositionAssignmentMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PartyPositionAssignment, PartyPositionAssignmentDto>();
        }
    }
}
