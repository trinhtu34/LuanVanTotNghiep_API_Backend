using api_LuanVan.DataTransferObject;
using api_LuanVan.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_LuanVan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderFoodDetailController : ControllerBase
    {
        private readonly DbluanvantotnghiepContext _context;
        public OrderFoodDetailController(DbluanvantotnghiepContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_OrderFoodDetail>>> GetAllOrderFoodDetails()
        {
            return await _context.OrderFoodDetails
                .Select(ofd => new DTO_OrderFoodDetail
                {
                    OrderFoodDetailsId = ofd.OrderFoodDetailsId,
                    OrderTableId = ofd.OrderTableId,
                    DishId = ofd.DishId,
                    Quantity = ofd.Quantity,
                    Price = ofd.Price,
                    Note = ofd.Note
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DTO_OrderFoodDetail>> GetOrderFoodDetailById(int id)
        {
            var orderFoodDetail = await _context.OrderFoodDetails.FindAsync(id);
            if (orderFoodDetail == null)
                return NotFound();
            return new DTO_OrderFoodDetail
            {
                OrderFoodDetailsId = orderFoodDetail.OrderFoodDetailsId,
                OrderTableId = orderFoodDetail.OrderTableId,
                DishId = orderFoodDetail.DishId,
                Quantity = orderFoodDetail.Quantity,
                Price = orderFoodDetail.Price,
                Note = orderFoodDetail.Note
            };
        }
        [HttpGet("list/{orderTableId}")]
        public async Task<ActionResult<IEnumerable<DTO_OrderFoodDetail>>> GetOrderFoodDetailsByOrderTableId(long orderTableId)
        {
            var orderFoodDetails = await _context.OrderFoodDetails
                .Where(ofd => ofd.OrderTableId == orderTableId)
                .Select(ofd => new DTO_OrderFoodDetail
                {
                    OrderFoodDetailsId = ofd.OrderFoodDetailsId,
                    OrderTableId = ofd.OrderTableId,
                    DishId = ofd.DishId,
                    Quantity = ofd.Quantity,
                    Price = ofd.Price,
                    Note = ofd.Note
                })
                .ToListAsync();
            if (orderFoodDetails == null || !orderFoodDetails.Any())
                return NotFound();
            return orderFoodDetails;
        }
        [HttpPost]
        public async Task<ActionResult<DTO_OrderFoodDetail>> CreateOrderFoodDetail(DTO_OrderFoodDetail dto)
        {
            var orderFoodDetail = new OrderFoodDetail
            {
                OrderTableId = dto.OrderTableId,
                DishId = dto.DishId,
                Quantity = dto.Quantity,
                Price = dto.Price,
                Note = dto.Note
            };
            _context.OrderFoodDetails.Add(orderFoodDetail);
            await _context.SaveChangesAsync();
            dto.OrderFoodDetailsId = orderFoodDetail.OrderFoodDetailsId;
            return CreatedAtAction(nameof(GetOrderFoodDetailById), new { id = orderFoodDetail.OrderFoodDetailsId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderFoodDetail(int id, DTO_OrderFoodDetail dto)
        {
            if (id != dto.OrderFoodDetailsId)
                return BadRequest("ID mismatch");
            var orderFoodDetail = await _context.OrderFoodDetails.FindAsync(id);
            if (orderFoodDetail == null)
                return NotFound();
            orderFoodDetail.OrderTableId = dto.OrderTableId;
            orderFoodDetail.DishId = dto.DishId;
            orderFoodDetail.Quantity = dto.Quantity;
            orderFoodDetail.Price = dto.Price;
            orderFoodDetail.Note = dto.Note;
            _context.Entry(orderFoodDetail).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
