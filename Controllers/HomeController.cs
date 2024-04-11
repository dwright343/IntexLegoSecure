using IntexLegoSecure.Models.ViewModels;
using IntexLegoSecure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

   
        public IActionResult Products()
        {
            return View();
        }

        private I_Repository _repo;
        public HomeController(I_Repository temp)
        {
            _repo = temp;
        }


        public IActionResult ListProducts(string? primaryColor, int pageNum = 1) // name this pageNum, because "page" means something to the .NET environment
        {
            int pageSize = 5;
            var PageInfo = new DefaultListViewModel
            {
                Products = _repo.Products
                .Where(x => x.PrimaryColor == primaryColor || primaryColor == null)
                .OrderBy(x => x.ProductId)
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize),

                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize,
                    TotalItems = primaryColor == null ? _repo.Products.Count() : _repo.Products.Where(x => x.PrimaryColor == primaryColor).Count()
                },

                CurrentPrimaryColor = primaryColor
            };

            return View(PageInfo);
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


