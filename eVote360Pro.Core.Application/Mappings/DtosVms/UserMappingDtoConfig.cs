using eVote360Pro.Core.Application.DTOs.User;
using eVote360Pro.Core.Application.ViewModels.User;
using Mapster;

namespace eVote360Pro.Core.Application.Mappings.EntitiesDtos
{
    public class UserMappingDtoConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<UserDto, UserViewModel>();
            config.NewConfig<UserDto, UpdateUserViewModel>();
            config.NewConfig<CreateUserViewModel, CreateUserDto>();
            config.NewConfig<UpdateUserViewModel, UpdateUserDto>();

            config.NewConfig<UserDto, ChangeStatusUserViewModel>()
                .Map(dest => dest.NewStatus, src => !src.IsActive);
        }
    }
}
