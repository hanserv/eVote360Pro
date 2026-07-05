using System.ComponentModel.DataAnnotations;

namespace eVote360Pro.Core.Application.ViewModels.Voting
{
    public class SaveVoteViewModel
    {
        public int PositionId { get; set; }
        [Required(ErrorMessage = "You must select a valid option")]
        public required int SelectedAssignmentId { get; set; }
    }
}
