namespace eVote360Pro.Core.Application.DTOs.ElectivePosition
{
    public class ElectivePositionDto : BaseDto<int>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required bool IsActive { get; set; }
    }
}
