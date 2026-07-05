using System.ComponentModel.DataAnnotations;

namespace eVote360Pro.Core.Application.ViewModels.Election
{
    public class CreateElectionViewModel
    {
        [Required(ErrorMessage = "Field {0} is required.")]
        [DataType(DataType.Text)]
        public required string Name { get; set; }
        [Required(ErrorMessage = "Field {0} is required.")]
        [DataType(DataType.DateTime)]
        public required DateTime Date { get; set; }
    }
}
