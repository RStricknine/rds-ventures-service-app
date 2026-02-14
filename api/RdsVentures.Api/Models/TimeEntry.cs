namespace RdsVentures.Api.Models;

public class TimeEntry
{
    public int Id { get; set; }
    public int TechId { get; set; }
    public int ServiceRequestId { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime? EndUtc { get; set; }
    public int? DurationMinutes { get; set; }
    public DateTime WeekStartMondayUtc { get; set; }
    
    public Technician? Technician { get; set; }
    public ServiceRequest? ServiceRequest { get; set; }
}
