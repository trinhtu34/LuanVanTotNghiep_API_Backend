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
            var cartItem = await _context.Carts
                .Where(c => c.CartId == cartId)
                .Select(c => new DTO_Cart
                {
                    CartId = c.CartId,
                    UserId = c.UserId,
                    OrderTime = c.OrderTime,
                    IsCancel = c.IsCancel
                }).FirstOrDefaultAsync();
            if (cartItem == null)
                return NotFound();
            return Ok(cartItem);
        }
    }
}
