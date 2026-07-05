namespace eVote360Pro.Core.Application.DTOs.Citizen
{
    public class CitizenDto : BaseDto<int>
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string DocumentId { get; set; }
        public required bool IsActive { get; set; }
    }   
}
