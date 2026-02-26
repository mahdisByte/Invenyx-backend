using InventoryManagementAPI.DTOs.Stock;
using InventoryManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StockController : ControllerBase
    {
        private readonly StockService _service;

        public StockController(StockService service)
        {
            _service = service;
        }

        [HttpPost("stock-in")]
        public async Task<IActionResult> StockIn(CreateStockDto dto)
        {
            var result = await _service.StockIn(dto);

            if (!result) return NotFound("Product not found");

            return Ok("Stock Added");
        }

        [HttpPost("stock-out")]
        public async Task<IActionResult> StockOut(CreateStockDto dto)
        {
            var result = await _service.StockOut(dto);

            if (result != "Success") return BadRequest(result);

            return Ok("Stock Removed");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStocks()
        {
            var stocks = await _service.GetAllStocks();
            return Ok(stocks);
        }
    }
}
