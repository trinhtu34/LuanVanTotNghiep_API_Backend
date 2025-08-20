using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace api_LuanVan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadyController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public ReadyController(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // 1. Check DB connection
            var dbConnStr = _config.GetConnectionString("DefaultConnection");
            try
            {
                using (var conn = new MySqlConnection(dbConnStr))
                {
                    await conn.OpenAsync();
                }
            }
            catch
            {
                return StatusCode(503, "Database not ready");
            }

            // 2. Check Redis / Cache (nếu có)
            // try { ... } catch { return StatusCode(503, "Cache not ready"); }

            // 3. Check service bên ngoài (nếu cần)
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync("https://third-party-service/health");
                if (!response.IsSuccessStatusCode)
                    return StatusCode(503, "Dependency service not ready");
            }
            catch
            {
                return StatusCode(503, "Dependency service error");
            }

            // 4. Nếu tất cả OK
            return Ok("READY");
        }
    }
}
