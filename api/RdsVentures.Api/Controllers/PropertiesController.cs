using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RdsVentures.Api.Data;
using RdsVentures.Api.Models;

namespace RdsVentures.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly AppDbContext _context;

    public PropertiesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Property>>> GetProperties()
    {
        return await _context.Properties.Include(p => p.Client).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Property>> GetProperty(int id)
    {
        var property = await _context.Properties.Include(p => p.Client).FirstOrDefaultAsync(p => p.Id == id);
        if (property == null)
            return NotFound();
        return property;
    }

    [HttpPost]
    public async Task<ActionResult<Property>> CreateProperty(Property property)
    {
        _context.Properties.Add(property);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetProperty), new { id = property.Id }, property);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProperty(int id, Property property)
    {
        if (id != property.Id)
            return BadRequest();

        _context.Entry(property).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProperty(int id)
    {
        var property = await _context.Properties.FindAsync(id);
        if (property == null)
            return NotFound();

        _context.Properties.Remove(property);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
