using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RdsVentures.Api.Data;
using RdsVentures.Api.DTOs;
using RdsVentures.Api.Models;
using RdsVentures.Api.Services;

namespace RdsVentures.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttachmentsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IBlobStorageService _blobStorageService;

    public AttachmentsController(AppDbContext context, IBlobStorageService blobStorageService)
    {
        _context = context;
        _blobStorageService = blobStorageService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AttachmentDto>>> GetAttachments()
    {
        var attachments = await _context.Attachments
            .Include(a => a.Technician)
            .Select(a => new AttachmentDto
            {
                Id = a.Id,
                ServiceRequestId = a.ServiceRequestId,
                TechId = a.TechId,
                BlobUrl = a.BlobUrl,
                ContentType = a.ContentType,
                UploadedUtc = a.UploadedUtc,
                Caption = a.Caption,
                TechnicianName = a.Technician != null ? a.Technician.Name : null
            })
            .ToListAsync();

        return Ok(attachments);
    }

    [HttpGet("service-request/{serviceRequestId}")]
    public async Task<ActionResult<IEnumerable<AttachmentDto>>> GetAttachmentsByServiceRequest(int serviceRequestId)
    {
        var attachments = await _context.Attachments
            .Include(a => a.Technician)
            .Where(a => a.ServiceRequestId == serviceRequestId)
            .Select(a => new AttachmentDto
            {
                Id = a.Id,
                ServiceRequestId = a.ServiceRequestId,
                TechId = a.TechId,
                BlobUrl = a.BlobUrl,
                ContentType = a.ContentType,
                UploadedUtc = a.UploadedUtc,
                Caption = a.Caption,
                TechnicianName = a.Technician != null ? a.Technician.Name : null
            })
            .ToListAsync();

        return Ok(attachments);
    }

    [HttpPost("sas-token")]
    public async Task<ActionResult<SasTokenResponse>> GetSasToken(CreateAttachmentDto dto)
    {
        var (sasUrl, blobUrl) = await _blobStorageService.GenerateSasTokenAsync(dto.FileName, dto.ContentType);

        // Save attachment metadata
        var attachment = new Attachment
        {
            ServiceRequestId = dto.ServiceRequestId,
            TechId = dto.TechId,
            BlobUrl = blobUrl,
            ContentType = dto.ContentType,
            UploadedUtc = DateTime.UtcNow,
            Caption = dto.Caption
        };

        _context.Attachments.Add(attachment);
        await _context.SaveChangesAsync();

        return Ok(new SasTokenResponse
        {
            SasUrl = sasUrl,
            BlobUrl = blobUrl,
            ExpiresOn = DateTime.UtcNow.AddHours(1)
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAttachment(int id)
    {
        var attachment = await _context.Attachments.FindAsync(id);
        if (attachment == null)
            return NotFound();

        _context.Attachments.Remove(attachment);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
