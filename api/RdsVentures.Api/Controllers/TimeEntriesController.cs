using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RdsVentures.Api.Data;
using RdsVentures.Api.DTOs;
using RdsVentures.Api.Models;

namespace RdsVentures.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeEntriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public TimeEntriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TimeEntryDto>>> GetTimeEntries()
    {
        var entries = await _context.TimeEntries
            .Include(te => te.Technician)
            .Include(te => te.ServiceRequest)
            .Select(te => new TimeEntryDto
            {
                Id = te.Id,
                TechId = te.TechId,
                ServiceRequestId = te.ServiceRequestId,
                StartUtc = te.StartUtc,
                EndUtc = te.EndUtc,
                DurationMinutes = te.DurationMinutes,
                WeekStartMondayUtc = te.WeekStartMondayUtc,
                TechnicianName = te.Technician != null ? te.Technician.Name : null,
                ServiceRequestTitle = te.ServiceRequest != null ? te.ServiceRequest.Title : null
            })
            .ToListAsync();

        return Ok(entries);
    }

    [HttpGet("technician/{techId}")]
    public async Task<ActionResult<IEnumerable<TimeEntryDto>>> GetTechnicianTimeEntries(int techId)
    {
        var entries = await _context.TimeEntries
            .Include(te => te.Technician)
            .Include(te => te.ServiceRequest)
            .Where(te => te.TechId == techId)
            .Select(te => new TimeEntryDto
            {
                Id = te.Id,
                TechId = te.TechId,
                ServiceRequestId = te.ServiceRequestId,
                StartUtc = te.StartUtc,
                EndUtc = te.EndUtc,
                DurationMinutes = te.DurationMinutes,
                WeekStartMondayUtc = te.WeekStartMondayUtc,
                TechnicianName = te.Technician != null ? te.Technician.Name : null,
                ServiceRequestTitle = te.ServiceRequest != null ? te.ServiceRequest.Title : null
            })
            .ToListAsync();

        return Ok(entries);
    }

    [HttpPost("start")]
    public async Task<ActionResult<TimeEntryDto>> StartTimeEntry(StartTimeEntryDto dto)
    {
        var now = DateTime.UtcNow;
        var weekStart = GetWeekStartMonday(now);

        var timeEntry = new TimeEntry
        {
            TechId = dto.TechId,
            ServiceRequestId = dto.ServiceRequestId,
            StartUtc = now,
            WeekStartMondayUtc = weekStart
        };

        _context.TimeEntries.Add(timeEntry);
        await _context.SaveChangesAsync();

        var result = await _context.TimeEntries
            .Include(te => te.Technician)
            .Include(te => te.ServiceRequest)
            .Where(te => te.Id == timeEntry.Id)
            .Select(te => new TimeEntryDto
            {
                Id = te.Id,
                TechId = te.TechId,
                ServiceRequestId = te.ServiceRequestId,
                StartUtc = te.StartUtc,
                EndUtc = te.EndUtc,
                DurationMinutes = te.DurationMinutes,
                WeekStartMondayUtc = te.WeekStartMondayUtc,
                TechnicianName = te.Technician != null ? te.Technician.Name : null,
                ServiceRequestTitle = te.ServiceRequest != null ? te.ServiceRequest.Title : null
            })
            .FirstAsync();

        return CreatedAtAction(nameof(GetTimeEntries), new { id = timeEntry.Id }, result);
    }

    [HttpPost("stop")]
    public async Task<ActionResult<TimeEntryDto>> StopTimeEntry(StopTimeEntryDto dto)
    {
        var timeEntry = await _context.TimeEntries.FindAsync(dto.TimeEntryId);
        if (timeEntry == null)
            return NotFound();

        if (timeEntry.EndUtc.HasValue)
            return BadRequest("Time entry already stopped");

        var now = DateTime.UtcNow;
        timeEntry.EndUtc = now;
        timeEntry.DurationMinutes = (int)(now - timeEntry.StartUtc).TotalMinutes;

        await _context.SaveChangesAsync();

        var result = await _context.TimeEntries
            .Include(te => te.Technician)
            .Include(te => te.ServiceRequest)
            .Where(te => te.Id == timeEntry.Id)
            .Select(te => new TimeEntryDto
            {
                Id = te.Id,
                TechId = te.TechId,
                ServiceRequestId = te.ServiceRequestId,
                StartUtc = te.StartUtc,
                EndUtc = te.EndUtc,
                DurationMinutes = te.DurationMinutes,
                WeekStartMondayUtc = te.WeekStartMondayUtc,
                TechnicianName = te.Technician != null ? te.Technician.Name : null,
                ServiceRequestTitle = te.ServiceRequest != null ? te.ServiceRequest.Title : null
            })
            .FirstAsync();

        return Ok(result);
    }

    [HttpGet("weekly")]
    public async Task<ActionResult<IEnumerable<WeeklyTimeDto>>> GetWeeklyTotals([FromQuery] DateTime? weekStart = null)
    {
        var targetWeekStart = weekStart.HasValue ? GetWeekStartMonday(weekStart.Value) : GetWeekStartMonday(DateTime.UtcNow);

        var weeklyTotals = await _context.TimeEntries
            .Include(te => te.Technician)
            .Where(te => te.WeekStartMondayUtc == targetWeekStart && te.DurationMinutes.HasValue)
            .GroupBy(te => new { te.TechId, te.Technician!.Name, te.WeekStartMondayUtc })
            .Select(g => new WeeklyTimeDto
            {
                TechId = g.Key.TechId,
                TechnicianName = g.Key.Name,
                WeekStart = g.Key.WeekStartMondayUtc,
                TotalMinutes = g.Sum(te => te.DurationMinutes!.Value)
            })
            .ToListAsync();

        return Ok(weeklyTotals);
    }

    private static DateTime GetWeekStartMonday(DateTime date)
    {
        var offset = ((int)date.DayOfWeek - 1 + 7) % 7;
        return date.Date.AddDays(-offset);
    }
}
