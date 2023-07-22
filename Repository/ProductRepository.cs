using Microsoft.EntityFrameworkCore;
using Proiect_IR.Interfaces;
using Proiect_IR.Data;
using Proiect_IR.Models;
using Proiect_IR.ViewModels;
using Microsoft.Data.SqlClient;
using Dapper;
using Proiect_IR.Helpers;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Proiect_IR.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;
       


        public ProductRepository(ApplicationDbContext context, string connectionString)
        {
            _context = context;
            _connectionString = connectionString;
         
        }

        public bool Add(Product Posts)
        {
            _context.Add(Posts);
            return Save();
        }

        public bool Delete(Product Posts)
        {
            _context.Remove(Posts);
            return Save();
        }
        public async Task<IEnumerable<AppUser>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _context.Products.ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetByCategory(string category)
        {
            return await _context.Products
                .Where(p => string.IsNullOrEmpty(category) || p.ProductCategory == category)
                .ToListAsync();
        }

        public async Task<AppUser> GetUserById(string id)
        {
            return await _context.Users.FindAsync(id);
        }


        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }

        public bool Update(Product Posts)
        {
            _context.Update(Posts);
            return Save();
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Product> GetByIdAsyncNoTracking(int id)
        {
            return await _context.Products.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }
        public async Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return await _context.Products.Where(p => ids.Contains(p.Id)).ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetByProductName(string productName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Products WHERE ProductName LIKE '%' + @ProductName + '%'";
                var parameters = new { ProductName = productName };
                var products = await connection.QueryAsync<Product>(query, parameters);
                return products;
            }
        }


        public async Task<IEnumerable<Product>> GetProductsByIds(IEnumerable<int> ids)
        {
            var products = await _context.Products
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();

            return products;
        }


        //public async Task<IEnumerable<Product>> SearchByNameAndSpecificationAsync(string searchTerm)
        //{
        //    return await _context.Products
        //        .Where(p => p.ProductName.Contains(searchTerm)
        //                    || p.Sauces.Contains(searchTerm)
        //                    || p.Calories.Contains(searchTerm)
        //                    || p.Fat.Contains(searchTerm)
        //                    || p.Protein.Contains(searchTerm))
        //        .ToListAsync();
        //}


        //public async Task<IEnumerable<Product>> SearchByNameAndSpecificationAsync(string searchTerm, string sortOrder)
        //{
        //    using (var searcher = new IndexSearcher(_productIndexer._directory, true))
        //    {
        //        var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
        //        var query = new TermQuery(new Term("ProductName", searchTerm));

        //        var hits = searcher.Search(query, null, 1000).ScoreDocs;

        //        var products = _context.Products
        //            .Where(p => p.ProductName.Contains(searchTerm)
        //                || p.Sauces.Contains(searchTerm)
        //                || p.Calories.ToString().Contains(searchTerm)
        //                || p.Fat.ToString().Contains(searchTerm)
        //                || p.Protein.ToString().Contains(searchTerm));

        //        switch (sortOrder)
        //        {
        //            case "asc":
        //                products = products.OrderBy(p => _productIndexer.GetAccuracyScore(p, query));
        //                break;
        //            case "desc":
        //                products = products.OrderByDescending(p => _productIndexer.GetAccuracyScore(p, query));
        //                break;
        //            default:
        //                break;
        //        }

        //        return await products.ToListAsync();
        //    }
        //}
    }
}




//public async Task<IEnumerable<Product>> SearchByNameAndSpecificationAsync(string searchTerm, string sortOrder)
//{
//    var query = _context.Products
//        .Where(p => p.ProductName.Contains(searchTerm)
//            || p.Sauces.Contains(searchTerm)
//            || p.Calories.ToString().Contains(searchTerm)
//            || p.Fat.ToString().Contains(searchTerm)
//            || p.Protein.ToString().Contains(searchTerm));

//    switch (sortOrder)
//    {
//        case "asc":
//            query = query.OrderBy(p => _productIndexer.GetAccuracyScore(p, searchTerm));
//            break;
//        case "desc":
//            query = query.OrderByDescending(p => _productIndexer.GetAccuracyScore(p, searchTerm));
//            break;
//        default:
//            break;
//    }

//    return await query.ToListAsync();
//}

//public async Task<IEnumerable<Product>> SearchByNameAndSpecificationAsync(string searchTerm, string sortOrder)
//{
//    var query = _context.Products
//        .Where(p => p.ProductName.Contains(searchTerm)
//            || p.Sauces.Contains(searchTerm)
//            || p.Calories.ToString().Contains(searchTerm)
//            || p.Fat.ToString().Contains(searchTerm)
//            || p.Protein
//}