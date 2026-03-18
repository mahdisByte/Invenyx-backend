using InventoryManagementAPI.Data;
using InventoryManagementAPI.DTOs.Stock;
using InventoryManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventoryManagementAPI.Services
{
    public class StockService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public StockService(AppDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }

        //  GET CURRENT USER FROM JWT
        private string GetCurrentUser()
        {
            return _httpContext.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
        }

        public async Task<bool> StockIn(CreateStockDto dto)
        {
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null) return false;

            using var transaction = await _context.Database.BeginTransactionAsync();

            var stockIn = new StockIn
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };

            product.Quantity += dto.Quantity;

            // 🔥 AUDIT LOG
            var audit = new AuditLog
            {
                UserName = GetCurrentUser(),
                Action = "Stock In",
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = dto.Quantity,
                Date = DateTime.UtcNow
            };

            _context.StockIns.Add(stockIn);
            _context.AuditLogs.Add(audit);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }

        public async Task<string> StockOut(CreateStockDto dto)
        {
            var product = await _context.Products.FindAsync(dto.ProductId);

            if (product == null) return "Product not found";
            if (product.Quantity < dto.Quantity)
                return "Insufficient stock";

            using var transaction = await _context.Database.BeginTransactionAsync();

            var stockOut = new StockOut
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };

            product.Quantity -= dto.Quantity;

            // 🔥 AUDIT LOG
            var audit = new AuditLog
            {
                UserName = GetCurrentUser(),
                Action = "Stock Out",
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = dto.Quantity,
                Date = DateTime.UtcNow
            };

            _context.StockOuts.Add(stockOut);
            _context.AuditLogs.Add(audit);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return "Success";
        }

        public async Task<List<StockResponseDto>> GetAllStocks()
        {
            return await _context.Products
                .Select(p => new StockResponseDto
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    Quantity = p.Quantity
                })
                .ToListAsync();
        }
    }
}