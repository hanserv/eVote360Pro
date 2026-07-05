namespace eVote360Pro.Core.Application.Interfaces
{
    public interface IOcrService
    {
        Task<string> ExtractDocumentIdFromImageAsync(Stream imageStream);
    }
}
