using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RdsVentures.Api.Data;
using RdsVentures.Api.DTOs;
using RdsVentures.Api.Models;

namespace RdsVentures.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceRequestsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ServiceRequestsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceRequestDto>>> GetServiceRequests()
    {
        var requests = await _context.ServiceRequests
            .Include(sr => sr.Property)
                .ThenInclude(p => p!.Client)
            .Include(sr => sr.AssignedTech)
            .Select(sr => new ServiceRequestDto
            {
                Id = sr.Id,
                PropertyId = sr.PropertyId,
                Title = sr.Title,
                Description = sr.Description,
                Priority = sr.Priority,
                Status = sr.Status,
                ScheduledAt = sr.ScheduledAt,
                CompletedAt = sr.CompletedAt,
                AssignedTechId = sr.AssignedTechId,
                AssignedTechName = sr.AssignedTech != null ? sr.AssignedTech.Name : null,
                PropertyAddress = sr.Property != null ? sr.Property.Address : null,
                ClientName = sr.Property != null && sr.Property.Client != null ? sr.Property.Client.Name : null
            })
            .ToListAsync();

        return Ok(requests);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceRequestDto>> GetServiceRequest(int id)
    {
        var request = await _context.ServiceRequests
            .Include(sr => sr.Property)
                .ThenInclude(p => p!.Client)
            .Include(sr => sr.AssignedTech)
            .Where(sr => sr.Id == id)
            .Select(sr => new ServiceRequestDto
            {
                Id = sr.Id,
                PropertyId = sr.PropertyId,
                Title = sr.Title,
                Description = sr.Description,
                Priority = sr.Priority,
                Status = sr.Status,
                ScheduledAt = sr.ScheduledAt,
                CompletedAt = sr.CompletedAt,
                AssignedTechId = sr.AssignedTechId,
                AssignedTechName = sr.AssignedTech != null ? sr.AssignedTech.Name : null,
                PropertyAddress = sr.Property != null ? sr.Property.Address : null,
                ClientName = sr.Property != null && sr.Property.Client != null ? sr.Property.Client.Name : null
            })
            .FirstOrDefaultAsync();

        if (request == null)
            return NotFound();

        return Ok(request);
    }

    [HttpPost]
    public async Task<ActionResult<ServiceRequestDto>> CreateServiceRequest(CreateServiceRequestDto dto)
    {
        var serviceRequest = new ServiceRequest
        {
            PropertyId = dto.PropertyId,
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            Status = Status.Open,
            ScheduledAt = dto.ScheduledAt
        };

        _context.ServiceRequests.Add(serviceRequest);
        await _context.SaveChangesAsync();

        var result = await GetServiceRequest(serviceRequest.Id);
        return CreatedAtAction(nameof(GetServiceRequest), new { id = serviceRequest.Id }, result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateServiceRequest(int id, UpdateServiceRequestDto dto)
    {
        var serviceRequest = await _context.ServiceRequests.FindAsync(id);
        if (serviceRequest == null)
            return NotFound();

        if (dto.Title != null)
            serviceRequest.Title = dto.Title;
        if (dto.Description != null)
            serviceRequest.Description = dto.Description;
        if (dto.Priority.HasValue)
            serviceRequest.Priority = dto.Priority.Value;
        if (dto.Status.HasValue)
        {
            serviceRequest.Status = dto.Status.Value;
            if (dto.Status.Value == Status.Complete && !serviceRequest.CompletedAt.HasValue)
                serviceRequest.CompletedAt = DateTime.UtcNow;
        }
        if (dto.ScheduledAt.HasValue)
            serviceRequest.ScheduledAt = dto.ScheduledAt;
        if (dto.AssignedTechId.HasValue)
            serviceRequest.AssignedTechId = dto.AssignedTechId.Value;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteServiceRequest(int id)
    {
        var serviceRequest = await _context.ServiceRequests.FindAsync(id);
        if (serviceRequest == null)
            return NotFound();

        _context.ServiceRequests.Remove(serviceRequest);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/assign")]
    public async Task<IActionResult> AssignTechnician(int id, [FromBody] int technicianId)
    {
        var serviceRequest = await _context.ServiceRequests.FindAsync(id);
        if (serviceRequest == null)
            return NotFound();

        serviceRequest.AssignedTechId = technicianId;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
