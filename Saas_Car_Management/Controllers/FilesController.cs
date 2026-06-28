using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saas_Car_Management.Core.Entities;
using Saas_Car_Management.Core.Interfaces;
using Saas_Car_Management.Infrastructure.Persistence;

namespace Saas_Car_Management.Controllers
{
    [Authorize]
    public class FilesController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public FilesController(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromQuery] string folder, [FromQuery] string? entityName, [FromQuery] int? entityId)
        {
            if (file == null || file.Length == 0) return BadRequest("No file provided.");

            var tenantId = GetTenantId();
            if (string.IsNullOrEmpty(folder)) folder = "general";

            // Upload using service
            var relativePath = await _fileService.SaveFileAsync(file, folder);

            // Record in DB
            var fileRecord = new Core.Entities.File
            {
                TenantId = tenantId,
                FilePath = relativePath,
                FileName = file.FileName,
                FileType = file.ContentType
            };

            await _context.Files.AddAsync(fileRecord);
            await _context.SaveChangesAsync();

            // Link to entity if details provided
            if (!string.IsNullOrEmpty(entityName) && entityId.HasValue && entityId.Value != 0)
            {
                var entityFile = new EntityFile
                {
                    TenantId = tenantId,
                    EntityName = entityName,
                    EntityId = entityId.Value,
                    FileId = fileRecord.Id
                };

                await _context.EntityFiles.AddAsync(entityFile);
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                FileId = fileRecord.Id,
                FilePath = relativePath,
                FileName = file.FileName
            });
        }

        [HttpGet("entity/{entityName}/{entityId}")]
        public async Task<IActionResult> GetEntityFiles(string entityName, int entityId)
        {
            var tenantId = GetTenantId();
            var files = await _context.EntityFiles
                .Include(ef => ef.File)
                .Where(ef => ef.TenantId == tenantId && ef.EntityName.ToLower() == entityName.ToLower() && ef.EntityId == entityId && !ef.IsDeleted)
                .Select(ef => new
                {
                    ef.Id,
                    ef.FileId,
                    ef.File.FilePath,
                    ef.File.FileName,
                    ef.File.FileType
                })
                .ToListAsync();

            return Ok(files);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var tenantId = GetTenantId();
            var file = await _context.Files.FindAsync(id);
            if (file == null || file.TenantId != tenantId || file.IsDeleted) return NotFound();

            // Delete physical file
            _fileService.DeleteFile(file.FilePath);

            // Remove entity links
            var entityLinks = await _context.EntityFiles.Where(ef => ef.FileId == id).ToListAsync();
            foreach (var link in entityLinks)
            {
                link.IsDeleted = true;
                link.DeletedAt = DateTime.UtcNow;
                _context.EntityFiles.Update(link);
            }

            // Soft delete record
            file.IsDeleted = true;
            file.DeletedAt = DateTime.UtcNow;
            _context.Files.Update(file);

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
