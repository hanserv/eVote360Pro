using eVote360Pro.Core.Application.DTOs.ElectivePosition;
using eVote360Pro.Core.Domain.Entities;
using Mapster;

namespace eVote360Pro.Core.Application.Mappings.EntitiesDtos
{
    public class ElectivePositionMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ElectivePosition, ElectivePositionDto>()
                .TwoWays();

            config.NewConfig<CreateElectivePositionDto, ElectivePosition>();
        }
    }
}
