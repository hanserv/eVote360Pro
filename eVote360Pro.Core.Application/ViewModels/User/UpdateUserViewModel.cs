using System.ComponentModel.DataAnnotations;
using eVote360Pro.Core.Domain.Common.Enums;

namespace eVote360Pro.Core.Application.ViewModels.User
{
    public class UpdateUserViewModel : BaseViewModel<int>
    {
        [Required(ErrorMessage = "Field {0} is required.")]
        [DataType(DataType.Text)]
        public required string Name { get; set; }
        [Required(ErrorMessage = "Field {0} is required.")]
        [DataType(DataType.Text)]
        public required string LastName { get; set; }
        [Required(ErrorMessage = "Field {0} is required.")]
        [DataType(DataType.Text)]
        public required string Username { get; set; }
        [Required(ErrorMessage = "Field {0} is required.")]
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Compare(nameof(Password), ErrorMessage = "The password and password confirmation do not match.")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "You must select a valid role")]
        public required UserRole Role { get; set; }
    }
}
