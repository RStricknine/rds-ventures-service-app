namespace RdsVentures.Api.Models;

public class ServiceRequest
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Priority Priority { get; set; }
    public Status Status { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? AssignedTechId { get; set; }
    
    public Property? Property { get; set; }
    public Technician? AssignedTech { get; set; }
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}

public enum Priority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

public enum Status
{
    Open = 0,
    InProgress = 1,
    Complete = 2
}
