using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace eVote360Pro.Core.Application.ViewModels.Candidate
{
    public class UpdateCandidateViewModel : BaseViewModel<int>
    {
        [Required(ErrorMessage = "Field {0} is required.")]
        [DataType(DataType.Text)]
        public required string Name { get; set; }
        [Required(ErrorMessage = "Field {0} is required.")]
        [DataType(DataType.Text)]
        public required string LastName { get; set; }
        [DataType(DataType.Upload)]
        public IFormFile? PhotoFile { get; set; }
    }
}
