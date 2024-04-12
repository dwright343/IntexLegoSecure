using IntexLegoSecure.Models.ViewModels;
using IntexLegoSecure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using IntexLegoSecure.ViewModels;
using IntexLegoSecure.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Drawing.Text;
using Elfie.Serialization;


namespace IntexLegoSecure.Controllers
{

    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
        private readonly InferenceSession _session;

        public IActionResult Index()
        {
            // Retrieve products with specific IDs (e.g., 21, 19, 3) from the database
            var productIds = new List<int> { 21, 19, 3 };
            var products = _repo.Products.Where(p => productIds.Contains(p.ProductId)).ToList();

            // Pass the products to the view
            return View(products);
        }

        private I_Repository _repo;
        public HomeController(I_Repository temp)
        {
            _repo = temp;

            _session = new InferenceSession("C:\\Users\\dayis\\source\\repos\\IntexLegoSecure\\model.onnx");
        }



        public IActionResult PredictFraud(Order newOrder)
        {

            //var record = new Order()
            //{
            //     = _repo.Orders
            //    .Where(x => x.customer_ID == 500)
            //};
            //newOrder = _repo.Orders
            //    .Where(x => x. == 500);


            //float day = newOrder.Date.Day;
            DateTime date = DateTime.Today;

            var input = new List<float>
            {
                (float)newOrder.Time,
                (float)newOrder.Amount,                

                //(float)date,

                newOrder.DayOfWeek == "Mon" ? 1 : 0,
                newOrder.DayOfWeek == "Sat" ? 1 : 0,
                newOrder.DayOfWeek == "Sun" ? 1 : 0,
                newOrder.DayOfWeek == "Thu" ? 1 : 0,
                newOrder.DayOfWeek == "Tue" ? 1 : 0,
                newOrder.DayOfWeek == "Wed" ? 1 : 0,

                newOrder.EntryMode == "PIN" ? 1 : 0,
                newOrder.EntryMode == "Tap" ? 1 : 0,

                newOrder.TransactionType == "Online" ? 1 : 0,
                newOrder.TransactionType == "POS" ? 1 : 0,
                newOrder.TransactionType == "Online" ? 1 : 0,

                newOrder.TransactionCountry == "India" ? 1 : 0,
                newOrder.TransactionCountry == "Russia" ? 1 : 0,
                newOrder.TransactionCountry == "USA" ? 1 : 0,
                newOrder.TransactionCountry == "UnitedKingdom" ? 1 : 0,

                newOrder.ShippingCountry == "India" ? 1 : 0,
                newOrder.ShippingCountry == "Russia" ? 1 : 0,
                newOrder.ShippingCountry == "USA" ? 1 : 0,
                newOrder.ShippingCountry == "UnitedKingdom" ? 1 : 0,

                newOrder.Bank == "HSBC" ? 1 : 0,
                newOrder.Bank == "Halifax" ? 1 : 0,
                newOrder.Bank == "Lloyds" ? 1 : 0,
                newOrder.Bank == "Metro" ? 1 : 0,
                newOrder.Bank == "Monzo" ? 1 : 0,
                newOrder.Bank == "RBS" ? 1 : 0,

                newOrder.CardType == "Visa" ? 1 : 0
            };

            var inputTensor = new DenseTensor<float>(input.ToArray(), new[] { 1, input.Count });

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", inputTensor)
            };

            var Fraud = new int();

            using (var results = _session.Run(inputs))
            {
                var prediction = results.FirstOrDefault(item => item.Name == "output_label")?.AsTensor<long>().ToArray();
                Fraud = (int)prediction[0];
            }

            if (Fraud == 0)
            {

                return View("Confirmation");
            }
            else
            {
                return View("Review");
            }
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
        public IActionResult AdminProducts()
        {
            return View();
        }
    }
}


