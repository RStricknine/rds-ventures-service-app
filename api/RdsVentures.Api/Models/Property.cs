namespace RdsVentures.Api.Models;

public class Property
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string Zip { get; set; }
    public string? Notes { get; set; }
    
    public Client? Client { get; set; }
    public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
}
