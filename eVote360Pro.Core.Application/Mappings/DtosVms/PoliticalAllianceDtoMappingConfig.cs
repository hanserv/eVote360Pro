using eVote360Pro.Core.Application.DTOs.PoliticalAlliance;
using eVote360Pro.Core.Application.ViewModels.PoliticalAlliance;
using Mapster;

namespace eVote360Pro.Core.Application.Mappings.DtosVms
{
    public class PoliticalAllianceDtoMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PoliticalAllianceDto, PoliticalAllianceViewModel>();
            config.NewConfig<PoliticalAllianceDto, AcceptAllianceViewModel>();
            config.NewConfig<PoliticalAllianceDto, RejectAllianceViewModel>();
            config.NewConfig<PoliticalAllianceDto, DeleteAllianceViewModel>();
            config.NewConfig<PoliticalAllianceDto, DeleteAllianceRequestViewModel>();
            config.NewConfig<CreatePoliticalAllianceViewModel, CreatePoliticalAllianceDto>();
            config.NewConfig<CurrentAllianceDto, CurrentAllianceViewModel>();

            config.NewConfig<PoliticalAllianceDto, AcceptAllianceViewModel>()
                .Map(dest => dest.Name, src => $"{src.RequesterParty.Name} ({src.RequesterParty.Acronym})");

            config.NewConfig<PoliticalAllianceDto, RejectAllianceViewModel>()
               .Map(dest => dest.Name, src => $"{src.RequesterParty.Name} ({src.RequesterParty.Acronym})");
            
            config.NewConfig<PoliticalAllianceDto, DeleteAllianceRequestViewModel>()
               .Map(dest => dest.Name, src => $"{src.TargetParty.Name} ({src.TargetParty.Acronym})");
            
            config.NewConfig<PoliticalAllianceDto, DeleteAllianceViewModel>()
               .Map(dest => dest.Name, src => $"{src.TargetParty.Name} ({src.TargetParty.Acronym})");
        }
    }
}
