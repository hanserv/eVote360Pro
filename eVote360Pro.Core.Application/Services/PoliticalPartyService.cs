using eVote360Pro.Core.Application.DTOs.PoliticalParty;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Core.Domain.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Core.Application.Services
{
    public class PoliticalPartyService : IPoliticalPartyService
    {
        private readonly IPoliticalPartyRepository _politicalPartyRepository;
        private readonly IMapper _mapper;
        private readonly IElectionRepository _electionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPoliticalAllianceRepository _politicalAllianceRepository;
        public PoliticalPartyService(IPoliticalPartyRepository politicalPartyRepository, IMapper mapper,
            IElectionRepository electionRepository, IUserRepository userRepository,
            IPoliticalAllianceRepository politicalAllianceRepository)
        {
            _politicalPartyRepository = politicalPartyRepository;
            _mapper = mapper;
            _electionRepository = electionRepository;
            _userRepository = userRepository;
            _politicalAllianceRepository = politicalAllianceRepository;
        }

        public async Task<Result<IEnumerable<PoliticalPartyDto>>> GetAllAsync()
        {
            var entities = _mapper.Map<IEnumerable<PoliticalPartyDto>>(await _politicalPartyRepository.GetAllAsync());
            return Result<IEnumerable<PoliticalPartyDto>>.Success(entities);
        }

        public async Task<Result<IEnumerable<PoliticalPartyDto>>> GetAllActivesAsync(int? includeId = null)
        {
            var query = _politicalPartyRepository.GetAllQuery()
                            .Where(pp => pp.IsActive || pp.Id == includeId);

            var entities =  _mapper.Map<IEnumerable<PoliticalPartyDto>>(await query.ToListAsync());
            return Result<IEnumerable<PoliticalPartyDto>>.Success(entities);
        }

        public async Task<Result<IEnumerable<PoliticalPartyDto>>> GetAvailablePartiesForAllianceAsync(int currentUserId)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser is null || !currentUser.PoliticalPartyId.HasValue)
            {
                return Result<IEnumerable<PoliticalPartyDto>>.Failure(error: "User not found or no political party assigned.");
            }

            int currentUserPartyId = currentUser.PoliticalPartyId.Value;

            var excludedPartyIds = await _politicalAllianceRepository.GetAllQuery()
                .Where(pa => (pa.RequesterPartyId == currentUserPartyId || pa.TargetPartyId == currentUserPartyId) &&
                             (pa.Status == AllianceStatus.Accepted || pa.Status == AllianceStatus.Pending))
                .Select(pa => pa.RequesterPartyId == currentUserPartyId ? pa.TargetPartyId : pa.RequesterPartyId)
                .ToListAsync();

            var availableParties = await _politicalPartyRepository.GetAllQuery()
                .Where(p => p.IsActive &&
                            p.Id != currentUserPartyId &&
                            !excludedPartyIds.Contains(p.Id))
                .OrderBy(p => p.Name)
                .ToListAsync();

            return Result<IEnumerable<PoliticalPartyDto>>.Success(_mapper.Map<IEnumerable<PoliticalPartyDto>>(availableParties));
        }

        public async Task<Result<IEnumerable<PoliticalPartyDto>>> GetAllAvailablePartiesAsync()
        {
            var entities = await _politicalPartyRepository.GetAllQuery()
                                .Where(pp => pp.IsActive && pp.User == null)
                                .ToListAsync();

            var entityDtos = _mapper.Map<IEnumerable<PoliticalPartyDto>>(entities);
            return Result<IEnumerable<PoliticalPartyDto>>.Success(entityDtos);
        }

        public async Task<Result<PoliticalPartyDto>> GetByIdAsync(int id)
        {
            var entity = await _politicalPartyRepository.GetByIdAsync(id);

            if (entity is null)
            {
                return Result<PoliticalPartyDto>.Failure(error: "The political party doesnt exist");
            }

            return Result<PoliticalPartyDto>.Success(_mapper.Map<PoliticalPartyDto>(entity));
        }

        public async Task<Result<PoliticalPartyDto>> AddAsync(CreatePoliticalPartyDto createDto)
        {
            if (await _electionRepository.HasActiveElectionAsync())
            {
                return Result<PoliticalPartyDto>.Failure(error: "A political party cannot be created while there is an active election.");
            }

            createDto.Acronym = createDto.Acronym.Trim().ToUpper();
            createDto.Name = createDto.Name.Trim();

            var isAcronymAvailable = await _politicalPartyRepository.IsAcronymAvailableAsync(createDto.Acronym);

            if(!isAcronymAvailable)
            {
                return Result<PoliticalPartyDto>.Failure(error: "There is already a registered political party with that acronym.");
            }

            var entity = _mapper.Map<PoliticalParty>(createDto);
            entity.IsActive = true;
            entity.Logo = "";

            var result = await _politicalPartyRepository.AddAsync(entity);

            return Result<PoliticalPartyDto>.Success(_mapper.Map<PoliticalPartyDto>(result));
        }

        public async Task<Result> UpdateAsync(UpdatePoliticalPartyDto updateDto)
        {
            if (await _electionRepository.HasActiveElectionAsync())
            {
                return Result.Failure(error: "You cannot edit a political party while there is an active election.");
            }

            var entity = await _politicalPartyRepository.GetByIdAsync(updateDto.Id);

            if(entity is null)
            {
                return Result.Failure(error: "The political party doesnt exist");
            }

            updateDto.Acronym = updateDto.Acronym.Trim().ToUpper();
            updateDto.Name = updateDto.Name.Trim();

            if(await _politicalPartyRepository.HasParticipatedInElectionAsync(entity.Id))
            {
                if (entity.Name != updateDto.Name)
                {
                    return Result.Failure(error: "The name of this political party cannot be changed because it has already participated in an election.");
                }

                if (entity.Acronym != updateDto.Acronym)
                {
                    return Result.Failure(error: "The acronym of this political party cannot be changed because it has already participated in an election.");
                }

                if(entity.Logo != updateDto.Logo) // TODO: Test this validation
                {
                    return Result.Failure(error: "The logo of this political party cannot be changed because it has already participated in an election.");
                }
            }

            var isAcronymAvailable = await _politicalPartyRepository.IsAcronymAvailableAsync(updateDto.Acronym, excludeId: updateDto.Id);

            if (!isAcronymAvailable)
            {
                return Result.Failure(error: "There is already a registered political party with that acronym.");
            }

            entity.Logo = updateDto.Logo ?? entity.Logo;
            entity.Name = updateDto.Name;
            entity.Description = updateDto.Description;
            entity.Acronym = updateDto.Acronym;

            await _politicalPartyRepository.UpdateAsync(entity);
            return Result.Success();
        }

        public async Task<Result> ChangeStatusAsync(int id, bool active)
        {
            var entity = await _politicalPartyRepository.GetByIdAsync(id);

            if (entity is null)
            {
                return Result.Failure(error: "The political party doesnt exist");
            }

            var hasActiveElection = await _electionRepository.HasActiveElectionAsync();

            if (active)
            {
                return await ActivateAsync(entity, hasActiveElection);
            }

            return await InactivateAsync(entity, hasActiveElection);
        }

        #region Private Methods 
        private async Task<Result> ActivateAsync(PoliticalParty entity, bool hasActiveElection)
        {
            if (hasActiveElection)
            {
                return Result.Failure(error: "A political party cannot be activated while an election is active.");
            }

            if (entity.IsActive)
            {
                return Result.Failure(error: "The political party is already active.");
            }

            entity.IsActive = true;
            await _politicalPartyRepository.UpdateAsync(entity);
            return Result.Success();
        }

        private async Task<Result> InactivateAsync(PoliticalParty entity, bool hasActiveElection)
        {
            if (hasActiveElection)
            {
                return Result.Failure(error: "A political party cannot be deactivated while an election is active.");
            }

            if (!entity.IsActive)
            {
                return Result.Failure(error: "This political party is already inactive.");
            }

            if(await _politicalPartyRepository.HasActiveCandidatesAsync(entity.Id))
            {
                return Result.Failure(error: "This political party cannot be deactivated because it has registered active candidates.");
            }

            if(await _politicalPartyRepository.HasAssignedLeaderAsync(entity.Id,isActive: true))
            {
                return Result.Failure(error: "This political party cannot be deactivated because it has an assigned political leader.");
            }

            entity.IsActive = false;
            await _politicalPartyRepository.UpdateAsync(entity);
            return Result.Success();
        }
        #endregion
    }
}