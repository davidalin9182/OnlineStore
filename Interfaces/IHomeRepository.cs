
using Proiect_IR.Models;
using Proiect_IR.Data;

namespace Proiect_IR.Interfaces
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Home>> GetAll();

        Task<Home?> GetByIdAsync(int id);

        Task<Home?> GetByIdAsyncNoTracking(int id);


        Task<int> GetCountAsync();

        bool Add(Home home);

        bool Update(Home home);

        bool Delete(Home home);

        bool Save();
    }
}