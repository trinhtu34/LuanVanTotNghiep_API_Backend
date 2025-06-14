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
    }
}
