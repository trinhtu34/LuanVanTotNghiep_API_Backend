using api_LuanVan.DataTransferObject;
using api_LuanVan.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace api_LuanVan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly Dbluanvan2Context _context;
        public PaymentController(Dbluanvan2Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_Payment>>> GetAllPaymentResults()
        {
            return await _context.PaymentResults
                .Select(pr => new DTO_Payment
                {
                    OrderTableId = pr.OrderTableId,
                    CartId = pr.CartId,
                    PaymentId = pr.PaymentId,
                    Amount = pr.Amount,
                    IsSuccess = pr.IsSuccess,
                    Description = pr.Description,
                    Timestamp = pr.Timestamp,
                    VnpayTransactionId = pr.VnpayTransactionId,
                    PaymentMethod = pr.PaymentMethod,
                    BankCode = pr.BankCode,
                    BankTransactionId = pr.BankTransactionId,
                    ResponseDescription = pr.ResponseDescription,
                    TransactionStatusDescription = pr.TransactionStatusDescription
                }).ToListAsync();
        }
        [HttpGet("paymenthistory/ordertable")]
        public async Task<ActionResult<IEnumerable<DTO_Payment_ShowHistoryPayment_OrderTable>>> GetPaymentHistoryOrderTable()
        {
            var paymentResults = await _context.PaymentResults
                .Where(pr => pr.OrderTableId != null)
                .Select(pr => new DTO_Payment_ShowHistoryPayment_OrderTable
                {
                    PaymentResultId = pr.PaymentResultId,
                    OrderTableId = pr.OrderTableId,
                    Amount = pr.Amount,
                    IsSuccess = pr.IsSuccess,
                    Timestamp = pr.Timestamp,
                    PaymentMethod = pr.PaymentMethod,
                    BankCode = pr.BankCode,
                    ResponseDescription = pr.ResponseDescription,
                    TransactionStatusDescription = pr.TransactionStatusDescription
                }).ToListAsync();
            if (paymentResults == null || !paymentResults.Any())
                return NotFound();
            return paymentResults;
        }
        [HttpGet("paymenthistory/cart")]
        public async Task<ActionResult<IEnumerable<DTO_Payment_ShowHistoryPayment_Cart>>> GetPaymentHistoryCart()
        {
            var paymentResults = await _context.PaymentResults
                .Where(pr => pr.CartId != null)
                .Select(pr => new DTO_Payment_ShowHistoryPayment_Cart
                {
                    PaymentResultId = pr.PaymentResultId,
                    CartId = pr.CartId,
                    Amount = pr.Amount,
                    IsSuccess = pr.IsSuccess,
                    Timestamp = pr.Timestamp,
                    PaymentMethod = pr.PaymentMethod,
                    BankCode = pr.BankCode,
                    ResponseDescription = pr.ResponseDescription,
                    TransactionStatusDescription = pr.TransactionStatusDescription
                }).ToListAsync();
            if (paymentResults == null || !paymentResults.Any())
                return NotFound();
            return paymentResults;
        }

        [HttpGet("ordertable/{id}")]
        public async Task<ActionResult<IEnumerable<DTO_Payment>>> GetPaymentResultsByOrderTableId(long id)
        {
            var paymentResults = await _context.PaymentResults
                .Where(pr => pr.OrderTableId == id)
                .Select(pr => new DTO_Payment
                {
                    PaymentResultId = pr.PaymentResultId,
                    OrderTableId = pr.OrderTableId,
                    CartId = pr.CartId,
                    Amount = pr.Amount,
                    PaymentId = pr.PaymentId,
                    IsSuccess = pr.IsSuccess,
                    Description = pr.Description,
                    Timestamp = pr.Timestamp,
                    VnpayTransactionId = pr.VnpayTransactionId,
                    PaymentMethod = pr.PaymentMethod,
                    BankCode = pr.BankCode,
                    BankTransactionId = pr.BankTransactionId,
                    ResponseDescription = pr.ResponseDescription,
                    TransactionStatusDescription = pr.TransactionStatusDescription
                }).ToListAsync();
            if (paymentResults == null || !paymentResults.Any())
                return NotFound();

            return paymentResults;
        }
        [HttpGet("ordertable/juststatus/{id}")]
        public async Task<ActionResult<bool>> GetPaymentJustStatusByOrderTableId(long id)
        {
            var isPaid = await _context.PaymentResults
                .AnyAsync(p => p.OrderTableId == id && p.IsSuccess == true);

            return Ok(isPaid); // true nếu đã thanh toán, false nếu chưa
        }


        [HttpGet("ordertable/status/{id}")]
        public async Task<ActionResult<IEnumerable<DTO_PaymentStatusOrderTable>>> GetPaymentStatusByOrderTableId(long id)
        {
            var paymentStatuses = await _context.PaymentResults
                .Where(pr => pr.OrderTableId == id)
                .Select(pr => new DTO_PaymentStatusOrderTable
                {
                    OrderTableId = pr.OrderTableId,
                    IsSuccess = pr.IsSuccess
                }).ToListAsync();

            if (paymentStatuses == null || !paymentStatuses.Any())
                return NotFound();

            return paymentStatuses;
        }
        [HttpGet("cart/status/{id}")]
        public async Task<ActionResult<IEnumerable<DTO_PaymentStatusCart>>> GetPaymentStatusByCartId(long id)
        {
            var paymentStatuses = await _context.PaymentResults
                .Where(pr => pr.CartId == id)
                .Select(pr => new DTO_PaymentStatusCart
                {
                    CartId = pr.CartId,
                    IsSuccess = pr.IsSuccess
                }).ToListAsync();

            if (paymentStatuses == null || !paymentStatuses.Any())
                return NotFound();

            return paymentStatuses;
        }
        [HttpPost]
        public async Task<ActionResult<DTO_Payment>> CreatePaymentResult(DTO_Payment paymentDto)
        {
            var paymentResult = new PaymentResult
            {
                OrderTableId = paymentDto.OrderTableId,
                CartId = paymentDto.CartId,
                Amount = paymentDto.Amount,
                PaymentId = paymentDto.PaymentId,
                IsSuccess = paymentDto.IsSuccess,
                Description = paymentDto.Description,
                Timestamp = paymentDto.Timestamp,
                VnpayTransactionId = paymentDto.VnpayTransactionId,
                PaymentMethod = paymentDto.PaymentMethod,
                BankCode = paymentDto.BankCode,
                BankTransactionId = paymentDto.BankTransactionId,
                ResponseDescription = paymentDto.ResponseDescription,
                TransactionStatusDescription = paymentDto.TransactionStatusDescription
            };
            _context.PaymentResults.Add(paymentResult);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("admin/ordertable/{ordertableid}")]
        public async Task<ActionResult<DTO_Payment_OrderTable>> CreatePaymentResultForOrderTable(long ordertableid, DTO_Payment_OrderTable paymentDto)
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            var paymentid = DateTime.Now.Ticks;
            var orderTable = await _context.OrderTables.FindAsync(ordertableid);
            if (orderTable == null)
            {
                return NotFound("Order table not found.");
            }
            var paymentResult = new PaymentResult
            {
                OrderTableId = ordertableid,
                Amount = paymentDto.Amount,
                PaymentId = paymentid,
                IsSuccess = true,
                Description = $"Thanh toán tiền mặt cho đơn đặt bàn : {ordertableid}",
                Timestamp = currentTime,
                PaymentMethod = "Cash"
            };
            _context.PaymentResults.Add(paymentResult);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("admin/cart/{cartid}")]
        public async Task<ActionResult<DTO_Payment_Cart>> CreatePaymentResultForCart(long cartid, DTO_Payment_Cart paymentDto)
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            var paymentid = DateTime.Now.Ticks;
            var cart = await _context.Carts.FindAsync(cartid);
            if (cart == null)
            {
                return NotFound("Cart not found.");
            }
            var paymentResult = new PaymentResult
            {
                CartId = cartid,
                Amount = paymentDto.Amount,
                PaymentId = paymentid,
                IsSuccess = true,
                Description = $"Thanh toán tiền mặt cho đơn hàng : {cartid}",
                Timestamp = currentTime,
                PaymentMethod = "Cash"
            };
            _context.PaymentResults.Add(paymentResult);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
