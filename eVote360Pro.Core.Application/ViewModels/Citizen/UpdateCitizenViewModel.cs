using System.ComponentModel.DataAnnotations;

namespace eVote360Pro.Core.Application.ViewModels.Citizen
{
    public class UpdateCitizenViewModel : BaseViewModel<int>
    {
        [Required(ErrorMessage = "Field {0} is required.")]
        [DataType(DataType.Text)]
        public required string Name { get; set; }
        [Required(ErrorMessage = "Field {0} is required.")]
        [DataType(DataType.Text)]
        public required string LastName { get; set; }
        [Required(ErrorMessage = "Field {0} is required.")]
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Field {0} is required.")]
        [DataType(DataType.Text)]
        public required string DocumentId { get; set; }
    }
}
