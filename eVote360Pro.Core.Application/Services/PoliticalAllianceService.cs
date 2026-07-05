using eVote360Pro.Core.Application.DTOs.Candidates;
using eVote360Pro.Core.Application.DTOs.PoliticalAlliance;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Core.Domain.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Core.Application.Services
{
    public class PoliticalAllianceService : IPoliticalAllianceService
    {
        private readonly IPoliticalAllianceRepository _politicalAllianceRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IPoliticalPartyRepository _politicalPartyRepository;
        private readonly IElectionRepository _electionRepository;
        private readonly IPartyPositionAssignmentRepository _assignmentRepository;

        public PoliticalAllianceService(IPoliticalAllianceRepository politicalAllianceRepository, IMapper mapper,
            IUserRepository userRepository, IPoliticalPartyRepository politicalPartyRepository,
            IElectionRepository electionRepository, IPartyPositionAssignmentRepository assignmentRepository)
        {
            _politicalAllianceRepository = politicalAllianceRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _politicalPartyRepository = politicalPartyRepository;
            _electionRepository = electionRepository;
            _assignmentRepository = assignmentRepository;
        }

        public async Task<Result<PoliticalAllianceDto>> GetByIdAsync(int id, int currentUserId)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser is null || !currentUser.PoliticalPartyId.HasValue)
            {
                return Result<PoliticalAllianceDto>.Failure(error: "User not found or no political party assigned.");
            }

            var entity = await _politicalAllianceRepository.GetAllQueryInclude(["RequesterParty", "TargetParty"])
                                .FirstOrDefaultAsync(pa => pa.Id == id);

            if (entity is null)
            {
                return Result<PoliticalAllianceDto>.Failure(error: "The alliance request does not exist.");
            }

            if (entity.RequesterPartyId != currentUser.PoliticalPartyId && entity.TargetPartyId != currentUser.PoliticalPartyId)
            {
                return Result<PoliticalAllianceDto>.Failure(error: "You do not have permission to view the details of this request.");
            }

            var dto = _mapper.Map<PoliticalAllianceDto>(entity);

            return Result<PoliticalAllianceDto>.Success(dto);
        }

        public async Task<Result<IEnumerable<CurrentAllianceDto>>> GetCurrentAlliancesAsync(int currentUserId)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser is null)
            {
                return Result<IEnumerable<CurrentAllianceDto>>.Failure(error: "You dont have an account.");
            }

            var entities = await _politicalAllianceRepository.GetAllQueryInclude(["RequesterParty", "TargetParty"])
                                    .Where(pa => (pa.RequesterPartyId == currentUser.PoliticalPartyId || pa.TargetPartyId == currentUser.PoliticalPartyId) &&
                                    pa.Status == AllianceStatus.Accepted &&
                                    pa.RequesterParty!.IsActive &&
                                    pa.TargetParty!.IsActive)
                                    .ToListAsync();

            var dtos = entities.Select(pa =>
            {
                var alliedParty = pa.RequesterPartyId == currentUser.PoliticalPartyId ? pa.TargetParty! : pa.RequesterParty!;

                return new CurrentAllianceDto
                {
                    Id = pa.Id,
                    AlliedPartyName = alliedParty.Name,
                    AlliedPartyAcronym = alliedParty.Acronym,
                    AcceptedDate = pa.AcceptedDate!.Value
                };
            });

            return Result<IEnumerable<CurrentAllianceDto>>.Success(dtos);
        }

        public async Task<Result<IEnumerable<PoliticalAllianceDto>>> GetPendingAlliancesAsync(int currentUserId)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser is null)
            {
                return Result<IEnumerable<PoliticalAllianceDto>>.Failure(error: "You dont have an account.");
            }

            var entities = await _politicalAllianceRepository.GetAllQueryInclude(["RequesterParty", "TargetParty"])
                            .Where(pa => pa.Status == AllianceStatus.Pending && pa.TargetPartyId == currentUser.PoliticalPartyId)
                            .ToListAsync();

            return Result<IEnumerable<PoliticalAllianceDto>>.Success(_mapper.Map<IEnumerable<PoliticalAllianceDto>>(entities));
        }

        public async Task<Result<IEnumerable<PoliticalAllianceDto>>> GetSentAlliancesAsync(int currentUserId)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser is null)
            {
                return Result<IEnumerable<PoliticalAllianceDto>>.Failure(error: "You dont have an account.");
            }

            var entities = await _politicalAllianceRepository.GetAllQueryInclude(["RequesterParty", "TargetParty"])
                            .Where(pa => pa.RequesterPartyId == currentUser.PoliticalPartyId)
                            .ToListAsync();

            return Result<IEnumerable<PoliticalAllianceDto>>.Success(_mapper.Map<IEnumerable<PoliticalAllianceDto>>(entities));
        }

        public async Task<Result<PoliticalAllianceDto>> CreateAllianceRequestAsync(CreatePoliticalAllianceDto createDto, int currentUserId)
        {
            if (await _electionRepository.HasActiveElectionAsync())
            {
                return Result<PoliticalAllianceDto>.Failure(error: "You cannot create an alliance request while there is an active election.");
            }

            var currentUser = await _userRepository.GetAllQueryInclude(["PoliticalParty"])
                                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (currentUser is null)
            {
                return Result<PoliticalAllianceDto>.Failure(error: "You dont have an account.");
            }

            if (currentUser.Role != UserRole.PoliticalLeader)
            {
                return Result<PoliticalAllianceDto>.Failure(error: "You need to be a political leader to send alliance requests.");
            }

            if (!currentUser.PoliticalPartyId.HasValue || currentUser.PoliticalParty is null)
            {
                return Result<PoliticalAllianceDto>.Failure(error: "You cannot send alliance requests because you do not have an assigned political party.");
            }

            if (!currentUser.PoliticalParty!.IsActive)
            {
                return Result<PoliticalAllianceDto>.Failure(error: "You cannot send alliance requests because the assigned political party is inactive");
            }

            var party = await _politicalPartyRepository.GetByIdAsync(createDto.PoliticalPartyId);

            if(party is null)
            {
                return Result<PoliticalAllianceDto>.Failure(error: "The target political party doesnt exist.");
            }

            if(currentUser.PoliticalParty.Id == party.Id)
            {
                return Result<PoliticalAllianceDto>.Failure(error: "You cannot create an alliance request towards your own political party.");
            }

            if(!party.IsActive)
            {
                return Result<PoliticalAllianceDto>.Failure(error: "You cannot create an alliance request with an inactive political party.");
            }

            if (await _politicalAllianceRepository.HasActiveAllianceBetweenPartiesAsync(currentUser.PoliticalParty.Id, party.Id))
            {
                return Result<PoliticalAllianceDto>.Failure(error: "There is already an existing alliance with this political party."); 
            }

            if(await _politicalAllianceRepository.HasPendingRequestAsync(currentUser.PoliticalParty.Id, party.Id))
            {
                return Result<PoliticalAllianceDto>.Failure(error: "There is already a pending alliance request sent to this political party.");
            }

            if (await _politicalAllianceRepository.HasPendingRequestAsync(party.Id, currentUser.PoliticalParty.Id))
            {
                return Result<PoliticalAllianceDto>.Failure(error: "There is already a pending alliance request submitted by this political party.");
            }

            var entity = new PoliticalAlliance
            {
                Id = 0,
                RequestDate = DateTime.Now,
                Status = AllianceStatus.Pending,
                RequesterPartyId = currentUser.PoliticalParty.Id,
                TargetPartyId = party.Id
            };

            var result = await _politicalAllianceRepository.AddAsync(entity);

            return Result<PoliticalAllianceDto>.Success(_mapper.Map<PoliticalAllianceDto>(result));
        }

        public async Task<Result> AcceptAllianceRequestAsync(int allianceId, int currentUserId)
        {
            if (await _electionRepository.HasActiveElectionAsync())
            {
                return Result.Failure(error: "You cannot accept an alliance request while there is an active election.");
            }

            var entity = await _politicalAllianceRepository.GetAllQueryInclude(["RequesterParty", "TargetParty"])
                                .FirstOrDefaultAsync(pa => pa.Id == allianceId);

            if (entity is null)
            {
                return Result.Failure(error: "The alliance doesnt exist");
            }

            if(entity.Status != AllianceStatus.Pending)
            {
                return Result.Failure(error: "This alliance request has already been answered.");
            }

            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser is null)
            {
                return Result.Failure(error: "You dont have an account.");
            }

            if (entity.TargetPartyId != currentUser.PoliticalPartyId)
            {
                return Result.Failure(error: "You do not have permission to respond to this partnership request.");
            }

            if(await _politicalAllianceRepository.HasActiveAllianceBetweenPartiesAsync(entity.RequesterPartyId,entity.TargetPartyId))
            {
                return Result.Failure(error: "There is already an existing alliance with this political party.");
            }

            if(!entity.RequesterParty!.IsActive)
            {
                return Result.Failure(error: "The request cannot be accepted because the requesting party is inactive.");
            }

            if(!entity.TargetParty!.IsActive)
            {
                return Result.Failure(error: "The application cannot be accepted because your political party is inactive.");
            }

            entity.Status = AllianceStatus.Accepted;
            entity.AcceptedDate = DateTime.Now;
            await _politicalAllianceRepository.UpdateAsync(entity);

            return Result.Success();
        }

        public async Task<Result> RejectAllianceRequestAsync(int allianceId, int currentUserId)
        {
            if (await _electionRepository.HasActiveElectionAsync())
            {
                return Result.Failure(error: "You cannot reject an alliance request while there is an active election.");
            }

            var entity = await _politicalAllianceRepository.GetByIdAsync(allianceId);

            if (entity is null)
            {
                return Result.Failure(error: "The alliance doesnt exist");
            }

            if (entity.Status != AllianceStatus.Pending)
            {
                return Result.Failure(error: "This alliance request has already been answered.");
            }

            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser is null)
            {
                return Result.Failure(error: "You dont have an account.");
            }

            if (entity.TargetPartyId != currentUser.PoliticalPartyId)
            {
                return Result.Failure(error: "You do not have permission to respond to this alliance request.");
            }

            entity.Status = AllianceStatus.Rejected;
            await _politicalAllianceRepository.UpdateAsync(entity);
            return Result.Success();
        }

        public async Task<Result> DeleteAllianceRequestAsync(int allianceId, int currentUserId)
        {
            if (await _electionRepository.HasActiveElectionAsync())
            {
                return Result.Failure(error: "An alliance request cannot be deleted while there is an active election.");
            }

            var entity = await _politicalAllianceRepository.GetByIdAsync(allianceId);

            if (entity is null)
            {
                return Result.Failure(error: "The selected alliance request does not exist or has already been deleted.");
            }

            if (entity.Status == AllianceStatus.Accepted)
            {
                return Result.Failure(error: "An accepted request cannot be deleted because it has already generated an active alliance. To terminate it, you must delete the alliance from the active alliances list.");
            }

            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser is null || !currentUser.PoliticalPartyId.HasValue)
            {
                return Result.Failure(error: "User not found or no political party assigned.");
            }

            if (entity.RequesterPartyId != currentUser.PoliticalPartyId)
            {
                return Result.Failure(error: "You do not have permission to delete this alliance request.");
            }

            await _politicalAllianceRepository.DeleteAsync(entity);
            return Result.Success();
        }

        public async Task<Result> DeleteAllianceAsync(int allianceId, int currentUserId)
        {
            if (await _electionRepository.HasActiveElectionAsync())
            {
                return Result.Failure(error: "A political alliance cannot be deleted while there is an active election.");
            }

            var entity = await _politicalAllianceRepository.GetByIdAsync(allianceId);

            if (entity is null)
            {
                return Result.Failure(error: "The selected political alliance does not exist or has already been deleted.");
            }

            if (entity.Status != AllianceStatus.Accepted)
            {
                return Result.Failure(error: "The selected alliance is not currently active.");
            }

            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser is null || !currentUser.PoliticalPartyId.HasValue)
            {
                return Result.Failure(error: "User not found or no political party assigned.");
            }

            if (entity.RequesterPartyId != currentUser.PoliticalPartyId && entity.TargetPartyId != currentUser.PoliticalPartyId)
            {
                return Result.Failure(error: "You do not have permission to delete this political alliance.");
            }

            var hasAlliedCandidatesAssigned = await _assignmentRepository
                        .HasAlliedCandidatesAssignedAsync(entity.RequesterPartyId, entity.TargetPartyId);

            if (hasAlliedCandidatesAssigned)
            {
                return Result.Failure(error: "This alliance cannot be deleted because there are allied candidates assigned between these parties. First, the corresponding assignments must be deleted from the Assign candidate to position module.");
            }

            await _politicalAllianceRepository.DeleteAsync(entity);
            return Result.Success();
        }
    }
}
