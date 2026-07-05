namespace eVote360Pro.Core.Application.ViewModels.User
{
    public class ChangeStatusUserViewModel
    {
        public required int Id { get; set; }
        public string? Name { get; set; }
        public bool NewStatus { get; set; }
    }
}
