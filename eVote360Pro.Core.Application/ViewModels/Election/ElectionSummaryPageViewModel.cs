using System.ComponentModel.DataAnnotations;

namespace eVote360Pro.Core.Application.ViewModels.Election
{
    public class ElectionSummaryPageViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "You must select a valid year.")]
        public int SelectedYear { get; set; }
        public IEnumerable<ElectionSummaryViewModel>? Summaries { get; set; }
    }
}
