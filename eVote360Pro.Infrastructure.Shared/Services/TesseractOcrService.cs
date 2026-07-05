using System.Text.RegularExpressions;
using eVote360Pro.Core.Application.Interfaces;
using Tesseract;

namespace eVote360Pro.Infrastructure.Shared.Services
{
    public class TesseractOcrService : IOcrService
    {
        public async Task<string> ExtractDocumentIdFromImageAsync(Stream imageStream)
        {
            if (imageStream == null || imageStream.Length == 0)
            {
                return string.Empty;
            }

            using var memoryStream = new MemoryStream();
            await imageStream.CopyToAsync(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();

            string tessDataPath = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");

            try
            {
                using var engine = new TesseractEngine(tessDataPath, "spa", EngineMode.Default);
                using var img = Pix.LoadFromMemory(imageBytes);
                using var page = engine.Process(img);

                string allExtractedText = page.GetText();

                string pattern = @"\d{3}[- ]?\d{7}[- ]?\d{1}";

                var match = Regex.Match(allExtractedText, pattern);

                if (!match.Success)
                {
                    return "";
                }

                string cleanDocumentId = match.Value.Replace("-", "").Replace(" ", ""); // string clean solo numbers
                return cleanDocumentId;
            }
            catch (Exception)
            {
                return ""; 
            }
        }
    }
}
