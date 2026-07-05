using System.ComponentModel.DataAnnotations;

namespace eVote360Pro.Core.Application.ViewModels.PoliticalLeaderAssignment
{
    public class CreatePoliticalLeaderAssignmentViewModel
    {
        [Required(ErrorMessage = "Field {0} is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a valid user.")]
        public required int UserId { get; set; }
        [Required(ErrorMessage = "Field {0} is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a valid political party.")]
        public required int PoliticalPartyId { get; set; }
    }
}
