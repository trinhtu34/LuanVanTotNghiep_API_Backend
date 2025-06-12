using api_LuanVan.DataTransferObject;
using api_LuanVan.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_LuanVan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly DbluanvantotnghiepContext _context;

        public MenuController(DbluanvantotnghiepContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_Menu>>> GetAllMenus()
        {
            return await _context.Menus
                .Select(m => new DTO_Menu
                {
                    DishId = m.DishId,
                    DishName = m.DishName,
                    Price = m.Price,
                    Descriptions = m.Descriptions,
                    CategoryId = m.CategoryId,
                    RegionId = m.RegionId,
                    Images = m.Images
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DTO_Menu>> GetMenu(string id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
                return NotFound();

            return new DTO_Menu
            {
                DishId = menu.DishId,
                DishName = menu.DishName,
                Price = menu.Price,
                Descriptions = menu.Descriptions,
                CategoryId = menu.CategoryId,
                RegionId = menu.RegionId,
                Images = menu.Images
            };
        }

        [HttpPost]
        public async Task<ActionResult<DTO_Menu>> CreateMenu([FromBody] DTO_Menu dto)
        {
            var menu = new Menu
            {
                DishId = dto.DishId,
                DishName = dto.DishName,
                Price = dto.Price,
                Descriptions = dto.Descriptions,
                CategoryId = dto.CategoryId,
                RegionId = dto.RegionId,
                Images = dto.Images
            };
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMenu), new { id = menu.DishId }, menu);
            //return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DTO_Menu>> UpdateMenu(string id, [FromBody] DTO_Menu dto)
        {
            if (id != dto.DishId)
                return BadRequest("DishId in URL and body do not match.");

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
                return NotFound();

            menu.DishName = dto.DishName;
            menu.Price = dto.Price;
            menu.Descriptions = dto.Descriptions;
            menu.CategoryId = dto.CategoryId;
            menu.RegionId = dto.RegionId;
            menu.Images = dto.Images;

            _context.Entry(menu).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(dto);
        }

        //// DELETE: api/Menu/{id}
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteMenu(string id)
        //{
        //    var menu = await _context.Menus.FindAsync(id);
        //    if (menu == null)
        //        return NotFound();

        //    _context.Menus.Remove(menu);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}
    }
}
