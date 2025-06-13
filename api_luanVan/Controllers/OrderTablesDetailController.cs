using api_LuanVan.DataTransferObject;
using api_LuanVan.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_LuanVan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderTablesDetailController : ControllerBase
    {
        private readonly DbluanvantotnghiepContext _context;
        public OrderTablesDetailController(DbluanvantotnghiepContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_OrderTablesDetail>>> GetAllOrderTablesDetails()
        {
            return await _context.OrderTablesDetails
                .Select(otd => new DTO_OrderTablesDetail
                {
                    OrderTablesDetailsId = otd.OrderTablesDetailsId,
                    OrderTableId = otd.OrderTableId,
                    TableId = otd.TableId
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DTO_OrderTablesDetail>> GetOrderTablesDetailById(int id)
        {
            var orderTableDetail = await _context.OrderTablesDetails.FindAsync(id);
            if (orderTableDetail == null)
                return NotFound();
            return new DTO_OrderTablesDetail
            {
                OrderTablesDetailsId = orderTableDetail.OrderTablesDetailsId,
                OrderTableId = orderTableDetail.OrderTableId,
                TableId = orderTableDetail.TableId
            };
        }

        [HttpGet("list/{orderTableId}")]
        public async Task<ActionResult<IEnumerable<DTO_OrderTablesDetail>>> GetOrderTablesDetailsByOrderTableId(long orderTableId)
        {
            var orderTableDetails = await _context.OrderTablesDetails
                .Where(o => o.OrderTableId == orderTableId)
                .Select(o => new DTO_OrderTablesDetail
                {
                    OrderTablesDetailsId = o.OrderTablesDetailsId,
                    OrderTableId = o.OrderTableId,
                    TableId = o.TableId
                })
                .ToListAsync();

            if (orderTableDetails == null || !orderTableDetails.Any())
                return NotFound();

            return orderTableDetails;
        }

        [HttpPost]
        public async Task<ActionResult<DTO_OrderTablesDetail>> CreateOrderTablesDetail([FromBody] DTO_OrderTablesDetail dto)
        {
            var orderTableDetail = new OrderTablesDetail
            {
                OrderTablesDetailsId = dto.OrderTablesDetailsId,
                OrderTableId = dto.OrderTableId,
                TableId = dto.TableId
            };
            _context.OrderTablesDetails.Add(orderTableDetail);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrderTablesDetailById), new { id = orderTableDetail.OrderTablesDetailsId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderTablesDetail(int id, [FromBody] DTO_OrderTablesDetail dto)
        {
            if (id != dto.OrderTablesDetailsId)
                return BadRequest();
            var orderTableDetail = await _context.OrderTablesDetails.FindAsync(id);
            if (orderTableDetail == null)
                return NotFound();
            orderTableDetail.OrderTableId = dto.OrderTableId;
            orderTableDetail.TableId = dto.TableId;
            _context.Entry(orderTableDetail).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}