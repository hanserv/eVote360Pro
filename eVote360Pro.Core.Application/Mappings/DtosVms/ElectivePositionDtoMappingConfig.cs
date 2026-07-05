using eVote360Pro.Core.Application.DTOs.ElectivePosition;
using eVote360Pro.Core.Application.ViewModels.ElectivePosition;
using Mapster;

namespace eVote360Pro.Core.Application.Mappings.EntitiesDtos
{
    public class ElectivePositionDtoMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ElectivePositionDto, ElectivePositionViewModel>();
            config.NewConfig<ElectivePositionDto, UpdateElectivePositionViewModel>();
            config.NewConfig<CreateElectivePositionViewModel, CreateElectivePositionDto>();
            config.NewConfig<UpdateElectivePositionViewModel, UpdateElectivePositionDto>();

            config.NewConfig<ElectivePositionDto, ChangeStatusElectiveViewModel>()
                .Map(dest => dest.NewStatus, src => !src.IsActive);
        }
    }
}
