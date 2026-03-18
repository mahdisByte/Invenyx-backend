using InventoryManagementAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuditLogController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuditLogController(AppDbContext context)
        {
            _context = context;
        }

        // 🔥 GET ALL LOGS
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var logs = await _context.AuditLogs
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return Ok(logs);
        }
    }
}