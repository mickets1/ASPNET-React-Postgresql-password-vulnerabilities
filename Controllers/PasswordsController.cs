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

                var model = new Password
                {
                    Password_value = json.GetProperty("password_value").GetString(),
                    Strength = int.TryParse(json.GetProperty("strength").GetString(), out var strength) ? strength : 0,
                    Font_size = int.TryParse(json.GetProperty("font_size").GetString(), out var fontSize) ? fontSize : 0,
                    Value = float.TryParse(json.GetProperty("value").GetString(), out var value) ? value : 0,
                    Offline_crack_sec = float.TryParse(json.GetProperty("offline_crack_sec").GetString(), out var offlineCrackSec) ? offlineCrackSec : 0,
                    Rank_alt = int.TryParse(json.GetProperty("rank_alt").GetString(), out var rankAlt) ? rankAlt : 0,
                    Category_id = json.GetProperty("category_id").GetInt32(),
                    Time_unit_id = json.GetProperty("time_unit_id").GetInt32()
                };

                // Add a new password
                var password = new Password
                {
                    Password_value = model.Password_value,
                    Strength = model.Strength,
                    Font_size = model.Font_size,
                    Value = model.Value,
                    Offline_crack_sec = model.Offline_crack_sec,
                    Rank_alt = model.Rank_alt,
                    Category_id = model.Category_id,
                    Time_unit_id = model.Time_unit_id
                };

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
        public async Task<IActionResult> PutPassword(int id, [FromBody] JsonElement json)
        {
            try
            {
                // Validate that the input is a valid JSON
                if (json.ValueKind != JsonValueKind.Object)
                {
                    return BadRequest("Invalid JSON format");
                }

                // Find the existing password by id
                var existingPassword = await _context.Passwords.FindAsync(id);
                
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
                existingPassword.Category_id = id;
                existingPassword.Time_unit_id = id;

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