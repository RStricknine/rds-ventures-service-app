namespace RdsVentures.Api.Models;

public class Attachment
{
    public int Id { get; set; }
    public int ServiceRequestId { get; set; }
    public int TechId { get; set; }
    public required string BlobUrl { get; set; }
    public required string ContentType { get; set; }
    public DateTime UploadedUtc { get; set; }
    public string? Caption { get; set; }
    
    public ServiceRequest? ServiceRequest { get; set; }
    public Technician? Technician { get; set; }
}
