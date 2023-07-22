using Microsoft.EntityFrameworkCore;
using Proiect_IR.Data;
using Proiect_IR.Interfaces;
using Proiect_IR.Models;



namespace Proiect_IR.Repository
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProfileRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<Product>> GetAllUserProducts()
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId();
            var userProducts = _context.Products.Where(r => r.AppUser.Id == currentUserId);
            return userProducts.ToList();
        }

        public async Task<AppUser> GetUserById(string id)
        {
            return await _context.Users.FindAsync(id);
        }
        //public AppUser GetUserById(string id)
        //{
        //    var user = _context.Users.Find(id);
        //    return user ?? null;
        //}

        public async Task<AppUser> GetByIdNoTracking(string id)
        {
            return await _context.Users.Where(u => u.Id == id).AsNoTracking().FirstOrDefaultAsync();
            
        }
        //To list like with post i can use at friends list to return more than 1 user this is for only 1 user
        //ID: d00b16db-6a00-4c54-b6f3-b958d717a9de
        public bool Update(AppUser user)
        {
            _context.Users.Update(user);
            return Save();
        }
        public Product GetById(int id)
        {
            return _context.Products.FirstOrDefault(p => p.Id == id);
        }
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

    }
}
