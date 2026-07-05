namespace eVote360Pro.Core.Application.DTOs.ElectivePosition
{
    public class UpdateElectivePositionDto : BaseDto<int>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
    }
}
