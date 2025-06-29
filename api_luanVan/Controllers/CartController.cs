using api_LuanVan.DataTransferObject;
using api_LuanVan.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_LuanVan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly Dbluanvan2Context _context;
        public CartController(Dbluanvan2Context context)
        {
            _context = context;
        }
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<DTO_Cart>>> GetCartByUserId(string userId)
        {
            var cartItems = await _context.Carts
                .Where(c => c.UserId == userId)
                .Select(c => new DTO_Cart
                {
                    CartId = c.CartId,
                    UserId = c.UserId,
                    OrderTime = c.OrderTime,
                    IsCancel = c.IsCancel
                }).ToListAsync();
            if (cartItems == null || cartItems.Count == 0)
                return NotFound();
            return Ok(cartItems);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_Cart>>> GetAllCarts()
        {
            var cartItems = await _context.Carts
                .Select(c => new DTO_Cart
                {
                    CartId = c.CartId,
                    UserId = c.UserId,
                    OrderTime = c.OrderTime,
                    IsCancel = c.IsCancel
                }).ToListAsync();
            if (cartItems == null || cartItems.Count == 0)
                return NotFound();
            return Ok(cartItems);
        }

        [HttpGet("afterCurrentOrderTime")]
        public async Task<ActionResult<IEnumerable<DTO_Cart>>> GetCartsAfterCurrentOrderTime()
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone).AddHours(-1);
            var cartItems = await _context.Carts
                .Where(c => c.OrderTime > currentTime && c.IsCancel == false)
                .Select(c => new DTO_Cart
                {
                    CartId = c.CartId,
                    UserId = c.UserId,
                    OrderTime = c.OrderTime,
                    IsCancel = c.IsCancel
                }).ToListAsync();
            if (cartItems == null || cartItems.Count == 0)
                return NotFound();
            return Ok(cartItems);
        }

        [HttpGet("{cartId}")]
        public async Task<ActionResult<DTO_Cart>> GetCartById(int cartId)
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var cartItem = await _context.Carts
                .Where(c => c.CartId == cartId)
                .Select(c => new DTO_Cart
                {
                    CartId = c.CartId,
                    UserId = c.UserId,
                    OrderTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone),
                    IsCancel = c.IsCancel
                }).FirstOrDefaultAsync();
            if (cartItem == null)
                return NotFound();
            return Ok(cartItem);
        }

        [HttpPost]
        public async Task<ActionResult<DTO_Cart>> CreateCart(DTO_Cart dtoCart)
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var newCart = new Cart
            {
                UserId = dtoCart.UserId,
                OrderTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone),
                IsCancel = dtoCart.IsCancel
            };
            _context.Carts.Add(newCart);
            await _context.SaveChangesAsync();
            dtoCart.CartId = newCart.CartId;
            dtoCart.OrderTime = newCart.OrderTime;
            dtoCart.IsCancel = newCart.IsCancel;
            return CreatedAtAction(nameof(GetCartById), new { cartId = newCart.CartId }, dtoCart);
        }

        [HttpPut("{cartId}")]
        public async Task<IActionResult> UpdateCart(int cartId, DTO_Cart dtoCart)
        {
            if (cartId != dtoCart.CartId)
                return BadRequest("Cart ID mismatch");
            var existingCart = await _context.Carts.FindAsync(cartId);
            if (existingCart == null)
                return NotFound();
            existingCart.UserId = dtoCart.UserId;
            existingCart.OrderTime = dtoCart.OrderTime;
            existingCart.IsCancel = dtoCart.IsCancel;
            _context.Entry(existingCart).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
