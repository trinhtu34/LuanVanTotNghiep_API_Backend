using api_LuanVan.DataTransferObject;
using api_LuanVan.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_LuanVan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderTableController : ControllerBase
    {
        private readonly DbluanvantotnghiepContext _context;
        public OrderTableController(DbluanvantotnghiepContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_OrderTable>>> GetAllOrderTables()
        {
            return await _context.OrderTables
                .Select(m => new DTO_OrderTable
                {
                    OrderTableId = m.OrderTableId,
                    UserId = m.UserId,
                    StartingTime = m.StartingTime,
                    isCancel = m.isCancel,
                    TotalPrice = m.TotalPrice,
                    OrderDate = m.OrderDate
                }).ToListAsync();
        }
        [HttpGet("{userid}")]
        public async Task<ActionResult<DTO_OrderTable>> GetOrderTableByUserID(string userid)
        {
            var orderTable = await _context.OrderTables
                .Where(m => m.UserId == userid)
                .Select(m => new DTO_OrderTable
                {
                    OrderTableId = m.OrderTableId,
                    //UserId = m.UserId,
                    StartingTime = m.StartingTime,
                    isCancel = m.isCancel,
                    TotalPrice = m.TotalPrice,
                    OrderDate = m.OrderDate
                }).ToListAsync();
            if (orderTable == null || orderTable.Count == 0)
                return NotFound();
            return Ok(orderTable);
        }

        [HttpPost]
        public async Task<ActionResult<DTO_OrderTable>> CreateOrderTable([FromBody] DTO_OrderTable dto)
        {
            var orderTable = new OrderTable
            {
                UserId = dto.UserId,
                StartingTime = dto.StartingTime,
                isCancel = dto.isCancel,
                TotalPrice = dto.TotalPrice,
                OrderDate = DateTime.Now
            };
            _context.OrderTables.Add(orderTable);
            await _context.SaveChangesAsync();
            dto.OrderTableId = orderTable.OrderTableId;
            return CreatedAtAction(nameof(GetOrderTableByUserID), new { userid = dto.UserId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DTO_OrderTable>> UpdateOrderTable(long id, [FromBody] DTO_OrderTable dto)
        {
            if (id != dto.OrderTableId)
                return BadRequest();

            var orderTable = await _context.OrderTables.FindAsync(id);
            if (orderTable == null)
                return NotFound();

            //orderTable.UserId = dto.UserId;
            orderTable.StartingTime = dto.StartingTime;
            orderTable.isCancel = dto.isCancel;
            orderTable.TotalPrice = dto.TotalPrice;
            _context.Entry(orderTable).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
