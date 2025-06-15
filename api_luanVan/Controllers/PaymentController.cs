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
        private readonly DbluanvantotnghiepContext _context;
        public PaymentController(DbluanvantotnghiepContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_Payment>>> GetAllPaymentResults()
        {
            return await _context.PaymentResults
                .Select(pr => new DTO_Payment
                {
                    PaymentResultId = pr.PaymentResultId,
                    OrderTableId = pr.OrderTableId,
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

        [HttpPost]
        public async Task<ActionResult<DTO_Payment>> CreatePaymentResult(DTO_Payment paymentDto)
        {
            var paymentResult = new PaymentResult
            {
                OrderTableId = paymentDto.OrderTableId,
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
            return CreatedAtAction(nameof(GetPaymentResultsByOrderTableId), new { id = paymentResult.OrderTableId }, paymentDto);
        }
    }
}
