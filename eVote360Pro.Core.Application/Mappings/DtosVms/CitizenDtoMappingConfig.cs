using eVote360Pro.Core.Application.DTOs.Citizen;
using eVote360Pro.Core.Application.ViewModels.Citizen;
using eVote360Pro.Core.Application.ViewModels.User;
using Mapster;

namespace eVote360Pro.Core.Application.Mappings.DtosVms
{
    public class CitizenDtoMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CitizenDto, CitizenViewModel>();
            config.NewConfig<CitizenDto, UpdateCitizenViewModel>();
            config.NewConfig<CreateCitizenViewModel, CreateCitizenDto>();
            config.NewConfig<UpdateCitizenViewModel, UpdateCitizenDto>();

            config.NewConfig<CitizenDto, ChangeStatusCitizenViewModel>()
                .Map(dest => dest.NewStatus, src => !src.IsActive);
        }
    }
}
