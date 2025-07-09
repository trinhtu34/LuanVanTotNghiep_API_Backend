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
                    TotalPrice = c.TotalPrice,
                    IsCancel = c.IsCancel
                }).ToListAsync();
            if (cartItems == null || cartItems.Count == 0)
                return NotFound();
            return Ok(cartItems);
        }

        // lấy tất cả thông tin giỏ hàng , kèm theo thông tin thanh toán và trạng thái hoàn thành
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_Cart_WithPaymentInfo_andIsFinish>>> GetAllCarts()
        {
            var cartItems = await _context.Carts
                .Select(c => new DTO_Cart_WithPaymentInfo_andIsFinish
                {
                    CartId = c.CartId,
                    UserId = c.UserId,
                    OrderTime = c.OrderTime,
                    TotalPrice = c.TotalPrice,
                    IsCancel = c.IsCancel,
                    IsFinish = c.IsFinish,
                    IsPaid = _context.PaymentResults.Any(p => p.CartId == c.CartId && p.IsSuccess == true)
                }).ToListAsync();
            if (cartItems == null || cartItems.Count == 0)
                return NotFound();
            return Ok(cartItems);
        }

        // lấy thông tin giỏ hàng từ 2 tiếng trước trở về sau 
        [HttpGet("afterCurrentOrderTime")]
        public async Task<ActionResult<IEnumerable<DTO_Cart_WithPaymentInfo_andIsFinish>>> GetCartsAfterCurrentOrderTime()
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone).AddHours(-2);
            var cartItems = await _context.Carts
                .Where(c => c.OrderTime > currentTime && c.IsCancel == false)
                .Select(c => new DTO_Cart_WithPaymentInfo_andIsFinish
                {
                    CartId = c.CartId,
                    UserId = c.UserId,
                    OrderTime = c.OrderTime,
                    TotalPrice = c.TotalPrice,
                    IsCancel = c.IsCancel,
                    IsFinish = c.IsFinish,
                    IsPaid = _context.PaymentResults.Any(p => p.CartId == c.CartId && p.IsSuccess == true)
                }).ToListAsync();
            if (cartItems == null || cartItems.Count == 0)
                return NotFound();
            return Ok(cartItems);
        }

        // lấy thông tin giỏ hàng theo mã giỏ hàng 
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
                    TotalPrice = c.TotalPrice,
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
                TotalPrice = dtoCart.TotalPrice,
                IsCancel = false
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
            var existingCart = await _context.Carts.FindAsync(cartId);
            if (existingCart == null)
                return NotFound();
            existingCart.IsCancel = dtoCart.IsCancel;
            _context.Entry(existingCart).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // api thay đổi trạng thái của 1 giỏ hàng 
        [HttpPut("state/{id}")]
        public async Task<ActionResult<DTO_Cart_Cancel_Status>> UpdateOrderTableByState(bool id, [FromBody] DTO_Cart_Cancel_Status dto)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
                return NotFound();

            cart.IsCancel = dto.IsCancel;
            _context.Entry(cart).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // api thay đổi trạng thái hoàn thành của 1 giỏ hàng 
        [HttpPut("stateFinish/{id}")]
        public async Task<ActionResult<DTO_Cart_WithPaymentInfo_andIsFinish>> UpdateOrderTableByState(bool id, [FromBody] DTO_Cart_WithPaymentInfo_andIsFinish dto)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
                return NotFound();

            cart.IsFinish = dto.IsFinish;
            _context.Entry(cart).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // đếm số lượng tất cả đơn đặt món ăn ( giỏ hàng ) của 1 user id
        [HttpGet("user/count/{userid}")]
        public async Task<ActionResult<int>> GetCartCountByUserId(string userid)
        {
            var count = await _context.Carts
                .Where(c => c.UserId == userid && c.IsCancel == false)
                .CountAsync();
            return Ok(count);
        }
        // đếm số lượng giỏ hàng đã thanh toán của 1 user id
        [HttpGet("user/paid/count/{userid}")]
        public async Task<ActionResult<int>> GetPaidCartCountByUserId(string userid)
        {
            var count = await _context.Carts
                .Where(c => c.UserId == userid && c.IsCancel == false)
                .Where(c => _context.PaymentResults
                    .Any(p => p.CartId == c.CartId && p.IsSuccess == true))
                .CountAsync();
            return Ok(count);
        }
        // đếm số lượng giỏ hàng chưa thanh toán của 1 user id
        [HttpGet("user/unpaid/count/{userid}")] 
        public async Task<ActionResult<int>> GetUnpaidCartCountByUserId(string userid)
        {
            var count = await _context.Carts
                .Where(c => c.UserId == userid && c.IsCancel == false)
                .Where(c => !_context.PaymentResults
                    .Any(p => p.CartId == c.CartId && p.IsSuccess == true))
                .CountAsync();
            return Ok(count);
        }
        // tổng giá trị của tất cả giỏ hàng của 1 user id
        [HttpGet("user/totalPrice/{userId}")]
        public async Task<ActionResult<decimal>> GetTotalPriceByUserId(string userId)
        {
            var totalPrice = await _context.Carts
                .Where(c => c.UserId == userId && c.IsCancel == false)
                .SumAsync(c => c.TotalPrice);
            return Ok(totalPrice);
        }
        // tổng giá trị của giỏ hàng đã thanh toán của 1 user id
        [HttpGet("user/totalPrice/paid/{userId}")]
        public async Task<ActionResult<decimal>> GetTotalPaidPriceByUserId(string userId)
        {
            var totalPrice = await _context.Carts
                .Where(c => c.UserId == userId && c.IsCancel == false)
                .Where(c => _context.PaymentResults
                    .Any(p => p.CartId == c.CartId && p.IsSuccess == true))
                .SumAsync(c => c.TotalPrice);
            return Ok(totalPrice);
        }
        // tổng giá trị của giỏ hàng chưa thanh toán của 1 user id
        [HttpGet("user/totalPrice/unpaid/{userId}")]
        public async Task<ActionResult<decimal>> GetTotalUnpaidPriceByUserId(string userId)
        {
            var totalPrice = await _context.Carts
                .Where(c => c.UserId == userId && c.IsCancel == false)
                .Where(c => !_context.PaymentResults
                    .Any(p => p.CartId == c.CartId && p.IsSuccess == true))
                .SumAsync(c => c.TotalPrice);
            return Ok(totalPrice);
        }



        //// Lấy thông tin giỏ hàng đã thanh toán của người dùng theo userId
        //[HttpGet("user/paid/{userid}")]
        //public async Task<ActionResult<IEnumerable<DTO_Cart>>> GetPaidCartsByUserId(string userId)
        //{
        //    var cartItems = await _context.Carts
        //        .Where(c => c.UserId == userId)
        //        .Where(c => _context.PaymentResults
        //            .Any(p => p.CartId == c.CartId && p.IsSuccess == true))
        //        .Select(c => new DTO_Cart
        //        {
        //            CartId = c.CartId,
        //            UserId = c.UserId,
        //            OrderTime = c.OrderTime,
        //            TotalPrice = c.TotalPrice,
        //            IsCancel = c.IsCancel
        //        }).ToListAsync();
        //    if (cartItems == null || cartItems.Count == 0)
        //        return NotFound();
        //    return Ok(cartItems);
        //}

        //// Lấy thông tin giỏ hàng chưa thanh toán của người dùng theo userId
        //[HttpGet("user/unpaid/{userid}")]
        //public async Task<ActionResult<IEnumerable<DTO_Cart>>> GetUnpaidCartsByUserId(string userId)
        //{
        //    var cartItems = await _context.Carts
        //        .Where(c => c.UserId == userId)
        //        .Where(c => !_context.PaymentResults
        //            .Any(p => p.CartId == c.CartId && p.IsSuccess == true))
        //        .Select(c => new DTO_Cart
        //        {
        //            CartId = c.CartId,
        //            UserId = c.UserId,
        //            OrderTime = c.OrderTime,
        //            TotalPrice = c.TotalPrice,
        //            IsCancel = c.IsCancel
        //        }).ToListAsync();
        //    if (cartItems == null || cartItems.Count == 0)
        //        return NotFound();
        //    return Ok(cartItems);
        //}

    }
}
