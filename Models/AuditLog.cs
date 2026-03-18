using System.ComponentModel.DataAnnotations;

namespace InventoryManagementAPI.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Action { get; set; } = string.Empty;

        public int? ProductId { get; set; }
        public string? ProductName { get; set; }

        public int? Quantity { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}