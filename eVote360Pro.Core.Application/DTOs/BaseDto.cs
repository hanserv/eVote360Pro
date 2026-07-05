namespace eVote360Pro.Core.Application.DTOs
{
    public class BaseDto<TKey>
    {
        public required TKey Id { get; set; }
    }
}
