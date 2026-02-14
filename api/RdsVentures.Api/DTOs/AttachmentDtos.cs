namespace RdsVentures.Api.DTOs;

public class AttachmentDto
{
    public int Id { get; set; }
    public int ServiceRequestId { get; set; }
    public int TechId { get; set; }
    public string BlobUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedUtc { get; set; }
    public string? Caption { get; set; }
    public string? TechnicianName { get; set; }
}

public class CreateAttachmentDto
{
    public int ServiceRequestId { get; set; }
    public int TechId { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public string? Caption { get; set; }
}

public class SasTokenResponse
{
    public string SasUrl { get; set; } = string.Empty;
    public string BlobUrl { get; set; } = string.Empty;
    public DateTime ExpiresOn { get; set; }
}
