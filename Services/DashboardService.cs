using InventoryManagementAPI.Data;
using InventoryManagementAPI.DTOs.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementAPI.Services
{
    public class DashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> GetDashboardData()
        {
            var totalProducts = await _context.Products.CountAsync();
            var totalCategories = await _context.Categories.CountAsync();
            var totalSuppliers = await _context.Suppliers.CountAsync();

            var totalStockQuantity = await _context.Products
                .Select(p => (int?)p.Quantity)
                .SumAsync() ?? 0;

            var totalStockIn = await _context.StockIns.CountAsync();
            var totalStockOut = await _context.StockOuts.CountAsync();

            return new DashboardDto
            {
                TotalProducts = totalProducts,
                TotalCategories = totalCategories,
                TotalSuppliers = totalSuppliers,
                TotalStockQuantity = totalStockQuantity,
                TotalStockInTransactions = totalStockIn,
                TotalStockOutTransactions = totalStockOut
            };
        }

    }
}