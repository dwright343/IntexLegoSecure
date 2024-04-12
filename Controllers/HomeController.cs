using IntexLegoSecure.Models.ViewModels;
using IntexLegoSecure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using IntexLegoSecure.ViewModels;
using IntexLegoSecure.Models.ViewModels;


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
            // Retrieve products with specific IDs (e.g., 21, 19, 3) from the database
            var productIds = new List<int> { 27, 33, 3 };
            var products = _repo.Products.Where(p => productIds.Contains(p.ProductId)).ToList();

            // Pass the products to the view
            return View(products);
        }




        private I_Repository _repo;
        public HomeController(I_Repository temp)
        {
            _repo = temp;
        }

        public IActionResult ListProducts(string filter, int pageNum = 1)
        {
            int pageSize = 5;
            var productsQuery = _repo.Products.AsQueryable();

            // Fetch distinct categories from your products

            // Filter products based on the selected category

            if (!string.IsNullOrEmpty(filter))
            {
                productsQuery = productsQuery.Where(p => p.Category.Contains(filter));
            }

            var PageInfo = new ProductListViewModel
            {
                Products = productsQuery
                            .OrderBy(x => x.ProductId)
                            .Skip((pageNum - 1) * pageSize)
                            .Take(pageSize)
                            .ToList(),
                
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize,
                    TotalItems = _repo.Products.Count(p => string.IsNullOrEmpty(filter) || p.Category.Contains(filter))
                },
                CurrentFilter = filter
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

        [Authorize(Roles = "Admin")]

        public IActionResult OrderReview(int pageNum = 1, int pageSize = 1000) // name this pageNum, because "page" means something to the .NET environment
        {
            var PageInfo = new FraudRoleViewModel
            {
                Orders = _repo.Orders

                .OrderBy(x => x.TransactionId)
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize),

                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize,
                    TotalItems = _repo.Orders.Count()
                },
            };

            return View(PageInfo);
        }

        [Authorize]
        public IActionResult OrderConfirmation()
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
        //    var PageInfo = new ProductListViewModel
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

    }
}


