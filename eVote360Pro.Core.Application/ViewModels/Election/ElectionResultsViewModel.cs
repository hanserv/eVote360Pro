namespace eVote360Pro.Core.Application.ViewModels.Election
{
    public class ElectionResultsViewModel
    {
        public required int ElectionId { get; set; }
        public required string ElectionName { get; set; } = null!;
        public List<PositionResultViewModel> PositionResults { get; set; } = [];
    }
}
