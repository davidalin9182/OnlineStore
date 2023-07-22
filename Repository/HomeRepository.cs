using Microsoft.EntityFrameworkCore;
using Proiect_IR.Data;
using Proiect_IR.Interfaces;
using Proiect_IR.Models;

namespace Proiect_IR.Repository
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _context;

        public HomeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Home home)
        {
            _context.Add(home);
            return Save();
        }

        public bool Delete(Home home)
        {
            _context.Remove(home);
            return Save();
        }

        public async Task<IEnumerable<Home>> GetAll()
        {
            return await _context.Home.ToListAsync();
        }
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }

        public bool Update(Home home)
        {
            _context.Update(home);
            return Save();
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Home.CountAsync();
        }

        public Task<Home?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Home?> GetByIdAsyncNoTracking(int id)
        {
            throw new NotImplementedException();
        }

    }
}