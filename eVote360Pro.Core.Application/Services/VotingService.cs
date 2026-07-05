using System.Reflection.Metadata;
using eVote360Pro.Core.Application.DTOs.Email;
using eVote360Pro.Core.Application.DTOs.Voting;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Core.Application.Services
{
    public class VotingService : IVotingService
    {
        private readonly IElectionRepository _electionRepository;
        private readonly ICitizenRepository _citizenRepository;
        private readonly IElectionParticipationRepository _electionParticipationRepository;
        private readonly IElectivePositionRepository _electivePositionRepository;
        private readonly IPartyPositionAssignmentRepository _partyPositionAssignmentRepository;
        private readonly IVoteRepository _voteRepository;
        private readonly IEmailService _emailService;

        public VotingService(IElectionRepository electionRepository, ICitizenRepository citizenRepository,
            IElectionParticipationRepository electionParticipationRepository, IElectivePositionRepository electivePositionRepository,
            IPartyPositionAssignmentRepository partyPositionAssignmentRepository, IVoteRepository voteRepository,
            IEmailService emailService)
        {
            _electionRepository = electionRepository;
            _citizenRepository = citizenRepository;
            _electionParticipationRepository = electionParticipationRepository;
            _electivePositionRepository = electivePositionRepository;
            _partyPositionAssignmentRepository = partyPositionAssignmentRepository;
            _voteRepository = voteRepository;
            _emailService = emailService;
        }

        public async Task<Result<IEnumerable<AvailablePositionDto>>> GetAvailablePositionsForCitizenAsync(string citizenDocument)
        {
            var activeElection = await _electionRepository.GetActiveElectionAsync();

            if (activeElection is null)
            {
                return Result<IEnumerable<AvailablePositionDto>>.Failure("There is no active election at the moment.");
            }

            citizenDocument = new string(citizenDocument.Where(char.IsDigit).ToArray());

            var citizen = await _citizenRepository.GetByDocumentIdAsync(citizenDocument);

            if (citizen is null)
            {
                return Result<IEnumerable<AvailablePositionDto>>.Failure("Citizen not found in the system.");
            }

            if (!citizen.IsActive)
            {
                return Result<IEnumerable<AvailablePositionDto>>.Failure(error: "This citizen is inactive and cannot participate in the voting process.");
            }

            var hasParticipated = await _electionParticipationRepository.HasCitizenFinalizedAsync(citizen.Id, activeElection.Id);

            if (hasParticipated)
            {
                return Result<IEnumerable<AvailablePositionDto>>.Failure("You have already finalized your voting process for this election.");
            }

            var activePositions = await _electivePositionRepository.GetAllActiveAsync();
            var activeAssignments = await _partyPositionAssignmentRepository.GetAllActiveAssignmentsAsync();

            var currentVotes = await _voteRepository.GetAllQuery().Where(v =>
                                    v.CitizenId == citizen.Id &&
                                    v.ElectionId == activeElection.Id)
                                    .ToListAsync();

            var results = new List<AvailablePositionDto>();

            foreach (var position in activePositions)
            {
                var positionAssignments = activeAssignments
                    .Where(a => a.ElectivePositionId == position.Id)
                    .ToList();

                var totalParticipatingParties = positionAssignments
                    .Select(a => a.AssigningPartyId)
                    .Distinct()
                    .Count();

                var totalRealCandidates = positionAssignments
                    .Select(a => a.CandidateId)
                    .Distinct()
                    .Count();

                var hasSelectedCandidate = currentVotes.Any(v => v.ElectivePositionId == position.Id);

                results.Add(new AvailablePositionDto
                {
                    PositionId = position.Id,
                    PositionName = position.Name,
                    TotalParticipatingParties = totalParticipatingParties,
                    TotalRealCandidates = totalRealCandidates,
                    HasSelectedCandidate = hasSelectedCandidate
                });
            }

            return Result<IEnumerable<AvailablePositionDto>>.Success(results);
        }

        public async Task<Result<PositionCandidatesDto>> GetCandidatesForPositionAsync(int positionId, string citizenDocument)
        {
            var activeElection = await _electionRepository.GetActiveElectionAsync();
            
            if (activeElection is null)
            {
                return Result<PositionCandidatesDto>.Failure("There is no active election at the moment.");
            }

            citizenDocument = new string(citizenDocument.Where(char.IsDigit).ToArray());
            var citizen = await _citizenRepository.GetByDocumentIdAsync(citizenDocument);

            if (citizen is null || !citizen.IsActive)
            {
                return Result<PositionCandidatesDto>.Failure("Citizen not found or inactive.");
            }

            if (await _electionParticipationRepository.HasCitizenFinalizedAsync(citizen.Id, activeElection.Id))
            {
                return Result<PositionCandidatesDto>.Failure("You have already finalized your voting process.");
            }

            var position = await _electivePositionRepository.GetByIdAsync(positionId);

            if (position is null || !position.IsActive)
            {
                return Result<PositionCandidatesDto>.Failure("Party Position is not available for voting.");
            }

            var assignments = await _partyPositionAssignmentRepository.GetAllQueryInclude(["Candidate", "AssigningParty"])
                        .Where(a => a.ElectivePositionId == positionId)
                        .ToListAsync();

            var currentVote = await _voteRepository.GetAllQuery()
                        .FirstOrDefaultAsync(v => v.CitizenId == citizen.Id &&
                                            v.ElectionId == activeElection.Id &&
                                            v.ElectivePositionId == positionId);

            var dto = new PositionCandidatesDto
            {
                PositionId = position.Id,
                PositionName = position.Name,

                SelectedAssignmentId = assignments.FirstOrDefault(a =>
                    currentVote != null &&
                    a.CandidateId == currentVote.CandidateId &&
                    a.AssigningPartyId == currentVote.PoliticalPartyId)?.Id,

                Candidates = assignments.Select(a => new CandidateOptionDto
                {
                    AssignmentId = a.Id,
                    CandidatePhoto = a.Candidate!.Photo, 
                    CandidateName = $"{a.Candidate.Name} {a.Candidate.LastName}",
                    PoliticalPartyName = a.AssigningParty!.Name, 
                    PoliticalPartyLogo = a.AssigningParty.Logo 
                }).ToList()
            };

            return Result<PositionCandidatesDto>.Success(dto);
        }

        public async Task<Result> VoteAsync(int positionId, int selectedAssignmentId, string citizenDocument)
        {
            var activeElection = await _electionRepository.GetActiveElectionAsync();

            if (activeElection is null)
            {
                return Result.Failure("There is no active election at the moment.");
            }

            citizenDocument = new string(citizenDocument.Where(char.IsDigit).ToArray());
            var citizen = await _citizenRepository.GetByDocumentIdAsync(citizenDocument);

            if (citizen is null || !citizen.IsActive)
            {
                return Result.Failure("Citizen not found or inactive.");
            }

            if (await _electionParticipationRepository.HasCitizenFinalizedAsync(citizen.Id, activeElection.Id))
            {
                return Result.Failure("You have already finalized your voting process.");
            }

            var position = await _electivePositionRepository.GetByIdAsync(positionId);

            if (position is null || !position.IsActive)
            {
                return Result.Failure("Party Position is not available for voting.");
            }

            int? finalCandidateId = null;
            int? finalPartyId = null;

            if (selectedAssignmentId != -1) // if 0, was picked ninguno TODO: selectedAssignmentId != -1
            {
                var assignment = await _partyPositionAssignmentRepository.GetByIdAsync(selectedAssignmentId);

                if (assignment is null || assignment.ElectivePositionId != positionId)
                {
                    return Result.Failure("The selected candidate assignment is invalid.");
                }

                finalCandidateId = assignment.CandidateId;
                finalPartyId = assignment.AssigningPartyId;
            }

            var currentVote = await _voteRepository.GetAllQuery()
                .FirstOrDefaultAsync(v => v.CitizenId == citizen.Id &&
                                          v.ElectionId == activeElection.Id &&
                                          v.ElectivePositionId == positionId);

            if (currentVote is not null) // If exists it is an update, if not, a new
            {
                currentVote.CandidateId = finalCandidateId;
                currentVote.PoliticalPartyId = finalPartyId;

                await _voteRepository.UpdateAsync(currentVote);
            }
            else
            {
                var newVote = new Vote
                {
                    Id = 0,
                    ElectionId = activeElection.Id,
                    CitizenId = citizen.Id,
                    ElectivePositionId = positionId,
                    CandidateId = finalCandidateId,
                    PoliticalPartyId = finalPartyId
                };

                await _voteRepository.AddAsync(newVote);
            }

            return Result.Success();
        }

        public async Task<Result> FinalizeVotingAsync(string citizenDocument)
        {
            var activeElection = await _electionRepository.GetActiveElectionAsync();

            if (activeElection is null)
            {
                return Result.Failure("There is no active election at the moment.");
            }

            citizenDocument = new string(citizenDocument.Where(char.IsDigit).ToArray());
            var citizen = await _citizenRepository.GetByDocumentIdAsync(citizenDocument);

            if (citizen is null || !citizen.IsActive)
            {
                return Result.Failure("Citizen not found or inactive.");
            }

            if (await _electionParticipationRepository.HasCitizenFinalizedAsync(citizen.Id, activeElection.Id))
            {
                return Result.Failure("You have already finalized your voting process.");
            }

            var activePositions = await _electivePositionRepository.GetAllActiveAsync();

            var currentVotes = await _voteRepository.GetAllQueryInclude(["ElectivePosition", "Candidate", "PoliticalParty"])
                                .Where(v => v.CitizenId == citizen.Id && v.ElectionId == activeElection.Id)
                                .ToListAsync();

            var votedPositionIds = currentVotes.Select(v => v.ElectivePositionId).ToList();
            var missingPositions = activePositions.Where(p => !votedPositionIds.Contains(p.Id)).ToList();

            if (missingPositions.Any())
            {
                var missingNames = string.Join(", ", missingPositions.Select(p => p.Name));
                return Result.Failure($"You must complete your selection for the following elective positions: \n{missingNames}.");
            }

            var participation = new ElectionParticipation
            {
                Id = 0,
                CitizenId = citizen.Id,
                ElectionId = activeElection.Id,
                FinalizedDate = DateTime.Now
            };

            await _electionParticipationRepository.AddAsync(participation);

            await SendVotingSummaryAsync(citizen, activeElection, currentVotes);

            return Result.Success();
        }

        #region Private Methods
        private async Task SendVotingSummaryAsync(Citizen citizen, Election election, List<Vote> votes)
        {
            var subject = "Summary of your electoral participation";

            var body = $@"
                <p>
                    Hello <strong>{citizen.Name} {citizen.LastName}</strong>,
                    Your voting process has been successfully completed.
                </p>
                <h4>Selection summary:</h4>
                <p>
                    <b>Election:</b> {election.Name}<br/>
                    <b>Election Date:</b> {election.Date:dd/MM/yyyy}
                </p>
                <hr/>
                <ul style='list-style-type: none; padding: 0;'>";

            foreach (var vote in votes)
            {
                var positionName = vote.ElectivePosition!.Name;

                if (vote.CandidateId.HasValue && vote.PoliticalPartyId.HasValue)
                {
                    var candidateName = $"{vote.Candidate!.Name} {vote.Candidate.LastName}";
                    var partyName = vote.PoliticalParty!.Name;

                    body += $@"
                        <li style='margin-bottom: 15px;'>
                            <b>Elective Position:</b> {positionName}<br/>
                            <b>Choice:</b> {candidateName}<br/>
                            <b>Political Party:</b> {partyName}
                        </li>";
                }
                else
                {
                    // None
                    body += $@"
                        <li style='margin-bottom: 15px;'>
                            <b>Elective Position:</b> {positionName}<br/>
                            <b>Choice:</b> None
                        </li>";
                }
            }

            body += $@"
                </ul>
                <hr/>
                <p>Thank you for exercising your right to vote.</p>";

            var emailDto = new EmailRequestDto
            {
                To = citizen.Email,
                Subject = subject,
                BodyHtml = body
            };

            var result = await _emailService.SendAsync(emailDto);
        }
        #endregion
    }
}
