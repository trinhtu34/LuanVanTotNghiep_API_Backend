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
        private readonly Dbluanvan2Context _context;
        public OrderTableController(Dbluanvan2Context context)
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
                    IsCancel = m.IsCancel,
                    TotalPrice = m.TotalPrice,
                    TotalDeposit = m.TotalDeposit,
                    OrderDate = m.OrderDate
                }).ToListAsync();
        }

        [HttpGet("afterCurrentStartingTime")]
        public async Task<ActionResult<IEnumerable<DTO_OrderTable>>> GetOrderTableAfterCurrentStartingTime()
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            var orderTables = await _context.OrderTables
                .Where(m => m.StartingTime > currentTime && !m.IsCancel)
                .Select(m => new DTO_OrderTable
                {
                    OrderTableId = m.OrderTableId,
                    UserId = m.UserId,
                    StartingTime = m.StartingTime,
                    IsCancel = m.IsCancel,
                    TotalPrice = m.TotalPrice,
                    TotalDeposit = m.TotalDeposit,
                    OrderDate = m.OrderDate
                }).ToListAsync();
            if (orderTables == null || orderTables.Count == 0)
                return NotFound();
            return Ok(orderTables);
        }

        [HttpGet("{userid}")]
        public async Task<ActionResult<DTO_OrderTable>> GetOrderTableByUserID(string userid)
        {
            var orderTable = await _context.OrderTables
                .Where(m => m.UserId == userid)
                .Select(m => new DTO_OrderTable
                {
                    OrderTableId = m.OrderTableId,
                    UserId = m.UserId,
                    StartingTime = m.StartingTime,
                    IsCancel = m.IsCancel,
                    TotalPrice = m.TotalPrice,
                    TotalDeposit = m.TotalDeposit,
                    OrderDate = m.OrderDate
                }).ToListAsync();
            if (orderTable == null || orderTable.Count == 0)
                return NotFound();
            return Ok(orderTable);
        }

        [HttpPost]
        public async Task<ActionResult<DTO_OrderTable>> CreateOrderTable([FromBody] DTO_OrderTable dto)
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var orderTable = new OrderTable
            {
                UserId = dto.UserId,
                StartingTime = dto.StartingTime,
                IsCancel = dto.IsCancel,
                TotalPrice = dto.TotalPrice ?? 0,
                TotalDeposit = dto.TotalDeposit ?? 0,
                OrderDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone)
            };
            _context.OrderTables.Add(orderTable);
            await _context.SaveChangesAsync();
            dto.OrderTableId = orderTable.OrderTableId;
            dto.OrderDate = orderTable.OrderDate;
            dto.TotalPrice = orderTable.TotalPrice;
            dto.TotalDeposit = orderTable.TotalDeposit;
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
            orderTable.IsCancel = dto.IsCancel;
            orderTable.TotalPrice = dto.TotalPrice;
            orderTable.TotalDeposit = dto.TotalDeposit;
            _context.Entry(orderTable).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("state/{id}")]
        public async Task<ActionResult<DTO_OrderTable>> UpdateOrderTableByState(long id, [FromBody] DTO_OrderTable dto)
        {
            if (id != dto.OrderTableId)
                return BadRequest();

            var orderTable = await _context.OrderTables.FindAsync(id);
            if (orderTable == null)
                return NotFound();

            //orderTable.UserId = dto.UserId;
            orderTable.IsCancel = dto.IsCancel;
            _context.Entry(orderTable).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
