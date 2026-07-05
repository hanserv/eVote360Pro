using eVote360Pro.Core.Application.DTOs.Candidates;
using eVote360Pro.Core.Application.ViewModels.Candidate;
using Mapster;

namespace eVote360Pro.Core.Application.Mappings.DtosVms
{
    public class CandidateDtoMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CandidateDto, CandidateViewModel>();
            config.NewConfig<CandidateDto, UpdateCandidateViewModel>();
            config.NewConfig<CandidateDto, UpdateCandidateDto>();
            config.NewConfig<CreateCandidateViewModel, CreateCandidateDto>();
            config.NewConfig<UpdateCandidateViewModel, UpdateCandidateDto>();

            config.NewConfig<CandidateDto, ChangeStatusCandidateViewModel>()
                .Map(dest => dest.NewStatus, src => !src.IsActive);
        }
    }
}
