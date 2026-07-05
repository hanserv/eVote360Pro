using eVote360Pro.Core.Application.DTOs.Candidates;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Core.Domain.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Core.Application.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly IMapper _mapper;
        private readonly IElectionRepository _electionRepository;
        private readonly IUserRepository _userRepository;

        public CandidateService(ICandidateRepository candidateRepository, IMapper mapper,
            IElectionRepository electionRepository, IUserRepository userRepository)
        {
            _candidateRepository = candidateRepository;
            _mapper = mapper;
            _electionRepository = electionRepository;
            _userRepository = userRepository;
        }

        public async Task<Result<IEnumerable<CandidateDto>>> GetAllAsync(int currentUserId)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser is null || !currentUser.PoliticalPartyId.HasValue)
            {
                return Result<IEnumerable<CandidateDto>>.Failure("You dont have permissions or dont belong to any party.");
            }

            var query = _candidateRepository.GetAllQueryInclude(["PoliticalParty", "PositionAssignments.ElectivePosition"])
                                    .Where(c => c.PoliticalPartyId == currentUser.PoliticalPartyId);

            var dtos = _mapper.Map<IEnumerable<CandidateDto>>(await query.ToListAsync());
            return Result<IEnumerable<CandidateDto>>.Success(dtos);
        }

        public async Task<Result<CandidateDto>> GetByIdAsync(int id)
        {
            var entity = await _candidateRepository.GetAllQueryInclude(["PositionAssignments.ElectivePosition"])
                                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity is null)
            {
                return Result<CandidateDto>.Failure(error: "The candidate doesnt exist");
            }

            return Result<CandidateDto>.Success(_mapper.Map<CandidateDto>(entity));
        }

        public async Task<Result<CandidateDto>> AddAsync(CreateCandidateDto createDto, int currentUserId)
        {
            if(await _electionRepository.HasActiveElectionAsync())
            {
                return Result<CandidateDto>.Failure(error: "You cannot create a candidate while there is an active election.");
            }

            var currentUser = await _userRepository.GetAllQueryInclude(["PoliticalParty"])
                                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            if(currentUser is null)
            {
                return Result<CandidateDto>.Failure(error: "You dont have an account.");
            }

            if(currentUser.Role != UserRole.PoliticalLeader)
            {
                return Result<CandidateDto>.Failure(error: "You need to be a political leader to create a candidate.");
            }

            if (!currentUser.PoliticalPartyId.HasValue || currentUser.PoliticalParty is null)
            {
                return Result<CandidateDto>.Failure(error: "You cannot create candidates because you do not have an assigned political party.");
            }

            if(!currentUser.PoliticalParty.IsActive)
            {
                return Result<CandidateDto>.Failure(error: "You cannot create candidates because the assigned political party is inactive.");
            }

            var entity = _mapper.Map<Candidate>(createDto);
            entity.PoliticalPartyId = currentUser.PoliticalParty.Id;
            entity.IsActive = true;
            entity.Photo = "";

            var result = await _candidateRepository.AddAsync(entity);

            return Result<CandidateDto>.Success(_mapper.Map<CandidateDto>(result));
        }

        public async Task<Result> UpdateAsync(UpdateCandidateDto updateDto, int currentUserId)
        {
            if (await _electionRepository.HasActiveElectionAsync())
            {
                return Result.Failure(error: "You cannot edit a candidate while there is an active election.");
            }

            var entity = await _candidateRepository.GetByIdAsync(updateDto.Id);

            if(entity is null)
            {
                return Result.Failure(error: "The candidate doesnt exists.");
            }

            var currentUser = await _userRepository.GetAllQueryInclude(["PoliticalParty"])
                                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (currentUser is null)
            {
                return Result.Failure(error: "You dont have an account.");
            }

            if (currentUser.Role != UserRole.PoliticalLeader)
            {
                return Result.Failure(error: "You need to be a political leader to edit a candidate.");
            }

            if (!currentUser.PoliticalPartyId.HasValue || currentUser.PoliticalParty is null)
            {
                return Result.Failure(error: "You cannot edit candidates because you do not have an assigned political party.");
            }

            if (!currentUser.PoliticalParty.IsActive)
            {
                return Result.Failure(error: "You cannot edit candidates because your assigned political party is inactive.");
            }

            if (entity.PoliticalPartyId != currentUser.PoliticalPartyId)
            {
                return Result.Failure(error: "You do not have permission to modify this candidate.");
            }

            updateDto.Name = updateDto.Name.Trim();
            updateDto.LastName = updateDto.LastName.Trim();

            if(await _candidateRepository.HasParticipatedInElectionAsync(entity.Id))
            {
                if(entity.Name != updateDto.Name ||
                    entity.LastName != updateDto.LastName || 
                   !string.IsNullOrWhiteSpace(updateDto.Photo) && entity.Photo != updateDto.Photo)
                {
                    return Result.Failure(error: "The main details of this candidate cannot be modified because they have already participated in an election.");
                }
            }

            entity.Name = updateDto.Name;
            entity.LastName = updateDto.LastName;
            entity.Photo = updateDto.Photo ?? entity.Photo;

            await _candidateRepository.UpdateAsync(entity);
            return Result.Success();
        }

        public async Task<Result> ChangeStatusAsync(int id, bool active,int currentUserId)
        {
            var entity = await _candidateRepository.GetAllQueryInclude(["PoliticalParty"])
                                .FirstOrDefaultAsync(u => u.Id == id);

            if (entity is null)
            {
                return Result.Failure(error: "The candidate doesnt exist");
            }

            var permissionResult = await ValidateLeaderPermissionsAsync(currentUserId, entity.PoliticalPartyId);
            
            if (!permissionResult.IsSuccess)
            {
                return permissionResult; 
            }

            var hasActiveElection = await _electionRepository.HasActiveElectionAsync();

            if (active)
            {
                return await ActivateAsync(entity, hasActiveElection);
            }

            return await InactivateAsync(entity, hasActiveElection);
        }

        #region Private Methods
        private async Task<Result> ValidateLeaderPermissionsAsync(int currentUserId, int candidatePartyId)
        {
            var currentUser = await _userRepository.GetAllQueryInclude(["PoliticalParty"])
                                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (currentUser is null)
            {
                return Result.Failure(error: "You dont have an account.");
            }

            if (currentUser.Role != UserRole.PoliticalLeader)
            {
                return Result.Failure(error: "You need to be a political leader to modify a candidate.");
            }

            if (!currentUser.PoliticalPartyId.HasValue || currentUser.PoliticalParty is null)
            {
                return Result.Failure(error: "You cannot modify candidates because you do not have an assigned political party.");
            }

            if (!currentUser.PoliticalParty.IsActive)
            {
                return Result.Failure(error: "You cannot modify candidates because your assigned political party is inactive.");
            }

            if (candidatePartyId != currentUser.PoliticalPartyId)
            {
                return Result.Failure(error: "You do not have permission to modify this candidate.");
            }

            return Result.Success();
        }
        private async Task<Result> ActivateAsync(Candidate entity, bool hasActiveElection)
        {
            if (hasActiveElection)
            {
                return Result.Failure(error: "A candidate cannot be activated while there is an active election.");
            }

            if (entity.IsActive)
            {
                return Result.Failure(error: "The candidate is already active.");
            }

            entity.IsActive = true;
            await _candidateRepository.UpdateAsync(entity);
            return Result.Success();
        }

        private async Task<Result> InactivateAsync(Candidate entity, bool hasActiveElection)
        {
            if (hasActiveElection)
            {
                return Result.Failure(error: "A candidate cannot be deactivated while an election is active.");
            }

            if (!entity.IsActive)
            {
                return Result.Failure(error: "This candidate is already inactive.");
            }

            if (await _candidateRepository.IsAssignedToActiveElectivePositionAsync(entity.Id))
            {
                return Result.Failure(error: "This candidate cannot be deactivated because they are assigned to an elective position.");
            }

            entity.IsActive = false;
            await _candidateRepository.UpdateAsync(entity);
            return Result.Success();
        }
        #endregion
    }
}
