using Microsoft.AspNetCore.Mvc;
using Proiect_IR.Data;
using Proiect_IR.Models;
using Proiect_IR.Interfaces;
using Proiect_IR.ViewModels;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Proiect_IR.Helpers;
using System.IO;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;

namespace Proiect_IR.Controllers
{
    public class ProductController : Controller
    {

        private readonly IProductRepository _productRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ProductIndexer _productIndexer;
        private readonly Lucene.Net.Store.Directory _directory;

        public ProductController(IProductRepository productRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor, ProductIndexer productIndexer, IWebHostEnvironment env)
        {

            _productRepository = productRepository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
            _productIndexer = productIndexer;
            var indexPath = Path.Combine(env.WebRootPath, "index_directory");
            _directory = new SimpleFSDirectory(new DirectoryInfo(indexPath));
        }
     
        public async Task<IActionResult> Index(string category = "", string searchTerm = "")
        {
            IEnumerable<Product> products;

            if (string.IsNullOrEmpty(category))
            {
                if (string.IsNullOrEmpty(searchTerm))
                {
                    products = await _productRepository.GetAll();
                }
                else
                {
                    products = await _productRepository.GetByProductName(searchTerm);
                }
            }
            else
            {
                products = await _productRepository.GetByCategory(category);
            }

            ViewBag.Category = category;
            ViewBag.SearchTerm = searchTerm;
            return View(products);
        }

        [HttpPost]
        public IActionResult Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return RedirectToAction("Index");
            }

            var products = _productRepository.GetByProductName(searchTerm).Result;

            return View("Index", products);

        }
        [HttpGet]
        public IActionResult TypeAhead(string term)
        {
            using (var searcher = new IndexSearcher(_directory, true))
            {
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                var parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, new[] { "ProductName" }, analyzer);
                parser.DefaultOperator = QueryParser.Operator.OR;

                var query = parser.Parse(term + "*");
                var hits = searcher.Search(query, 5).ScoreDocs;

                var suggestions = hits.Select(hit => searcher.Doc(hit.Doc).Get("ProductName")).ToList();

                return Json(suggestions);
            }

        }

        public ActionResult SpecificationSearch()
        {
            return View();
        }

        public ActionResult SpecificationSearchResults(string searchTerm, string sortOrder)
        {
            var products = _productIndexer.Search(searchTerm);

            if (sortOrder == "asc")
            {
                products = products.OrderBy(p => p.RelevanceScore);
            }
            else if (sortOrder == "desc")
            {
                products = products.OrderByDescending(p => p.RelevanceScore);
            }

            return View(products);
        }

       






        public async Task<IActionResult> Create()
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _productRepository.GetUserById(currentUserId);
            var CreateProductViewModel = new CreateProductViewModel 
            { 
                AppUserId = currentUserId ,
                ProfileImageUrl = user.ProfileImageUrl, 
                UserName = user.UserName,
            };
            return View(CreateProductViewModel);

        }

       
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductViewModel ProductVM)
        {
            if (ModelState.IsValid)
            {
                if (ProductVM.ProductImage != null)
                {
                    var result = await _photoService.AddPhotoAsync(ProductVM.ProductImage);
                    var product = new Product
                    {
                        ProfileImageUrl = ProductVM.ProfileImageUrl,
                        UserName = ProductVM.UserName,
                        ProductName = ProductVM.ProductName,
                        ProductDescription = ProductVM.ProductDescription,
                        ProductPrice = ProductVM.ProductPrice,
                        ProductCategory = ProductVM.ProductCategory,
                        ProductImage = result.Url.ToString(),
                        AppUserId = ProductVM.AppUserId,
                    };
                    _productRepository.Add(product);

                    // Add the new product to the index
                    AddToIndex(product);

                    return RedirectToAction("Index", "Product");
                }
            }
            else
            {
                ModelState.AddModelError("", "Photo Upload Failed!");
            }
            return View(ProductVM);
        }

        public void AddToIndex(Product product)
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                var doc = new Document();
                doc.Add(new NumericField("Id", Field.Store.YES, true).SetIntValue(product.Id));
                doc.Add(new Field("ProductName", product.ProductName, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("ProductDescription", product.ProductDescription, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("ProductCategory", product.ProductCategory, Field.Store.YES, Field.Index.ANALYZED));
                writer.AddDocument(doc);
                writer.Optimize();
            }
        }



        public async Task<IActionResult> Edit(int id)
        {
            var Product = await _productRepository.GetByIdAsync(id);
            if (Product == null) return View("Error");
            var ProductVM = new EditProductViewModel
            {
                ProfileImageUrl = Product.ProfileImageUrl,
                UserName = Product.UserName,
                ProductName = Product.ProductName,
                ProductDescription = Product.ProductDescription,
                ProductPrice = Product.ProductPrice,
                ProductCategory = Product.ProductCategory,
                AppUserId = Product.AppUserId,
                URL = Product.ProductImage,
                Sauces = Product.Sauces,
                Calories = Product.Calories,
                Fat = Product.Fat,
                Protein = Product.Protein,
            };
            return View(ProductVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditProductViewModel ProductVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit product");
                return View("Edit", ProductVM);
            }

            var userProducts = await _productRepository.GetByIdAsyncNoTracking(id);

            if (userProducts == null)
            {
                return View("Error");
            }
            if (ProductVM.ProductImage != null)
            {
                var photoResult = await _photoService.AddPhotoAsync(ProductVM.ProductImage);

                if (photoResult.Error != null)
                {
                    ModelState.AddModelError("Image", "Photo upload failed");
                    return View(ProductVM);
                }
                var Products = new Product
                {
                    Id = id,
                    AppUserId = ProductVM.AppUserId,
                    ProfileImageUrl = ProductVM.ProfileImageUrl,
                    UserName = ProductVM.UserName,
                    ProductName = ProductVM.ProductName,
                    ProductDescription = ProductVM.ProductDescription,
                    ProductPrice = ProductVM.ProductPrice,
                    ProductCategory = ProductVM.ProductCategory,
                    ProductImage = photoResult.Url.ToString(),
                    Sauces = ProductVM.Sauces,
                    Calories = ProductVM.Calories,
                    Fat = ProductVM.Fat,
                    Protein = ProductVM.Protein,
                };
                _productRepository.Update(Products);
            }
           


            if (!string.IsNullOrEmpty(userProducts.ProductImage))
            {
                _ = _photoService.DeletePhotoAsync(userProducts.ProductImage);
            }


            return RedirectToAction("Index", "Product");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var Detail = await _productRepository.GetByIdAsync(id);
            if (Detail == null) return View("Error");
            return View(Detail);
        }


        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var productDetails = await _productRepository.GetByIdAsync(id);

            if (productDetails == null)
            {
                return View("Error");
            }

            // Use the _productIndexer instance to delete the product from the index
            _productIndexer.DeleteFromIndex(id);

            _productRepository.Delete(productDetails);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(int id)
        {
            Product Products = await _productRepository.GetByIdAsync(id);
            return Products == null ? NotFound() : View(Products);

        }

        


    }
}