using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebAPI.Db;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeUnitsController : ControllerBase
    {
        private readonly TheDbContext _context;

        public TimeUnitsController(TheDbContext context)
        {
            _context = context;
        }

        // Create a new time unit
        // POST: api/timeunits/add
        [HttpPost("add")]
        public async Task<ActionResult<TimeUnit>> PostTimeUnit([FromBody] JsonElement json)
        {
            try
            {
                var timeUnitName = json.GetProperty("time_unit_name").GetString();

                if (string.IsNullOrEmpty(timeUnitName))
                {
                    return BadRequest("Time_unit_name cannot be null or empty.");
                }

                var timeUnit = new TimeUnit { Time_unit_name = timeUnitName };

                _context.TimeUnits.Add(timeUnit);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetTimeUnit", new { id = timeUnit.Time_unit_id }, timeUnit);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing JSON payload: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Read all time units
        // GET: api/TimeUnits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TimeUnit>>> GetTimeUnits()
        {
            var timeUnits = await _context.TimeUnits.ToListAsync();

            return timeUnits.Count > 0 ? Ok(timeUnits) : NoContent();
        }

        // Read a specific time unit
        // GET: api/TimeUnits/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TimeUnit>> GetTimeUnit(int id)
        {
            var timeUnit = await _context.TimeUnits.FindAsync(id);

            return timeUnit != null ? Ok(timeUnit) : NotFound();
        }

        // PUT: api/TimeUnits/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTimeUnit(int id, [FromBody] JsonElement json)
        {
            try
            {
                var existingTimeUnit = await _context.TimeUnits.FindAsync(id);

                if (existingTimeUnit == null)
                {
                    return NotFound();
                }

                // Update the properties based on the JSON input
                existingTimeUnit.Time_unit_name = json.GetProperty("time_unit_name").GetString();

                _context.Entry(existingTimeUnit).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(existingTimeUnit);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating time unit: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // DELETE: api/TimeUnits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeUnit(int id)
        {
            try
            {
                var timeUnit = await _context.TimeUnits.FindAsync(id);
                if (timeUnit == null)
                {
                    return NotFound();
                }

                _context.TimeUnits.Remove(timeUnit);
                await _context.SaveChangesAsync();

                return Ok("TimeUnit deleted successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting time unit: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}