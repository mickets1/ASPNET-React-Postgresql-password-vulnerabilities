using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebAPI.Db;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordsController : ControllerBase
    {
        private readonly TheDbContext _context;
        private const int PageSize = 10;

        public PasswordsController(TheDbContext context)
        {
            _context = context;
        }

        // Read all passwords by page number
        // GET: api/Passwords/with-relations?page={page}
        [HttpGet("with-relations")]
        public async Task<ActionResult<IEnumerable<object>>> GetPasswordsWithRelations(int page = 1)
        {
            var passwords = await _context.Passwords
                .Include(p => p.Category)
                .Include(p => p.TimeUnit)
                .OrderBy(p => p.Strength)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(p => new
                {
                    PasswordId = p.Password_id,
                    PasswordText = p.Password_value,
                    Strength = p.Strength,
                    CategoryName = p.Category.Category_name,
                    TimeUnitName = p.TimeUnit.Time_unit_name,
                    Value = p.Value,
                    OfflineCrackSec = p.Offline_crack_sec,
                    RankAlt = p.Rank_alt,
                    FontSize = p.Font_size
                    
                })
                .ToListAsync();

            return Ok(passwords);
        }

        // Read a specific password
        // GET: api/Passwords/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Password>> GetPassword(int id)
        {
            var password = await _context.Passwords.Include(p => p.Category).Include(p => p.TimeUnit).FirstOrDefaultAsync(p => p.Password_id == id);

            return password != null ? Ok(password) : NotFound();
        }

    // Create a new Password
    // POST: api/passwords/add
    [HttpPost("add")]
    public async Task<ActionResult<Password>> PostPassword([FromBody] JsonElement json)
    {
        try
        {
            // Validate that the input is a valid JSON
            if (json.ValueKind != JsonValueKind.Object)
            {
                return BadRequest("Invalid JSON format");
            }

            // Parse JSON input
            var password = new Password
            {
                Password_value = json.GetProperty("password_value").GetString(),
                Strength = int.TryParse(json.GetProperty("strength").GetString(), out var strength) ? strength : 0,
                Font_size = int.TryParse(json.GetProperty("font_size").GetString(), out var fontSize) ? fontSize : 0,
                Value = float.TryParse(json.GetProperty("value").GetString(), out var value) ? value : 0,
                Offline_crack_sec = float.TryParse(json.GetProperty("offline_crack_sec").GetString(), out var offlineCrackSec) ? offlineCrackSec : 0,
                Rank_alt = int.TryParse(json.GetProperty("rank_alt").GetString(), out var rankAlt) ? rankAlt : 0,
            };

            // Check if category name is provided in the JSON
            if (json.TryGetProperty("category_name", out var categoryNameElement))
            {
                // Check if category already exists
                // Avoid creating empty/duplicate entries
                var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Category_name == categoryNameElement.GetString());
                if (existingCategory == null)
                {
                    // If category doesn't exist, create a new one
                    var category = new Category { Category_name = categoryNameElement.GetString() };
                    _context.Categories.Add(category);
                    password.Category = category;
                }
                else
                {
                    password.Category_id = existingCategory.Category_id; // Set existing category ID for the password
                }
            }

            // Check if time unit name is provided in the JSON
            if (json.TryGetProperty("time_unit_name", out var timeUnitNameElement))
            {
                // Check if time unit already exists
                // Avoid creating empty/duplicate entries
                var existingTimeUnit = await _context.TimeUnits.FirstOrDefaultAsync(t => t.Time_unit_name == timeUnitNameElement.GetString());
                if (existingTimeUnit == null)
                {
                    // If time unit doesn't exist, create a new one
                    var timeUnit = new TimeUnit { Time_unit_name = timeUnitNameElement.GetString() };
                    _context.TimeUnits.Add(timeUnit);
                    password.TimeUnit = timeUnit; // Set time unit for the password
                }
                else
                {
                    password.Time_unit_id = existingTimeUnit.Time_unit_id;
                }
            }

            _context.Passwords.Add(password);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPassword", new { id = password.Password_id }, password);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing password: {ex.Message}");
            return StatusCode(500, "Internal Server Error");
        }
    }

        // Update a specific password entity
        // PUT: api/passwords/{id}
       [HttpPut("{id}")]
        public async Task<ActionResult> PutPassword(int id, [FromBody] JsonElement json)
        {
            try
            {
                // Validate that the input is a valid JSON
                if (json.ValueKind != JsonValueKind.Object)
                {
                    return BadRequest("Invalid JSON format");
                }
                

                // Find the existing password by id
                var existingPassword = await _context.Passwords
                    .Include(p => p.Category)
                    .Include(p => p.TimeUnit)
                    .FirstOrDefaultAsync(p => p.Password_id == id);
                
                if (existingPassword == null)
                {
                    return NotFound("Password not found");
                }

                // Update the existing password
                existingPassword.Password_value = json.GetProperty("password_value").GetString();
                existingPassword.Strength = int.TryParse(json.GetProperty("strength").GetString(), out var strength) ? strength : 0;
                existingPassword.Font_size = int.TryParse(json.GetProperty("font_size").GetString(), out var fontSize) ? fontSize : 0;
                existingPassword.Value = float.TryParse(json.GetProperty("value").GetString(), out var value) ? value : 0;
                existingPassword.Offline_crack_sec = float.TryParse(json.GetProperty("offline_crack_sec").GetString(), out var offlineCrackSec) ? offlineCrackSec : 0;
                existingPassword.Rank_alt = int.TryParse(json.GetProperty("rank_alt").GetString(), out var rankAlt) ? rankAlt : 0;

                // Check if category name and time unit name are provided in the JSON
                if (json.TryGetProperty("category_name", out var categoryNameElement))
                {
                    // Create or retrieve the category and associate it with the password
                    var categoryName = categoryNameElement.GetString();
                    var category = await _context.Categories.FirstOrDefaultAsync(c => c.Category_name == categoryName);
                    if (category == null)
                    {
                        category = new Category { Category_name = categoryName };
                        _context.Categories.Add(category);
                    }
                    existingPassword.Category = category;
                }

                if (json.TryGetProperty("time_unit_name", out var timeUnitNameElement))
                {
                    // Create or retrieve the time unit and associate it with the password
                    var timeUnitName = timeUnitNameElement.GetString();
                    var timeUnit = await _context.TimeUnits.FirstOrDefaultAsync(t => t.Time_unit_name == timeUnitName);
                    if (timeUnit == null)
                    {
                        timeUnit = new TimeUnit { Time_unit_name = timeUnitName };
                        _context.TimeUnits.Add(timeUnit);
                    }
                    existingPassword.TimeUnit = timeUnit;
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                return Ok(existingPassword);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating password: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Delete a specific password entity
        // DELETE: api/Passwords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePassword(int id)
        {
            var password = await _context.Passwords.FindAsync(id);
            if (password == null)
            {
                return NotFound();
            }

            _context.Passwords.Remove(password);
            await _context.SaveChangesAsync();

            return Ok($"Password with ID {id} has been deleted.");
        }
    }
}