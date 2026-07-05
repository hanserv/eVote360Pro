using System.ComponentModel.DataAnnotations;

namespace eVote360Pro.Core.Application.ViewModels.ElectivePosition
{
    public class UpdateElectivePositionViewModel : BaseViewModel<int>
    {
        [Required(ErrorMessage = "Field {0} is required.")]
        [DataType(DataType.Text)]
        public required string Name { get; set; }
        [Required(ErrorMessage = "Field {0} is required.")]
        [DataType(DataType.Text)]
        public required string Description { get; set; }
    }
}
