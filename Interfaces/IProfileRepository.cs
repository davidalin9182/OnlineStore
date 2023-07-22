using Proiect_IR.Models;

namespace Proiect_IR.Interfaces
{
    public interface IProfileRepository
    {
        Task<List<Product>> GetAllUserProducts();
        Task<AppUser> GetUserById(string id);
        //public AppUser GetUserById(string id);
        Task<AppUser> GetByIdNoTracking(string id);
        bool Update(AppUser user);
        public Product GetById(int id);
        bool Save();


    }
}
