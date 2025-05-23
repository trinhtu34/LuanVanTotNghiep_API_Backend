using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api_LuanVan.Models;
using api_LuanVan.DataTransferObject;

namespace api_LuanVan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly DbluanvantotnghiepContext _context;

        public CategoryController(DbluanvantotnghiepContext context)
        {
            _context = context;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_Category>>> GetAllCategories()
        {
            return await _context.Categories
                .Select(c => new DTO_Category
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DTO_Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return new DTO_Category
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName
            };
        }

        // POST: api/Category
        [HttpPost]
        public async Task<ActionResult<DTO_Category>> PostCategory([FromBody] DTO_Category dto)
        {
            var category = new Category
            {
                CategoryName = dto.CategoryName
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            dto.CategoryId = category.CategoryId;

            return CreatedAtAction(nameof(GetCategory), new { id = dto.CategoryId }, dto);
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        public async Task<ActionResult<DTO_Category>> UpdateCategory(int id,[FromBody] DTO_Category dto)
        {
            if (id != dto.CategoryId)
            {
                return BadRequest("ID in URL and Body do not match");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            category.CategoryName = dto.CategoryName;
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new DTO_Category
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName
            });
        }

        //// DELETE: api/Category/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteCategory(int id)
        //{
        //    var category = await _context.Categories.FindAsync(id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Categories.Remove(category);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
