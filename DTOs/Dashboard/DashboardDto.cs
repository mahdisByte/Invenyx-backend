namespace InventoryManagementAPI.DTOs.Dashboard
{
    public class DashboardDto
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalStockQuantity { get; set; }

        public int TotalStockInTransactions { get; set; }
        public int TotalStockOutTransactions { get; set; }
    }
}