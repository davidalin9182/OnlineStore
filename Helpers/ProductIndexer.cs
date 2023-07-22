using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Proiect_IR.Data;
using Proiect_IR.Interfaces;
using Proiect_IR.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Proiect_IR.Helpers
{
    public class ProductIndexer
    {
        public readonly Lucene.Net.Store.Directory _directory;
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;


        public ProductIndexer(IWebHostEnvironment env, IProductRepository productRepository, ApplicationDbContext context)
        {
            var indexPath = Path.Combine(env.WebRootPath, "index_directory");
            _directory = new SimpleFSDirectory(new DirectoryInfo(indexPath));
            _productRepository = productRepository;
            _context = context;

            // create the index if it doesn't exist
            if (!IndexReader.IndexExists(_directory))
            {
                CreateIndex();
            }
        }

        public void CreateIndex()
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            var indexWriter = new IndexWriter(_directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);

            // get all products from the repository
            var products = _productRepository.GetAll().Result;


            // add each product to the index
            foreach (var product in products)
            {
                var doc = new Document();
                doc.Add(new NumericField("Id", Field.Store.YES, true).SetIntValue(product.Id));
                doc.Add(new Field("ProductName", product.ProductName, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("ProductDescription", product.ProductDescription, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("ProductCategory", product.ProductCategory, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("Sauces", product.Sauces, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new NumericField("Calories", Field.Store.YES, true).SetIntValue(product.Calories ?? 0));
                doc.Add(new NumericField("Fat", Field.Store.YES, true).SetIntValue(product.Fat ?? 0));
                doc.Add(new NumericField("Protein", Field.Store.YES, true).SetIntValue(product.Protein ?? 0));

                // add specification to the document
                // concatenate multiple fields of ProductSpecifications object into a string
                var specs = $"{product.Sauces}|{product.Calories}|{product.Fat}|{product.Protein}";

                // add specification to the document
                doc.Add(new Field("ProductSpecification", specs, Field.Store.YES, Field.Index.ANALYZED));


                indexWriter.AddDocument(doc);
            }


            // optimize the index and dispose the writer
            indexWriter.Optimize();
            indexWriter.Dispose();
        }


        public IEnumerable<Product> Search(string searchTerm, bool sortAscending = true)
        {
            using (var searcher = new IndexSearcher(_directory, true))
            {
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

                var fields = new[] { "ProductName", "Sauces", "Calories", "Fat", "Protein" };
                var queries = new List<Query>();
                foreach (var field in fields)
                {
                    var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, field, analyzer);
                    var query = parser.Parse(searchTerm);
                    queries.Add(query);
                }
                var booleanQuery = new BooleanQuery();
                foreach (var query in queries)
                {
                    booleanQuery.Add(query, Occur.SHOULD);
                }

                // sort by relevance score
                var sort = new Sort(new SortField(null, SortField.SCORE, !sortAscending));
                var hits = searcher.Search(booleanQuery, null, 1000, sort).ScoreDocs;

                var productIds = hits.Select(hit => int.Parse(searcher.Doc(hit.Doc).Get("Id")));
                var products = _productRepository.GetByIdsAsync(productIds).Result;

                // add relevance score to each product
                for (int i = 0; i < products.Count(); i++)
                {
                    products.ElementAt(i).RelevanceScore = hits.ElementAt(i).Score;
                }

                return products;
            }
        }


        public void DeleteFromIndex(int productId)
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

            // create a writer with the same analyzer used for indexing
            using (var indexWriter = new IndexWriter(_directory, analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // create a query to find the document based on the productId field
                var query = NumericRangeQuery.NewIntRange("Id", productId, productId, true, true);

                // delete the document from the index based on the query
                indexWriter.DeleteDocuments(query);

                // commit the changes and dispose the writer
                indexWriter.Commit();
                indexWriter.Dispose();
            }
        }

    }

}

