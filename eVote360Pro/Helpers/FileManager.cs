using eVote360Pro.Core.Application;

namespace eVote360Pro.Helpers
{
    public static class FileManager
    {
        public static string? UploadAsync(IFormFile? file, int id, string folderName, bool isEditMode = false, string? imagePath = "")
        {
            if (isEditMode && file is null)
            {
                return imagePath;
            }

            if (file is null)
            {
                return string.Empty;
            }

            string basePath = $"Images/{folderName}/{id}";
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/{basePath}");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Guid guid = Guid.NewGuid();
            FileInfo fileInfo = new(file.FileName);
            string fileName = guid + fileInfo.Extension;

            string fullFilePath = Path.Combine(path, fileName);

            using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            if (isEditMode && !string.IsNullOrWhiteSpace(imagePath))
            {
                string[] oldImagePart = imagePath.Split("/");
                string oldFileName = oldImagePart[^1];
                string completeOldPath = Path.Combine(path, oldFileName);

                if (File.Exists(completeOldPath))
                {
                    File.Delete(completeOldPath);
                }
            }

            return $"{basePath}/{fileName}";
        }

        public static Result ValidateFile(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return Result.Failure("Invalid file extension. Only image files .jpg, .jpeg and .png are allowed.");
            }

            const long maxFileSizeInBytes = 10 * 1024 * 1024;

            if (file.Length > maxFileSizeInBytes)
            {
                return Result.Failure("The file size exceeds the 10MB limit.");
            }

            return Result.Success();
        }
    }
}
