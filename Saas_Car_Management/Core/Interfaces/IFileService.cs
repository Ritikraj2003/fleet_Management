using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Saas_Car_Management.Core.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder);
        void DeleteFile(string filePath);
    }
}
