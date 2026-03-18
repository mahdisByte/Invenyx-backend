using InventoryManagementAPI.Data;
using InventoryManagementAPI.Models;
using InventoryManagementAPI.DTOs.Product;
using InventoryManagementAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementAPI.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    CategoryName = p.Category!.Name,
                    SupplierName = p.Supplier!.Name
                })
                .ToListAsync();
        }

        public async Task<ProductResponseDto?> GetByIdAsync(int id)
        {
            var p = await _context.Products
                .Include(x => x.Category)
                .Include(x => x.Supplier)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null) return null;

            return new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Quantity = p.Quantity,
                CategoryName = p.Category!.Name,
                SupplierName = p.Supplier!.Name
            };
        }

        public async Task<ProductResponseDto?> CreateAsync(CreateProductDto dto)
        {
            // VALIDATION
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            var supplierExists = await _context.Suppliers.AnyAsync(s => s.Id == dto.SupplierId);

            if (!categoryExists || !supplierExists)
                return null;

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity,
                CategoryId = dto.CategoryId,
                SupplierId = dto.SupplierId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(product.Id);
        }

        public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Quantity = dto.Quantity;
            product.CategoryId = dto.CategoryId;
            product.SupplierId = dto.SupplierId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<ProductResponseDto>> SearchAsync(string? search, int? categoryId, string? sort)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            query = sort switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "qty_asc" => query.OrderBy(p => p.Quantity),
                "qty_desc" => query.OrderByDescending(p => p.Quantity),
                _ => query.OrderBy(p => p.Name)
            };

            return await query.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Quantity = p.Quantity,
                CategoryName = p.Category!.Name,
                SupplierName = p.Supplier!.Name
            }).ToListAsync();
        }
    }
}
