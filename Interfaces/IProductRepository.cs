
using Proiect_IR.Models;
using Proiect_IR.Data;
using Proiect_IR.ViewModels;

namespace Proiect_IR.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAll();

        Task<Product?> GetByIdAsync(int id);

        Task<Product?> GetByIdAsyncNoTracking(int id);
        Task<AppUser> GetUserById(string id);

        Task<int> GetCountAsync();
        Task<IEnumerable<AppUser>> GetAllUsers();
        Task<IEnumerable<Product>> GetByCategory(string category);
        Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<int> ids);
        Task<IEnumerable<Product>> GetByProductName(string productName);
        //Task<IEnumerable<Product>> SearchByNameAndSpecificationAsync(string searchTerm, string sortOrder);
        //Task<IEnumerable<Product>> SearchByNameAndSpecificationAsync(string searchTerm);
        Task<IEnumerable<Product>> GetProductsByIds(IEnumerable<int> ids);
        bool Add(Product Posts);

        bool Update(Product Posts);

        bool Delete(Product Posts);

        bool Save();
    }
}