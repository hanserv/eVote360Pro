using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace eVote360Pro.Core.Application.ViewModels.Candidate
{
    public class CreateCandidateViewModel
    {
        [Required(ErrorMessage = "Field {0} is required.")]
        [DataType(DataType.Text)]
        public required string Name { get; set; }
        [Required(ErrorMessage = "Field {0} is required.")]
        [DataType(DataType.Text)]
        public required string LastName { get; set; }
        [Required(ErrorMessage = "Field {0} is required.")]
        [DataType(DataType.Upload)]
        public required IFormFile PhotoFile { get; set; }
    }
}
