using eVote360Pro.Core.Application.DTOs.Citizen;
using eVote360Pro.Core.Domain.Entities;
using Mapster;

namespace eVote360Pro.Core.Application.Mappings.EntitiesDtos
{
    public class CitizenMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Citizen, CitizenDto>().TwoWays();
            config.NewConfig<CreateCitizenDto, Citizen>();
        }
    }
}
