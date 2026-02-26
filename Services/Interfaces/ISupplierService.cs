using InventoryManagementAPI.DTOs.Supplier;

namespace InventoryManagementAPI.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierResponseDto>> GetAllAsync();
        Task<SupplierResponseDto?> GetByIdAsync(int id);
        Task<SupplierResponseDto> CreateAsync(CreateSupplierDto dto);
        Task<bool> UpdateAsync(int id, UpdateSupplierDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
