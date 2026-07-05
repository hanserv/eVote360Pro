using System.ComponentModel.DataAnnotations;

namespace eVote360Pro.Core.Application.ViewModels.User
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Field {0} is required.")]
        [DataType(DataType.Text)]
        public required string UserName { get; set; }
        [Required(ErrorMessage = "Field {0} is required.")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
