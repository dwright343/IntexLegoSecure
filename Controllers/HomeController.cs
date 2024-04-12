using IntexLegoSecure.Models.ViewModels;
using IntexLegoSecure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using IntexLegoSecure.ViewModels;


namespace IntexLegoSecure.Controllers
{

    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public IActionResult Index()
        {
            return View();
        }


        private I_Repository _repo;
        public HomeController(I_Repository temp)
        {
            _repo = temp;
        }

        public IActionResult ListProducts(string? category, int pageNum = 1)
        {
            int pageSize = 5;

            // Fetch distinct categories from your products
            var categories = _repo.Products.Select(p => p.Category).Distinct().ToList();

            // Filter products based on the selected category
            IQueryable<Product> productsQuery = _repo.Products.AsQueryable(); // Convert to IQueryable

            if (!string.IsNullOrEmpty(category))
            {
                productsQuery = productsQuery.Where(p => p.Category == category);
            }

            var PageInfo = new DefaultListViewModel
            {
                Categories = categories,
                Products = productsQuery
                    .OrderBy(x => x.ProductId)
                    .Skip((pageNum - 1) * pageSize)
                    .Take(pageSize),

                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize,
                    TotalItems = string.IsNullOrEmpty(category) ? _repo.Products.Count() : productsQuery.Count()
                }
            };

            return View(PageInfo);
        }
        
        public IActionResult DetailedProduct(int id)
        {
            // Retrieve the product from the database based on the provided id
            var product = _repo.Products.FirstOrDefault(p => p.ProductId == id);
    
            if (product == null)
            {
                return NotFound(); // Or handle the case where the product is not found
            }
    
            return View(product); // Pass the product to the DetailedProduct view
        }






        [Authorize(Roles = "Admin")]
        public IActionResult Privacy()
        {
            return View();
        }


        [Authorize]
        public IActionResult Secrets()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        //public IActionResult AdminOrders(int? fraud, int pageNum = 1) // we will want to pass in the fradulent bool here so that we can filter.
        //{
        //    int pageSize = 5;
        //    var PageInfo = new DefaultListViewModel
        //    {
        //        Products = _repo.Products
        //        .Where(x => x.PrimaryColor == primaryColor || primaryColor == null)
        //        .OrderBy(x => x.ProductId)
        //        .Skip((pageNum - 1) * pageSize)
        //        .Take(pageSize),

        //        PaginationInfo = new PaginationInfo
        //        {
        //            CurrentPage = pageNum,
        //            ItemsPerPage = pageSize,
        //            TotalItems = primaryColor == null ? _repo.Products.Count() : _repo.Products.Where(x => x.PrimaryColor == primaryColor).Count()
        //        },

        //        CurrentPrimaryColor = primaryColor
        //    };

        //    return View(PageInfo);
        //}
        public IActionResult AdminProducts()
        {
            return View();
        }
    }
}


