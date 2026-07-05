using eVote360Pro.Core.Application.DTOs.Citizen;
using eVote360Pro.Core.Application.DTOs.PoliticalLeaderAssignment;
using eVote360Pro.Core.Application.DTOs.PoliticalParty;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Core.Domain.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Core.Application.Services
{
    public class PoliticalLeaderAssignmentService : IPoliticalLeaderAssignmentService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPoliticalPartyRepository _politicalPartyRepository;
        private readonly IElectionRepository _electionRepository;
        private readonly IMapper _mapper;

        public PoliticalLeaderAssignmentService(IUserRepository userRepository, IPoliticalPartyRepository politicalPartyRepository, 
            IElectionRepository electionRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _politicalPartyRepository = politicalPartyRepository;
            _electionRepository = electionRepository;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<PoliticalLeaderAssignmentDto>>> GetAllAsync() 
        {
            var users = await _userRepository.GetAllQueryInclude(["PoliticalParty"])
                                .Where(u => u.PoliticalPartyId.HasValue).ToListAsync();

            var entities = _mapper.Map<IEnumerable<PoliticalLeaderAssignmentDto>>(users);
            return Result<IEnumerable<PoliticalLeaderAssignmentDto>>.Success(entities);
        }

        public async Task<Result> AsyncPoliticalLeaderAsync(CreatePoliticalLeaderAssignmentDto createDto)
        {
            if (await _electionRepository.HasActiveElectionAsync())
            {
                return Result.Failure(error: "A political leader assignment cannot be created while there is an active election.");
            }

            var user = await _userRepository.GetByIdAsync(createDto.UserId);

            if(user is null || !user.IsActive)
            {
                return Result.Failure(error: "The user doesnt exists or is not active.");
            }

            if (user.Role != UserRole.PoliticalLeader)
            {
                return Result.Failure(error: "The selected user does not have the role of political leader.");
            }

            if(user.PoliticalPartyId.HasValue)
            {
                return Result.Failure(error: "This leader is already linked to another political party.");
            }

            var party = await _politicalPartyRepository.GetByIdAsync(createDto.PoliticalPartyId);

            if (party is null || !party.IsActive)
            {
                return Result.Failure(error: "The political party doesnt exists or is not active.");
            }

            if (await _politicalPartyRepository.HasAssignedLeaderAsync(party.Id))
            {
                return Result.Failure(error: "This political party already has an assigned leader.");
            }

            user.PoliticalPartyId = party.Id;
            await _userRepository.UpdateAsync(user);
            return Result.Success();
        }

        public async Task<Result> ExistRelationAsync(int userId,int politicalPartyId)
        {
            var entity = await _userRepository.GetByIdAsync(userId);

            if(entity is null)
            {
                return Result.Failure(error: "The leader doesnt exists.");
            }

            if(entity.PoliticalPartyId !=  politicalPartyId)
            {
                return Result.Failure(error: "The selected assignment does not exist or has already been deleted.");
            }

            return Result.Success();
        }

        public async Task<Result> DeleteAssignmentAsync(int userId)
        {
            if (await _electionRepository.HasActiveElectionAsync())
            {
                return Result.Failure(error: "A political leader assignment cannot be deleted while there is an active election.");
            }

            var entity = await _userRepository.GetByIdAsync(userId);

            if (entity is null)
            {
                return Result.Failure(error: "The leader doesnt exists.");
            }

            entity.PoliticalPartyId = null;
            await _userRepository.UpdateAsync(entity);
            return Result.Success();
        }
    }
}
