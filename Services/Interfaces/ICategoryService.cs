using InventoryManagementAPI.Models;

namespace InventoryManagementAPI.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAll();
        Task<Category?> GetById(int id);
        Task<Category> Create(Category category);
        Task<Category?> Update(int id, Category category);
        Task<bool> Delete(int id);
    }
}