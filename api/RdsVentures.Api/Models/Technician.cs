namespace RdsVentures.Api.Models;

public class Technician
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public decimal HourlyRate { get; set; }
    
    public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
