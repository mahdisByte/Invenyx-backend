using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementAPI.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        // 🔗 CATEGORY FK
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // 🔗 SUPPLIER FK
        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
    }
}
