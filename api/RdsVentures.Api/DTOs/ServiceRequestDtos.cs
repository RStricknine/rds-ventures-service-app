using RdsVentures.Api.Models;

namespace RdsVentures.Api.DTOs;

public class ServiceRequestDto
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Priority Priority { get; set; }
    public Status Status { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? AssignedTechId { get; set; }
    public string? AssignedTechName { get; set; }
    public string? PropertyAddress { get; set; }
    public string? ClientName { get; set; }
}

public class CreateServiceRequestDto
{
    public int PropertyId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Priority Priority { get; set; }
    public DateTime? ScheduledAt { get; set; }
}

public class UpdateServiceRequestDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Priority? Priority { get; set; }
    public Status? Status { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public int? AssignedTechId { get; set; }
}
