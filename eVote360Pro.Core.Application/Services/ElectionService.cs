using eVote360Pro.Core.Application.DTOs.Election;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Core.Domain.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Core.Application.Services
{
    public class ElectionService : IElectionService
    {
        private readonly IElectionRepository _electionRepository;
        private readonly IMapper _mapper;
        private readonly IElectivePositionRepository _electivePositionRepository;
        private readonly IPoliticalPartyRepository _politicalPartyRepository;
        private readonly IPartyPositionAssignmentRepository _partyPositionAssignmentRepository;
        private readonly IElectionParticipationRepository _electionParticipationRepository;
        private readonly IVoteRepository _voteRepository;
        public ElectionService(IElectionRepository electionRepository, IMapper mapper,
            IElectivePositionRepository electivePositionRepository, IPoliticalPartyRepository politicalPartyRepository,
            IPartyPositionAssignmentRepository partyPositionAssignmentRepository, IElectionParticipationRepository electionParticipationRepository,
            IVoteRepository voteRepository)
        {
            _electionRepository = electionRepository;
            _mapper = mapper;
            _electivePositionRepository = electivePositionRepository;
            _politicalPartyRepository = politicalPartyRepository;
            _partyPositionAssignmentRepository = partyPositionAssignmentRepository;
            _electionParticipationRepository = electionParticipationRepository;
            _voteRepository = voteRepository;
        }

        public async Task<bool> HasActiveElectionAsync()
        {
            return await _electionRepository.HasActiveElectionAsync();
        }

        public async Task<Result<IEnumerable<ElectionDto>>> GetAllElectionsAsync()
        {
            var elections = await _electionRepository.GetElectionsOrderedAsync();
            var dtos = _mapper.Map<List<ElectionDto>>(elections);

            foreach (var election in dtos)
            {
                var participations = await _electionParticipationRepository.GetByElectionIdAsync(election.Id);
                election.TotalVoters = participations.Count();

                if (election.Status == ElectionStatus.Pending)
                {
                    election.TotalParties = (await _politicalPartyRepository.GetAllActiveAsync()).Count();
                    election.TotalPositions = (await _electivePositionRepository.GetAllActiveAsync()).Count();
                }
            }

            return Result<IEnumerable<ElectionDto>>.Success(dtos);
        }

        public async Task<Result<ElectionDto>> GetByIdAsync(int id)
        {
            var entity = await _electionRepository.GetByIdAsync(id);

            if(entity is null)
            {
                return Result<ElectionDto>.Failure(error: "The election doesnt exist");
            }

            return Result<ElectionDto>.Success(_mapper.Map<ElectionDto>(entity));
        }

        public async Task<Result<ElectionDto>> AddAsync(CreateElectionDto createDto)
        {
            if(await _electionRepository.HasActiveElectionAsync())
            {
                return Result<ElectionDto>.Failure(error: "You cannot create a new election while an active election exists.");
            }

            var activePositions = await _electivePositionRepository.GetAllActiveAsync();

            if(!activePositions.Any())
            {
                return Result<ElectionDto>.Failure(error: "There are no active elective positions to hold an election.");
            }

            var activeParties = await _politicalPartyRepository.GetAllActiveAsync();

            if(activeParties.Count() < 2)
            {
                return Result<ElectionDto>.Failure(error: "There are not enough political parties to hold an election.");
            }

            var allActiveAssignments = await _partyPositionAssignmentRepository.GetAllActiveAssignmentsAsync();
            var validationErrors = new List<string>();

            foreach (var party in activeParties)
            {
                var assignedPositionIds = allActiveAssignments
                    .Where(a => a.AssigningPartyId == party.Id)
                    .Select(a => a.ElectivePositionId)
                    .ToList();

                var missingPositions = activePositions
                    .Where(p => !assignedPositionIds.Contains(p.Id))
                    .ToList();

                if (missingPositions.Any())
                {
                    var missingPositionsNames = string.Join(", ", missingPositions.Select(p => p.Name));

                    validationErrors.Add($"The political party {party.Name} ({party.Acronym}) doesnt have active candidates assigned for the following elective positions: {missingPositionsNames}.");
                }
            }

            if (validationErrors.Any())
            {
                return Result<ElectionDto>.Failure(error: string.Join(Environment.NewLine, validationErrors));
            }

            var entity = _mapper.Map<Election>(createDto);
            entity.Status = ElectionStatus.Pending;

            var result = await _electionRepository.AddAsync(entity);
            return Result<ElectionDto>.Success(_mapper.Map<ElectionDto>(result));
        }

        public async Task<Result<IEnumerable<int>>> GetAvailableElectionYearsAsync()
        {
            var years = await _electionRepository.GetAllQuery()
                        .Select(e => e.Date.Year)
                        .Distinct()
                        .OrderByDescending(year => year)
                        .ToListAsync();

            return Result<IEnumerable<int>>.Success(years);
        }

        public async Task<Result<IEnumerable<ElectionSummaryDto>>> GetElectoralSummaryByYearAsync(int year)
        {
            if (year <= 0)
            {
                return Result<IEnumerable<ElectionSummaryDto>>.Failure("The election year provided is not valid.");
            }

            var elections = await _electionRepository.GetAllQuery()
                        .Where(e => e.Date.Year == year)
                        .OrderByDescending(e => e.Date)
                        .ToListAsync();

            if (!elections.Any())
            {
                return Result<IEnumerable<ElectionSummaryDto>>.Failure("There are no registered elections for the selected year.");
            }

            var summaryList = new List<ElectionSummaryDto>();

            foreach (var election in elections)
            {
                var totalCitizensVoted = await _electionParticipationRepository.GetAllQuery()
                            .Where(ep => ep.ElectionId == election.Id)
                            .CountAsync();

                summaryList.Add(new ElectionSummaryDto
                {
                    ElectionName = election.Name,
                    RealizationDate = election.Date,
                    TotalParticipatingParties = election.TotalParties ?? 0,
                    TotalParticipatingCandidates = election.TotalCandidates ?? 0,
                    TotalCitizensVoted = totalCitizensVoted
                });
            }

            return Result<IEnumerable<ElectionSummaryDto>>.Success(summaryList);
        }

        public async Task<Result> ActivateAsync(int id)
        {
            var election = await _electionRepository.GetByIdAsync(id);

            if (election is null)
            {
                return Result.Failure(error: "The election does not exist.");
            }

            if (election.Status != ElectionStatus.Pending)
            {
                return Result.Failure(error: "Only pending elections can be activated.");
            }

            if (await _electionRepository.HasActiveElectionAsync())
            {
                return Result.Failure(error: "You cannot activate this election because there is already an active election.");
            }

            var activePositions = await _electivePositionRepository.GetAllActiveAsync();
            if (!activePositions.Any())
            {
                return Result.Failure(error: "There are no active elective positions to activate this election.");
            }

            var activeParties = await _politicalPartyRepository.GetAllActiveAsync();
            if (activeParties.Count() < 2)
            {
                return Result.Failure(error: "There are not enough political parties to activate this election.");
            }

            var allActiveAssignments = await _partyPositionAssignmentRepository.GetAllActiveAssignmentsAsync();
            var validationErrors = new List<string>();

            foreach (var party in activeParties)
            {
                var assignedPositionIds = allActiveAssignments
                    .Where(a => a.AssigningPartyId == party.Id)
                    .Select(a => a.ElectivePositionId)
                    .ToList();

                var missingPositions = activePositions
                    .Where(p => !assignedPositionIds.Contains(p.Id))
                    .ToList();

                if (missingPositions.Any())
                {
                    var missingPositionsNames = string.Join(", ", missingPositions.Select(p => p.Name));
                    validationErrors.Add($"The political party {party.Name} ({party.Acronym}) doesnt have active candidates assigned for the following elective positions: {missingPositionsNames}.");
                }
            }

            if (validationErrors.Any())
            {
                validationErrors.Insert(0, "You cannot activate this election because the current electoral configuration is not complete.");
                return Result.Failure(error: string.Join(Environment.NewLine, validationErrors));
            }

            election.TotalParties = allActiveAssignments
                    .Select(a => a.AssigningPartyId)
                    .Distinct()
                    .Count();

            election.TotalPositions = allActiveAssignments
                    .Select(a => a.ElectivePositionId)
                    .Distinct()
                    .Count();

            election.TotalCandidates = allActiveAssignments
                    .Select(a => a.CandidateId)
                    .Distinct()
                    .Count();

            election.Status = ElectionStatus.Active;
            await _electionRepository.UpdateAsync(election);

            return Result.Success();
        }

        public async Task<Result> FinalizeAsync(int id)
        {
            var election = await _electionRepository.GetByIdAsync(id);

            if (election is null)
            {
                return Result.Failure(error: "The election does not exist.");
            }

            if (election.Status == ElectionStatus.Finished)
            {
                return Result.Failure(error: "This election is already finished.");
            }

            if (election.Status != ElectionStatus.Active)
            {
                return Result.Failure(error: "Only active elections can be finished.");
            }

            election.Status = ElectionStatus.Finished;
            await _electionRepository.UpdateAsync(election);

            return Result.Success();
        }

        public async Task<Result<ElectionResultsDto>> GetElectionResultsAsync(int electionId)
        {
            var election = await _electionRepository.GetByIdAsync(electionId);

            if (election is null)
            {
                return Result<ElectionResultsDto>.Failure(error: "The election does not exist.");
            }

            if (election.Status != ElectionStatus.Finished)
            {
                return Result<ElectionResultsDto>.Failure(error: "Results can only be viewed for finalized elections.");
            }

            var finalizedCitizenIds = await _electionParticipationRepository.GetAllQuery()
                            .Where(ep => ep.ElectionId == electionId)
                            .Select(ep => ep.CitizenId)
                            .ToListAsync();

            var allVotes = await _voteRepository.GetAllQuery()
                            .Where(v => v.ElectionId == electionId && finalizedCitizenIds.Contains(v.CitizenId))
                            .ToListAsync();

            var activePositions = await _electivePositionRepository.GetAllActiveAsync();
            var assignments = await _partyPositionAssignmentRepository.GetAllQueryInclude(["Candidate", "AssigningParty"])
                            .ToListAsync();

            var resultsDto = new ElectionResultsDto
            {
                ElectionId = election.Id,
                ElectionName = election.Name,
                PositionResults = []
            };


            foreach (var position in activePositions)
            {
                var positionAssignments = assignments.Where(a => a.ElectivePositionId == position.Id).ToList();

                if (!positionAssignments.Any())
                {
                    continue;
                }

                var positionVotes = allVotes.Where(v => v.ElectivePositionId == position.Id).ToList();
                int totalVotesForPosition = positionVotes.Count;

                var candidateResults = new List<CandidateResultDto>();

                foreach (var assignment in positionAssignments)
                {
                    int voteCount = positionVotes.Count(v => v.CandidateId == assignment.CandidateId &&
                                                             v.PoliticalPartyId == assignment.AssigningPartyId);

                    double percentage = totalVotesForPosition > 0
                                ? ((double)voteCount / totalVotesForPosition) * 100 : 0;

                    candidateResults.Add(new CandidateResultDto
                    {
                        CandidateName = $"{assignment.Candidate!.Name} {assignment.Candidate.LastName}",
                        PartyName = assignment.AssigningParty!.Name,
                        VoteCount = voteCount,
                        Percentage = Math.Round(percentage, 2),
                        IsWinner = false,
                        IsNone = false
                    });
                }

                int noneVoteCount = positionVotes.Count(v => v.CandidateId == null);
                double nonePercentage = totalVotesForPosition > 0
                    ? ((double)noneVoteCount / totalVotesForPosition) * 100  : 0;

                candidateResults.Add(new CandidateResultDto
                {
                    PartyName = "Not applicable",
                    CandidateName = "None",
                    VoteCount = noneVoteCount,
                    Percentage = Math.Round(nonePercentage, 2),
                    IsWinner = false,
                    IsNone = true
                });

                candidateResults = candidateResults.OrderByDescending(c => c.VoteCount).ToList();

                bool hasTie = false;

                if (totalVotesForPosition > 0)
                {
                    int maxVotes = candidateResults.Max(c => c.VoteCount);
                    var topOptions = candidateResults.Where(c => c.VoteCount == maxVotes).ToList();

                    if (topOptions.Count > 1)
                    {
                        hasTie = true; 
                    }
                    else
                    {
                        var topOption = topOptions.First();

                        if (!topOption.IsNone)
                        {
                            topOption.IsWinner = true;
                        }
                    }
                }

                resultsDto.PositionResults.Add(new PositionResultDto
                {
                    PositionId = position.Id,
                    PositionName = position.Name,
                    TotalVotes = totalVotesForPosition,
                    HasTie = hasTie,
                    Candidates = candidateResults
                });
            }

            return Result<ElectionResultsDto>.Success(resultsDto);
        }
    }
}
