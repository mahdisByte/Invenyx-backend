namespace InventoryManagementAPI.DTOs.Stock
{
    public class StockResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}