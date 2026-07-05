using eVote360Pro.Core.Application.DTOs.PoliticalParty;
using eVote360Pro.Core.Application.ViewModels.PoliticalParty;
using Mapster;

namespace eVote360Pro.Core.Application.Mappings.DtosVms
{
    public class PoliticalPartyDtoMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PoliticalPartyDto, PoliticalPartyViewModel>();
            config.NewConfig<PoliticalPartyDto, UpdatePoliticalPartyDto>();
            config.NewConfig<CreatePoliticalPartyViewModel, CreatePoliticalPartyDto>();
            config.NewConfig<PoliticalPartyDto, UpdatePoliticalPartyViewModel>();
            config.NewConfig<UpdatePoliticalPartyViewModel, UpdatePoliticalPartyDto>();

            config.NewConfig<PoliticalPartyDto, ChangeStatusPoliticalPartyViewModel>()
                .Map(dest => dest.Name, src =>  $"{src.Name} ({src.Acronym})")
                .Map(dest => dest.NewStatus, src => !src.IsActive);
        }
    }
}
