using eVote360Pro.Core.Application.DTOs.User;
using eVote360Pro.Core.Domain.Entities;
using Mapster;

namespace eVote360Pro.Core.Application.Mappings.EntitiesDtos
{
    public class UserMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateUserDto, User>();
            config.NewConfig<User, UserDto>().TwoWays();
        }
    }
}
