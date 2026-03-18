using InventoryManagementAPI.Data;
using InventoryManagementAPI.Models;

namespace InventoryManagementAPI.Services
{
    public class AuditService
    {
        private readonly AppDbContext _context;

        public AuditService(AppDbContext context)
        {
            _context = context;
        }

        public async Task Log(string user, string action, int? productId, string? productName, int? qty)
        {
            var log = new AuditLog
            {
                UserName = user,
                Action = action,
                ProductId = productId,
                ProductName = productName,
                Quantity = qty
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}