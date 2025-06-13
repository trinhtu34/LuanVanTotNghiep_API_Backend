using api_LuanVan.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

    }
}
