using eVote360Pro.Core.Domain.Common.Enums;

namespace eVote360Pro.Core.Application.DTOs.Election
{
    public class ElectionDto : BaseDto<int>
    {
        public required string Name { get; set; }
        public required DateTime Date { get; set; }
        public required ElectionStatus Status { get; set; }
        public required int TotalParties { get; set; }
        public required int TotalPositions { get; set; }
        public required int TotalVoters { get; set; }
    }
}
