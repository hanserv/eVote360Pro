using eVote360Pro.Core.Application.DTOs.Citizen;
using eVote360Pro.Core.Application.DTOs.Email;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Core.Domain.Interfaces;
using MapsterMapper;

namespace eVote360Pro.Core.Application.Services
{
    public class VoterIdentityService : IVoterIdentityService
    {
        private readonly IElectionRepository _electionRepository;
        private readonly ICitizenRepository _citizenRepository;
        private readonly IElectionParticipationRepository _electionParticipationRepository;
        private readonly IMapper _mapper;
        private readonly IOcrService _ocrService;
        private readonly IEmailService _emailService;
        private readonly IVerificationCodeRepository _verificationCodeRepository;

        public VoterIdentityService(IElectionRepository electionRepository, ICitizenRepository citizenRepository,
            IElectionParticipationRepository electionParticipationRepository, IMapper mapper,
            IOcrService ocrService, IEmailService emailService, IVerificationCodeRepository verificationCodeRepository)
        {
            _electionRepository = electionRepository;
            _citizenRepository = citizenRepository;
            _electionParticipationRepository = electionParticipationRepository;
            _mapper = mapper;
            _ocrService = ocrService;
            _emailService = emailService;
            _verificationCodeRepository = verificationCodeRepository;
        }

        public async Task<Result<CitizenDto>> ValidateDocumentAsync(string documentId)
        {
            if (string.IsNullOrWhiteSpace(documentId))
            {
                return Result<CitizenDto>.Failure("The identity document number is required.");
            }

            documentId = new string(documentId.Where(char.IsDigit).ToArray());

            var activeElection = await _electionRepository.GetActiveElectionAsync();
            if (activeElection is null)
            {
                return Result<CitizenDto>.Failure("There is no electoral process underway at the moment.");
            }

            var citizen = await _citizenRepository.GetByDocumentIdAsync(documentId);
            if (citizen == null)
            {
                return Result<CitizenDto>.Failure("There is no registered citizen with this document number.");
            }

            if (!citizen.IsActive)
            {
                return Result<CitizenDto>.Failure("This citizen is inactive and cannot participate in the voting process.");
            }

            var hasVoted = await _electionParticipationRepository.HasCitizenFinalizedAsync(citizen.Id, activeElection.Id);
            if (hasVoted)
            {
                return Result<CitizenDto>.Failure("He has already exercised his right to vote.");
            }

            return Result<CitizenDto>.Success(_mapper.Map<CitizenDto>(citizen));
        }

        public async Task<Result> ProcessDocumentIdAndGenerateCodeAsync(string documentId, Stream imageStream, string fileName)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(fileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return Result.Failure("Invalid file extension. Only image files .jpg, .jpeg and .png are allowed.");
            }

            documentId = new string(documentId.Where(char.IsDigit).ToArray()); 

            var extractedDocumentId = await _ocrService.ExtractDocumentIdFromImageAsync(imageStream);

            if(string.IsNullOrWhiteSpace(extractedDocumentId))
            {
                return Result.Failure("The document number could not be read correctly in the uploaded image. Please upload a clearer image.");
            }

            if (extractedDocumentId != documentId)
            {
                return Result.Failure("The data extracted from the photo does not match the data previously entered by the voter.");
            }

            var citizen = await _citizenRepository.GetByDocumentIdAsync(documentId);
            if (string.IsNullOrWhiteSpace(citizen!.Email))
            {
                return Result.Failure("This citizen does not have a registered email address. It is not possible to continue with identity verification.");
            }

            var activeElection = await _electionRepository.GetActiveElectionAsync();

            if(activeElection is null)
            {
                return Result.Failure(error: "There is no electoral process underway at the moment.");
            }

            var random = new Random();
            string newCode = random.Next(100000, 999999).ToString();

            var verificationCode = new VerificationCode
            {
                Id = 0,
                CitizenId = citizen.Id,
                ElectionId = activeElection!.Id,
                Code = newCode,
                GenerationDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddMinutes(5),
                IsUsed = false
            };

            await _verificationCodeRepository.AddAsync(verificationCode);
            
            var result = await _emailService.SendAsync(new EmailRequestDto
            {
                To = citizen.Email,
                Subject = "Verification code to vote",
                BodyHtml = $"""
                <p>Hello <strong>{citizen.Name} {citizen.LastName}</strong>,</p> 
                <p>Your verification code to continue with the voting process is: </p> 
                    <div style="text-align: center; margin: 20px 0;"> 
                        <span style=" display: inline-block; background-color: #fff3cd; border: 1px solid #ffc107; padding: 10px 20px; border-radius: 6px; font-size: 24px; font-weight: bold; letter-spacing: 4px;">
                            [{newCode}]
                        </span> 
                    </div> 
                <p>This code will be valid for 5 minutes.</p> <p>If you did not initiate this process, ignore this message.</p>
                """
            });

            if(!result.IsSuccess)
            {
                return Result.Failure("The verification code could not be sent. Please try again later.");
            }

            return Result.Success();
        }

        public async Task<Result> ValidateVerificationCodeAsync(string documentId, string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Result.Failure("You must enter the verification code sent to your email.");
            }

            var activeElection = await _electionRepository.GetActiveElectionAsync();
            if (activeElection is null)
            {
                return Result.Failure("There is no electoral process underway at the moment.");
            }

            documentId = new string(documentId.Where(char.IsDigit).ToArray());
            var citizen = await _citizenRepository.GetByDocumentIdAsync(documentId);

            if (citizen is null)
            {
                return Result.Failure("There is no registered citizen with this document number.");
            }

            var latestCode = await _verificationCodeRepository.GetLatestCodeAsync(citizen.Id, activeElection.Id);

            if (latestCode is null || latestCode.Code != code)
            {
                return Result.Failure("The verification code entered is not valid.");
            }

            if (latestCode.IsUsed)
            {
                return Result.Failure("This verification code has already been used.");
            }

            if (latestCode.ExpirationDate < DateTime.Now) 
            {
                return Result.Failure("The verification code has expired. Request a new code to continue.");
            }

            latestCode.IsUsed = true;
            await _verificationCodeRepository.UpdateAsync(latestCode);

            return Result.Success();
        }
    }
}
