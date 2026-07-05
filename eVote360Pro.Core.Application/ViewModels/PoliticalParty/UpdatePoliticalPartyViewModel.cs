using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace eVote360Pro.Core.Application.ViewModels.PoliticalParty
{
    public class UpdatePoliticalPartyViewModel : BaseViewModel<int>
    {
        [Required(ErrorMessage = "Field {0} is required.")]
        [DataType(DataType.Text)]
        public required string Name { get; set; }
        [DataType(DataType.Text)]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Field {0} is required.")]
        [DataType(DataType.Text)]
        public required string Acronym { get; set; }
        [DataType(DataType.Upload)]
        public IFormFile? LogoFile { get; set; }
    }
}
