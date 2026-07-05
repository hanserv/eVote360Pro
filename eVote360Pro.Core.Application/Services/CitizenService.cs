using eVote360Pro.Core.Application.DTOs.Citizen;
using eVote360Pro.Core.Application.Helpers;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Core.Domain.Interfaces;
using MapsterMapper;

namespace eVote360Pro.Core.Application.Services
{
    public class CitizenService : ICitizenService
    {
        private readonly ICitizenRepository _citizenRepository;
        private readonly IMapper _mapper;
        private readonly IElectionRepository _electionRepository;
        public CitizenService(ICitizenRepository citizenRepository, IMapper mapper, 
            IElectionRepository electionRepository)
        {
            _citizenRepository = citizenRepository;
            _mapper = mapper;
            _electionRepository = electionRepository;
        }

        public async Task<Result<IEnumerable<CitizenDto>>> GetAllAsync()
        {
            var entities = _mapper.Map<IEnumerable<CitizenDto>>(await _citizenRepository.GetAllAsync());
            return Result<IEnumerable<CitizenDto>>.Success(entities);
        }

        public async Task<Result<CitizenDto>> GetByIdAsync(int id)
        {
            var entity = await _citizenRepository.GetByIdAsync(id);

            if(entity is null)
            {
                return Result<CitizenDto>.Failure(error: "The citizen doesnt exist");
            }

            return Result<CitizenDto>.Success(_mapper.Map<CitizenDto>(entity));
        }

        public async Task<Result<CitizenDto>> AddAsync(CreateCitizenDto createDto)
        {
            if (await _electionRepository.HasActiveElectionAsync())
            {
                return Result<CitizenDto>.Failure(error: "A citizen cannot be created while there is an active election.");
            }

            createDto.DocumentId = new string(createDto.DocumentId.Trim().Where(char.IsDigit).ToArray());
            createDto.Name = createDto.Name.Trim();
            createDto.LastName = createDto.LastName.Trim();

            if(!EmailValidator.IsAValidEmail(createDto.Email))
            {
                return Result<CitizenDto>.Failure(error: "You must enter a valid email address.");
            }

            var isEmailAvailable = await _citizenRepository.IsEmailAvailableAsync(createDto.Email);

            if (!isEmailAvailable)
            {
                return Result<CitizenDto>.Failure(error: "There is already a registered citizen with this email address.");
            }

            var isDocumentIdAvailable = await _citizenRepository.IsDocumentIdAvailableAsync(createDto.DocumentId);

            if (!isDocumentIdAvailable)
            {
                return Result<CitizenDto>.Failure(error: "There is already a registered citizen with this identity document number.");
            }

            var entity = _mapper.Map<Citizen>(createDto);
            entity.IsActive = true;

            var result = await _citizenRepository.AddAsync(entity);

            return Result<CitizenDto>.Success(_mapper.Map<CitizenDto>(result));
        }

        public async Task<Result> UpdateAsync(UpdateCitizenDto updateDto)
        {
            if (await _electionRepository.HasActiveElectionAsync())
            {
                return Result.Failure(error: "A citizen cannot be edited while there is an active election.");
            }

            var entity = await _citizenRepository.GetByIdAsync(updateDto.Id);

            if (entity is null)
            {
                return Result.Failure(error: "The citizen doesnt exist");
            }

            updateDto.DocumentId = new string(updateDto.DocumentId.Trim().Where(char.IsDigit).ToArray());
            updateDto.Name = updateDto.Name.Trim();
            updateDto.LastName = updateDto.LastName.Trim();

            if (await _citizenRepository.HasVotedAsync(entity.Id))
            {
                if (entity.DocumentId != updateDto.DocumentId)
                {
                    return Result.Failure(error: "This citizen's identity document id cannot be changed because they have already participated in an election.");
                }
            }

            if (!EmailValidator.IsAValidEmail(updateDto.Email))
            {
                return Result.Failure(error: "You must enter a valid email address.");
            }


            var isEmailAvailable = await _citizenRepository.IsEmailAvailableAsync(updateDto.Email,updateDto.Id);

            if (!isEmailAvailable)
            {
                return Result.Failure(error: "There is already a registered citizen with this email address.");
            }

            var isDocumentIdAvailable = await _citizenRepository.IsDocumentIdAvailableAsync(updateDto.DocumentId, updateDto.Id);

            if (!isDocumentIdAvailable)
            {
                return Result.Failure(error: "There is already a registered citizen with this identity document number.");
            }

            entity.Name = updateDto.Name;
            entity.LastName = updateDto.LastName;
            entity.Email = updateDto.Email;
            entity.DocumentId = updateDto.DocumentId;

            await _citizenRepository.UpdateAsync(entity);
            return Result.Success();
        }

        public async Task<Result> ChangeStatusAsync(int id, bool active)
        {
            var entity = await _citizenRepository.GetByIdAsync(id);

            if (entity is null)
            {
                return Result.Failure(error: "The citizen doesnt exist");
            }

            var hasActiveElection = await _electionRepository.HasActiveElectionAsync();

            if(active)
            {
                return await ActivateAsync(entity, hasActiveElection);
            }

            return await InactivateAsync(entity, hasActiveElection);
        }

        #region Private Methods 
        private async Task<Result> ActivateAsync(Citizen entity, bool hasActiveElection)
        {
            if (hasActiveElection)
            {
                return Result.Failure(error: "A citizen cannot be activated while there is an active election.");
            }

            if (entity.IsActive)
            {
                return Result.Failure(error: "The citizen is already active.");
            }

            entity.IsActive = true;
            await _citizenRepository.UpdateAsync(entity);
            return Result.Success();
        }

        private async Task<Result> InactivateAsync(Citizen entity, bool hasActiveElection)
        {
            if (hasActiveElection)
            {
                return Result.Failure(error: "A citizen cannot be deactivated while an election is active.");
            }

            if (!entity.IsActive)
            {
                return Result.Failure(error: "This citizen is already inactive.");
            }

            entity.IsActive = false;
            await _citizenRepository.UpdateAsync(entity);
            return Result.Success();
        }
        #endregion
    }
}
