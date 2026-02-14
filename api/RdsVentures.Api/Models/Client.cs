namespace RdsVentures.Api.Models;

public class Client
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string ContactEmail { get; set; }
    public string? Phone { get; set; }
    
    public ICollection<Property> Properties { get; set; } = new List<Property>();
}
