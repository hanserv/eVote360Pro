using eVote360Pro.Core.Application.DTOs.PoliticalAlliance;
using eVote360Pro.Core.Application.ViewModels.PoliticalAlliance;
using eVote360Pro.Core.Domain.Entities;
using Mapster;

namespace eVote360Pro.Core.Application.Mappings.DtosVms
{
    public class PoliticalAllianceMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PoliticalAlliance, PoliticalAllianceDto>();

        }
    }
}
