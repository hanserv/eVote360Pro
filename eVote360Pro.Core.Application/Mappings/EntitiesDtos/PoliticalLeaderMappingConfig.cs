using eVote360Pro.Core.Application.DTOs.PoliticalLeaderAssignment;
using eVote360Pro.Core.Domain.Entities;
using Mapster;

namespace eVote360Pro.Core.Application.Mappings.EntitiesDtos
{
    public class PoliticalLeaderMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<User, PoliticalLeaderAssignmentDto>()
                .Map(dest => dest.UserId, src => src.Id)
                .Map(dest => dest.UserFullName, src => $"{src.Name} {src.LastName}")
                .Map(dest => dest.Username, src => src.Username)
                .Map(dest => dest.IsUserActive, src => src.IsActive)

                .Map(dest => dest.PoliticalPartyId, src => src.PoliticalParty!.Id)
                .Map(dest => dest.PoliticalPartyName, src => src.PoliticalParty!.Name)
                .Map(dest => dest.PoliticalPartyAcronym, src => src.PoliticalParty!.Acronym)
                .Map(dest => dest.IsPoliticalPartyActive, src => src.PoliticalParty!.IsActive);
        }
    }
}
