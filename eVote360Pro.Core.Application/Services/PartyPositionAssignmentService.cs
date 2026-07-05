using eVote360Pro.Core.Application.DTOs.Candidates;
using eVote360Pro.Core.Application.DTOs.ElectivePosition;
using eVote360Pro.Core.Application.DTOs.PartyPositionAssignment;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Core.Domain.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Core.Application.Services
{
    public class PartyPositionAssignmentService : IPartyPositionAssignmentService
    {
        private readonly IPartyPositionAssignmentRepository _partyPositionAssignmentRepository;
        private readonly IElectionRepository _electionRepository;
        private readonly IElectivePositionRepository _electivePositionRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPoliticalAllianceRepository _politicalAllianceRepository;

        public PartyPositionAssignmentService(IPartyPositionAssignmentRepository partyPositionAssignmentRepository, IElectionRepository electionRepository,
            IElectivePositionRepository electivePositionRepository, ICandidateRepository candidateRepository,
            IUserRepository userRepository, IMapper mapper, IPoliticalAllianceRepository politicalAllianceRepository)
        {
            _partyPositionAssignmentRepository = partyPositionAssignmentRepository;
            _electionRepository = electionRepository;
            _electivePositionRepository = electivePositionRepository;
            _candidateRepository = candidateRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _politicalAllianceRepository = politicalAllianceRepository;
        }

        public async Task<Result<IEnumerable<AssignmentSummaryDto>>> GetAllSummaryAsync(int currentUserId)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser is null || !currentUser.PoliticalPartyId.HasValue)
            {
                return Result<IEnumerable<AssignmentSummaryDto>>.Failure(error: "The authenticated user doesnt exist or no political party assigned.");
            }

            var assignments = await _partyPositionAssignmentRepository
                    .GetAllQueryInclude(["Candidate", "Candidate.PoliticalParty", "ElectivePosition"])
                    .Where(a => a.AssigningPartyId == currentUser.PoliticalPartyId.Value)
                    .ToListAsync();

            var dtos = assignments.Select(a => new AssignmentSummaryDto
            {
                Id = a.Id,
                CandidateName = a.Candidate!.Name, 
                CandidateLastName = a.Candidate!.LastName,
                OriginPartyName = a.Candidate.PoliticalParty!.Name,
                ElectivePositionName = a.ElectivePosition!.Name,
                CandidacyType = a.Candidate.PoliticalPartyId == currentUser.PoliticalPartyId.Value ? "Own" : "Allied"
            }).ToList();

            return Result<IEnumerable<AssignmentSummaryDto>>.Success(dtos);
        }

        public async Task<Result<PartyPositionAssignmentDto>> GetByIdAsync(int id, int currentUserId)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser is null || !currentUser.PoliticalPartyId.HasValue)
            {
                return Result<PartyPositionAssignmentDto>.Failure(error: "The authenticated user doesnt exist or no political party assigned.");
            }

            var entity = await _partyPositionAssignmentRepository.GetAllQueryInclude(["Candidate", "ElectivePosition"])
                     .FirstOrDefaultAsync(a => a.Id == id);

            if (entity is null)
            {
                return Result<PartyPositionAssignmentDto>.Failure(error: "The assignment does not exist or has already been deleted.");
            }

            if (entity.AssigningPartyId != currentUser.PoliticalPartyId.Value)
            {
                return Result<PartyPositionAssignmentDto>.Failure(error: "You do not have permission to access this assignment.");
            }

            var dto = _mapper.Map<PartyPositionAssignmentDto>(entity);
            return Result<PartyPositionAssignmentDto>.Success(dto);
        }

        public async Task<Result<IEnumerable<CandidateDto>>> GetAvailableCandidatesForAssignmentAsync(int currentUserId)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser is null || !currentUser.PoliticalPartyId.HasValue)
            {
                return Result<IEnumerable<CandidateDto>>.Failure(error: "The authenticated user doesnt exist or no political party assigned.");
            }

            var availableCandidates = await _candidateRepository.GetAvailableCandidatesForPartyAsync(currentUser.PoliticalPartyId.Value);

            var dtos = _mapper.Map<IEnumerable<CandidateDto>>(availableCandidates);
            return Result<IEnumerable<CandidateDto>>.Success(dtos);
        }

        public async Task<Result<IEnumerable<ElectivePositionDto>>> GetAvailablePositionsForAssignmentAsync(int currentUserId)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser is null || !currentUser.PoliticalPartyId.HasValue)
            {
                return Result<IEnumerable<ElectivePositionDto>>.Failure(error: "The authenticated user doesnt exist or no political party assigned.");
            }

            var availablePositions = await _electivePositionRepository.GetAvailablePositionsForPartyAsync(currentUser.PoliticalPartyId.Value);

            var dtos = _mapper.Map<IEnumerable<ElectivePositionDto>>(availablePositions);
            return Result<IEnumerable<ElectivePositionDto>>.Success(dtos);
        }

        public async Task<Result<PartyPositionAssignmentDto>> AddAsync(CreatePartyPositionAssignmentDto createDto, int currentUserId)
        {
            if (await _electionRepository.HasActiveElectionAsync())
            {
                return Result<PartyPositionAssignmentDto>.Failure(error: "Candidates cannot be assigned to positions while an election is active.");
            }

            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser is null || !currentUser.PoliticalPartyId.HasValue)
            {
                return Result<PartyPositionAssignmentDto>.Failure(error: "The authenticated user doesnt exist.");
            }

            var candidate = await _candidateRepository.GetByIdAsync(createDto.CandidateId);

            if (candidate is null || !candidate.IsActive)
            {
                return Result<PartyPositionAssignmentDto>.Failure(error: "The candidate doesnt exists or is inactive.");
            }

            var position = await _electivePositionRepository.GetByIdAsync(createDto.ElectivePositionId);

            if (position is null || !position.IsActive)
            {
                return Result<PartyPositionAssignmentDto>.Failure(error: "The elected position does not exist or is inactive");
            }

            if (await _partyPositionAssignmentRepository.HasAssignmentInPartyAsync(createDto.CandidateId, currentUser.PoliticalPartyId.Value))
            {
                return Result<PartyPositionAssignmentDto>.Failure(error: "This candidate has already been assigned a position within the party.");
            }

            if (await _partyPositionAssignmentRepository.IsPositionFilledInPartyAsync(createDto.ElectivePositionId, currentUser.PoliticalPartyId.Value))
            {
                return Result<PartyPositionAssignmentDto>.Failure(error: "This elected position already has a candidate assigned within the party.");
            }

            if (candidate.PoliticalPartyId != currentUser.PoliticalPartyId.Value)
            {
                // Allied candidate
                var hasActiveAlliance = await _politicalAllianceRepository.HasActiveAllianceBetweenPartiesAsync(currentUser.PoliticalPartyId.Value,candidate.PoliticalPartyId);

                if (!hasActiveAlliance)
                {
                    return Result<PartyPositionAssignmentDto>.Failure(error: "There is no current alliance with this candidate's party.");
                }

                var originPositionId = await _partyPositionAssignmentRepository.GetAssignedPositionInPartyAsync(candidate.Id,candidate.PoliticalPartyId);

                if (!originPositionId.HasValue)
                {
                    return Result<PartyPositionAssignmentDto>.Failure(error: "This allied candidate does not have an assigned position in his original party.");
                }

                if (originPositionId.Value != createDto.ElectivePositionId)
                {
                    return Result<PartyPositionAssignmentDto>.Failure(error: "This candidate in his original party aspires to a different position than the one selected.");
                }
            }

            var entity = new PartyPositionAssignment
            {
                Id = 0,
                CandidateId = createDto.CandidateId,
                ElectivePositionId = createDto.ElectivePositionId,
                AssigningPartyId = currentUser.PoliticalPartyId.Value
            };

            var result = await _partyPositionAssignmentRepository.AddAsync(entity);

            return Result<PartyPositionAssignmentDto>.Success(_mapper.Map<PartyPositionAssignmentDto>(result));
        }

        public async Task<Result> DeleteAsync(int assignmentId, int currentUserId)
        {
            if (await _electionRepository.HasActiveElectionAsync())
            {
                return Result.Failure(error: "An assignment cannot be removed while an active election exists.");
            }

            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser is null)
            {
                return Result.Failure(error: "The authenticated user doesnt exist."); // TODO: Refactor msg
            }

            if (currentUser.Role != UserRole.PoliticalLeader)
            {
                return Result.Failure(error: "You need to be a political leader to delete a assignment.");
            }

            if (!currentUser.PoliticalPartyId.HasValue)
            {
                return Result.Failure(error: "The authenticated user doesnt have an assigned political party.");
            }

            var entity = await _partyPositionAssignmentRepository.GetByIdAsync(assignmentId);

            if (entity is null)
            {
                return Result.Failure(error: "The selected assignment does not exist or has already been deleted.");
            }

            if (entity.AssigningPartyId != currentUser.PoliticalPartyId.Value)
            {
                return Result.Failure(error: "You do not have permission to delete this assignment.");
            }

            await _partyPositionAssignmentRepository.DeleteAsync(entity);
            return Result.Success();
        }
    }
}
