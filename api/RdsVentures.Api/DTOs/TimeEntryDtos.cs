namespace RdsVentures.Api.DTOs;

public class TimeEntryDto
{
    public int Id { get; set; }
    public int TechId { get; set; }
    public int ServiceRequestId { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime? EndUtc { get; set; }
    public int? DurationMinutes { get; set; }
    public DateTime WeekStartMondayUtc { get; set; }
    public string? TechnicianName { get; set; }
    public string? ServiceRequestTitle { get; set; }
}

public class StartTimeEntryDto
{
    public int ServiceRequestId { get; set; }
    public int TechId { get; set; }
}

public class StopTimeEntryDto
{
    public int TimeEntryId { get; set; }
}

public class WeeklyTimeDto
{
    public int TechId { get; set; }
    public string TechnicianName { get; set; } = string.Empty;
    public DateTime WeekStart { get; set; }
    public int TotalMinutes { get; set; }
    public decimal TotalHours => TotalMinutes / 60m;
}
