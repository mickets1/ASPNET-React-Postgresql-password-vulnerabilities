using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebAPI.Db;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly TheDbContext _context;

        public CategoriesController(TheDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var categories = await _context.Categories.Include(c => c.Passwords).ToListAsync();

            // If there are no category we return 204 No content.
            return categories.Count > 0 ? Ok(categories) : NoContent();
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.Include(c => c.Passwords).FirstOrDefaultAsync(c => c.Category_id == id);

            // If the category ID does not exist, we return 404 Not found.
            return category != null ? Ok(category) : NotFound();
        }

        // Add a new category
        [HttpPost("add")]
        public async Task<IActionResult> PostCategory([FromBody] JsonElement json)
        {
            try
            {
                var categoryName = json.GetProperty("category_name").GetString();
                

                if (string.IsNullOrEmpty(categoryName))
                {
                    return BadRequest("Category_name cannot be null or empty.");
                }

                var category = new Category { Category_name = categoryName };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCategory", new { id = category.Category_id }, category);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing JSON payload: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Update a specific category
        // PUT: api/Categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, [FromBody] JsonElement json)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);

                if (category == null)
                {
                    return NotFound();
                }

                // Update category properties based on JSON input
                category.Category_name = json.GetProperty("category_name").GetString();

                _context.Entry(category).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok($"Category with ID {id} has been updated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating category: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Remove a specific category
        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok($"Category with ID {id} has been deleted.");
        }
    }
}