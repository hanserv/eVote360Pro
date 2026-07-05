using eVote360Pro.Core.Application.DTOs.PartyPositionAssignment;
using eVote360Pro.Core.Application.ViewModels.PartyPositionAssignment;
using Mapster;

namespace eVote360Pro.Core.Application.Mappings.DtosVms
{
    public class PartyPositionAssignmentDtoMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AssignmentSummaryDto, AssignmentSummaryViewModel>();
            config.NewConfig<CreatePartyPositionAssignmentViewModel, CreatePartyPositionAssignmentDto>();
        }
    }
}
