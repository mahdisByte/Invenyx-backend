using InventoryManagementAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HistoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HistoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var logs = await _context.AuditLogs
                .OrderByDescending(x => x.Date)
                .ToListAsync();

            return Ok(logs);
        }

        [HttpGet("product/{id}")]
        public async Task<IActionResult> GetByProduct(int id)
        {
            var logs = await _context.AuditLogs
                .Where(x => x.ProductId == id)
                .OrderByDescending(x => x.Date)
                .ToListAsync();

            return Ok(logs);
        }
    }
}