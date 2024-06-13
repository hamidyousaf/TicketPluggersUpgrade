using Microsoft.AspNetCore.Http;
namespace TicketServer.Helpers
{
    public interface IFileHelper
    {
        Task<string> UploadFile(string folder, IFormFile file);
    }
    public class FileHelper : IFileHelper
    {
        public async Task<string> UploadFile(string folder, IFormFile file)
        {
            string folderName = @"wwwroot/"+ folder; //folder is like => @"static//venues"
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }
            var filePath = Path.Combine(folderName, Guid.NewGuid().ToString() + Path.GetExtension(file.FileName.ToLower()));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return filePath;
        }
    }
}