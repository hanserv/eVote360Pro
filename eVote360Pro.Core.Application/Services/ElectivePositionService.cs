using eVote360Pro.Core.Application.DTOs.ElectivePosition;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Core.Domain.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Core.Application.Services
{
    public class ElectivePositionService : IElectivePositionService
    {
        private readonly IElectivePositionRepository _electivePositionRepository;
        private readonly IElectionRepository _electionRepository;
        private readonly IMapper _mapper;
        private readonly IPartyPositionAssignmentRepository _partyPositionAssignmentRepository;
        private readonly IUserRepository _userRepository;
        public ElectivePositionService(IElectivePositionRepository electivePositionRepository, IElectionRepository electionRepository,
            IMapper mapper, IPartyPositionAssignmentRepository partyPositionAssignmentRepository, IUserRepository userRepository)
        {
            _electivePositionRepository = electivePositionRepository;
            _electionRepository = electionRepository;
            _mapper = mapper;
            _partyPositionAssignmentRepository = partyPositionAssignmentRepository;
            _userRepository = userRepository;
        }

        public async Task<Result<IEnumerable<ElectivePositionDto>>> GetAllAsync()
        {
            var entities = _mapper.Map<IEnumerable<ElectivePositionDto>>(await _electivePositionRepository.GetAllAsync());
            return Result<IEnumerable<ElectivePositionDto>>.Success(entities);
        }

        public async Task<Result<IEnumerable<ElectivePositionDto>>> GetAllActivesAsync(int? includeId = null)
        {
            var query = _electivePositionRepository.GetAllQuery()
                            .Where(ep => ep.IsActive || ep.Id == includeId);

            var entities = _mapper.Map<IEnumerable<ElectivePositionDto>>(query.ToListAsync());
            return Result<IEnumerable<ElectivePositionDto>>.Success(entities);
        }

        public async Task<Result<ElectivePositionDto>> GetByIdAsync(int id)
        {
            var entity = await _electivePositionRepository.GetByIdAsync(id);

            if(entity is null)
            {
                return Result<ElectivePositionDto>.Failure(error: "The elective position doesnt exist.");
            }

            return Result<ElectivePositionDto>.Success(_mapper.Map<ElectivePositionDto>(entity));
        }

        public async Task<Result<ElectivePositionDto>> AddAsync(CreateElectivePositionDto createDto)
        {
            if(await _electionRepository.HasActiveElectionAsync())
            {
                return Result<ElectivePositionDto>.Failure(error: "An elected position cannot be created while an active election exists.");
            }

            createDto.Name = createDto.Name.Trim();

            var isNameUsed= await _electivePositionRepository.GetAllQuery()
                            .AnyAsync(ep => ep.Name == createDto.Name);

            if(isNameUsed)
            {
                return Result<ElectivePositionDto>.Failure(error: "There is already an elected position registered with this name.");
            }

            var entity = _mapper.Map<ElectivePosition>(createDto);
            entity.IsActive = true;

            var result = await _electivePositionRepository.AddAsync(entity);

            if(result is null)
            {
                return Result<ElectivePositionDto>.Failure(error: "There was an error in the elective position register.");
            }

            return Result<ElectivePositionDto>.Success(_mapper.Map<ElectivePositionDto>(result));
        }

        public async Task<Result> UpdateAsync(UpdateElectivePositionDto updateDto)
        {
            var entity = await _electivePositionRepository.GetAllQueryInclude(["Votes"])
                            .FirstOrDefaultAsync(ep => ep.Id == updateDto.Id);

            if(entity is null)
            {
                return Result.Failure(error: "The elective position doesnt exist");
            }

            if (await _electionRepository.HasActiveElectionAsync())
            {
                return Result.Failure(error: "An elected position cannot be edited while an election is active.");
            }

            updateDto.Name = updateDto.Name.Trim();

            if (entity.Name != updateDto.Name)
            {
                if(entity.Votes is not null && entity.Votes.Any())
                {
                    return Result.Failure(error: "The name of this elected position cannot be changed because it has already been used in an election.");
                }
            }

            var isNameUsed = await _electivePositionRepository.GetAllQuery()
                            .AnyAsync(ep => ep.Name == updateDto.Name && ep.Id != entity.Id);

            if (isNameUsed)
            {
                return Result.Failure(error: "There is already an elected position registered with this name.");
            }

            entity.Name = updateDto.Name;
            entity.Description = updateDto.Description;

            await _electivePositionRepository.UpdateAsync(entity);
            return Result.Success();
        }

        public async Task<Result> ChangeStatusAsync(int id, bool active)
        {
            var entity = await _electivePositionRepository.GetByIdAsync(id);

            if (entity is null)
            {
                return Result.Failure(error: "The elective position doesnt exist");
            }

            var hasActiveElection = await _electionRepository.HasActiveElectionAsync();

            if(active)
            {
                return await ActivateAsync(entity, hasActiveElection);
            }

            return await InactivateAsync(entity, hasActiveElection);
        }

        #region Private Methods
        private async Task<Result> ActivateAsync(ElectivePosition entity, bool hasActiveElection)
        {
            if (hasActiveElection)
            {
                return Result.Failure(error: "An elected position cannot be activated while an election is active.");
            }

            if (entity.IsActive)
            {
                return Result.Failure(error: "This elective position is already active.");
            }

            var isNameUsedByActive = await _electivePositionRepository.GetAllQuery()
                            .AnyAsync(ep => ep.Name == entity.Name && ep.IsActive && ep.Id != entity.Id);

            if (isNameUsedByActive)
            {
                return Result.Failure(error: "Cannot activate this position because there is already an active position with the same name.");
            }

            entity.IsActive = true;
            await _electivePositionRepository.UpdateAsync(entity);
            return Result.Success();
        }

        private async Task<Result> InactivateAsync(ElectivePosition entity, bool hasActiveElection)
        {
            if (hasActiveElection)
            {
                return Result.Failure(error: "An elected position cannot be deactivated while an election is active.");
            }

            if (!entity.IsActive)
            {
                return Result.Failure(error: "This elective position is already inactive.");
            }

            var hasCandidatesAssigned = await _partyPositionAssignmentRepository.GetAllQuery()
                                    .AnyAsync(ppa => ppa.ElectivePositionId == entity.Id &&
                                        ppa.Candidate != null && ppa.Candidate.IsActive);

            if (hasCandidatesAssigned)
            {
                return Result.Failure(error: "This elective position cannot be deactivated because it has active candidates assigned.");
            }

            entity.IsActive = false;
            await _electivePositionRepository.UpdateAsync(entity);
            return Result.Success();
        }
        #endregion
    }
}
