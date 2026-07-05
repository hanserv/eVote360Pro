using eVote360Pro.Core.Application.DTOs.User;
using eVote360Pro.Core.Application.Helpers;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Core.Domain.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Core.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IElectionRepository _electionRepository;

        public UserService(IUserRepository userRepository, IMapper mapper,
            IElectionRepository electionRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _electionRepository = electionRepository;
        }

        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.LoginAsync(loginDto.UserName,PasswordEncryptation.ComputeSha256Hash(loginDto.Password));

            if(user is null)
            {
                return Result<UserDto>.Failure(error: "The username or password are incorrect.");
            }

            if(!user.IsActive)
            {
                return Result<UserDto>.Failure(error: "The user is inactive.");
            }

            if (!Enum.IsDefined(typeof(UserRole), user.Role))
            {
                return Result<UserDto>.Failure(error: "The user's role is not valid in the system.");
            }

            if(user.Role == UserRole.PoliticalLeader)
            {
                if(user.PoliticalParty is null)
                {
                    return Result<UserDto>.Failure(error: "You do not have an assigned political party, therefore you cannot log in. Please contact an administrator.");
                }

                if(!user.PoliticalParty.IsActive)
                {
                    return Result<UserDto>.Failure(error: "Your assigned political party is currently inactive.");
                }
            }

            return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
        }

        public async Task<Result<IEnumerable<UserDto>>> GetAllAsync()
        {
            var entities = _mapper.Map<IEnumerable<UserDto>>(await _userRepository.GetAllAsync());
            return Result<IEnumerable<UserDto>>.Success(entities);
        }

        public async Task<Result<IEnumerable<UserDto>>> GetAllAvailableLeadersAsync()
        {
            var entities = await _userRepository.GetAllQuery()
                                .Where(u => u.IsActive && u.Role == UserRole.PoliticalLeader && !u.PoliticalPartyId.HasValue)
                                .ToListAsync();

            var entityDtos = _mapper.Map<IEnumerable<UserDto>>(entities);
            return Result<IEnumerable<UserDto>>.Success(entityDtos);
        }

        public async Task<Result<UserDto>> GetByIdAsync(int id)
        {
            var entity = await _userRepository.GetByIdAsync(id);

            if (entity is null)
            {
                return Result<UserDto>.Failure(error: "The user doesnt exist.");
            }

            return Result<UserDto>.Success(_mapper.Map<UserDto>(entity));
        }

        public async Task<Result<UserDto>> AddAsync(CreateUserDto createDto)
        {
            createDto.Username = createDto.Username.Trim();
            createDto.Email = createDto.Email.Trim();

            var isUserNameUsed = await _userRepository.GetAllQuery()
                                .AnyAsync(u => u.Username == createDto.Username);

            if(isUserNameUsed)
            {
                return Result<UserDto>.Failure(error: "There is already a registered user with this username.");
            }

            var isEmailUsed = await _userRepository.GetAllQuery()
                                .AnyAsync(u => u.Email == createDto.Email);

            if (isEmailUsed)
            {
                return Result<UserDto>.Failure(error: "There is already a registered user with this email address.");
            }

            var passwordValidation = ValidatePassword(createDto.Password);

            if(!passwordValidation.IsSuccess)
            {
                return Result<UserDto>.Failure(passwordValidation.Error!);
            }

            if (!Enum.IsDefined(typeof(UserRole), createDto.Role))
            {
                return Result<UserDto>.Failure(error: "You must select a valid role for the user.");
            }

            if (await _electionRepository.HasActiveElectionAsync())
            {
                return Result<UserDto>.Failure(error: "A user cannot be created while an election is active.");
            }

            var entity = _mapper.Map<User>(createDto);
            entity.IsActive = true;
            entity.PasswordHash = PasswordEncryptation.ComputeSha256Hash(createDto.Password);

            var result = await _userRepository.AddAsync(entity);
            return Result<UserDto>.Success(_mapper.Map<UserDto>(result));
        }
        
        public async Task<Result> UpdateAsync(UpdateUserDto updateDto, int currentUserId)
        {

            var entity = await _userRepository.GetByIdAsync(updateDto.Id);

            if(entity is null)
            {
                return Result.Failure(error: "The user doesnt exist.");
            }

            if (currentUserId == updateDto.Id && entity.Role != updateDto.Role)
            {
                return Result.Failure(error: "You cannot change your own role while authenticated.");
            }

            if (await _electionRepository.HasActiveElectionAsync())
            {
                return Result<UserDto>.Failure(error: "A user cannot be edited while an election is active.");
            }

            updateDto.Username = updateDto.Username.Trim();
            updateDto.Email = updateDto.Email.Trim();

            if (entity.Username != updateDto.Username)
            {
                var isUserNameUsed = await _userRepository.GetAllQuery()
                                .AnyAsync(u => u.Username == updateDto.Username && u.Id != entity.Id);

                if (isUserNameUsed)
                {
                    return Result.Failure(error: "There is already a registered user with this username.");
                }
            }

            if (entity.Email != updateDto.Email)
            {
                var isEmailUsed = await _userRepository.GetAllQuery()
                                .AnyAsync(u => u.Email == updateDto.Email && u.Id != entity.Id);

                if (isEmailUsed)
                {
                    return Result.Failure(error: "There is already a registered user with this email address.");
                }
            }

            if (!Enum.IsDefined(typeof(UserRole), updateDto.Role))
            {
                return Result.Failure(error: "You must select a valid role for the user.");
            }

            if (entity.Role == UserRole.PoliticalLeader && entity.PoliticalPartyId.HasValue)
            {
                if (updateDto.Role == UserRole.Admin)
                {
                    return Result.Failure(error: "You cannot change the role of this user because they have a political party assigned as a leader.");
                }
            }

            if (entity.Role == UserRole.Admin && entity.IsActive)
            {
                if (updateDto.Role != UserRole.Admin)
                {
                    var activeAdminsCount = await _userRepository.GetAllQuery()
                        .CountAsync(u => u.Role == UserRole.Admin && u.IsActive);

                    if (activeAdminsCount <= 1)
                    {
                        return Result.Failure(error: "This user cannot be modified because it is the only active administrator in the system.");
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Password))
            {
                var passwordValidation = ValidatePassword(updateDto.Password);

                if (!passwordValidation.IsSuccess)
                {
                    return Result.Failure(passwordValidation.Error!);
                }
            }

            entity.Name = updateDto.Name.Trim();
            entity.LastName = updateDto.LastName.Trim();
            entity.Username = updateDto.Username;
            entity.Email = updateDto.Email;
            entity.PasswordHash = string.IsNullOrWhiteSpace(updateDto.Password) ? entity.PasswordHash : PasswordEncryptation.ComputeSha256Hash(updateDto.Password);
            entity.Role = updateDto.Role;

            await _userRepository.UpdateAsync(entity);
            return Result.Success();
        }

        public async Task<Result> ChangeStatusAsync(int id, bool active, int currentUserId)
        {
            var entity = await _userRepository.GetByIdAsync(id);

            if (entity is null)
            {
                return Result.Failure(error: "The user doesnt exist.");
            }

            var hasActiveElection = await _electionRepository.HasActiveElectionAsync();

            if (active)
            {
                return await ActivateAsync(entity, hasActiveElection);
            }

            return await InactivateAsync(entity, hasActiveElection, currentUserId);
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _userRepository.GetAllQuery().AnyAsync(u => u.Username == username);
        }

        #region Private Methods 
        private Result ValidatePassword(string password)
        {
            if(password.Length < 8)
            {
                return Result.Failure(error: "The password must be at least 8 characters long.");
            }

            if(!password.Any(char.IsLetter))
            {
                return Result.Failure(error: "The password must contain at least one letter.");
            }

            if (!password.Any(char.IsNumber))
            {
                return Result.Failure(error: "The password must contain at least one number.");
            }

            return Result.Success();
        }

        private async Task<Result> ActivateAsync(User entity, bool hasActiveElection)
        {
            if (hasActiveElection)
            {
                return Result.Failure(error: "A user cannot be activated while there is an active election.");
            }

            if (entity.IsActive)
            {
                return Result.Failure(error: "This user is already active.");
            }

            entity.IsActive = true;
            await _userRepository.UpdateAsync(entity);
            return Result.Success();
        }

        private async Task<Result> InactivateAsync(User entity, bool hasActiveElection, int currentUserId)
        {
            if (hasActiveElection)
            {
                return Result.Failure(error: "A user cannot be deactivated while an election is active.");
            }

            if (!entity.IsActive)
            {
                return Result.Failure(error: "This user is already inactive.");
            }

            if (entity.Id == currentUserId)
            {
                return Result.Failure(error: "You cannot deactivate your own user while authenticated.");
            }

            if (entity.Role == UserRole.Admin)
            {
                var activeAdminsCount = await _userRepository.GetAllQuery()
                    .CountAsync(u => u.Role == UserRole.Admin && u.IsActive);

                if (activeAdminsCount <= 1)
                {
                    return Result.Failure(error: "This user cannot be deactivated because it is the only active administrator on the system.");
                }
            }

            entity.IsActive = false;
            await _userRepository.UpdateAsync(entity);
            return Result.Success();
        }
        #endregion
    }
}
