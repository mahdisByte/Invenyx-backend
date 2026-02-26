using InventoryManagementAPI.Data;
using InventoryManagementAPI.DTOs.Stock;
using InventoryManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementAPI.Services
{
    public class StockService
    {
        private readonly AppDbContext _context;

        public StockService(AppDbContext context)
        {
            _context = context;
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

            _context.StockIns.Add(stockIn);

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

            _context.StockOuts.Add(stockOut);

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
