using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RdsVentures.Api.Data;
using RdsVentures.Api.Models;

namespace RdsVentures.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TechniciansController : ControllerBase
{
    private readonly AppDbContext _context;

    public TechniciansController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Technician>>> GetTechnicians()
    {
        return await _context.Technicians.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Technician>> GetTechnician(int id)
    {
        var technician = await _context.Technicians.FindAsync(id);
        if (technician == null)
            return NotFound();
        return technician;
    }

    [HttpPost]
    public async Task<ActionResult<Technician>> CreateTechnician(Technician technician)
    {
        _context.Technicians.Add(technician);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTechnician), new { id = technician.Id }, technician);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTechnician(int id, Technician technician)
    {
        if (id != technician.Id)
            return BadRequest();

        _context.Entry(technician).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTechnician(int id)
    {
        var technician = await _context.Technicians.FindAsync(id);
        if (technician == null)
            return NotFound();

        _context.Technicians.Remove(technician);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
