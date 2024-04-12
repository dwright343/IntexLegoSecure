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

            _session = new InferenceSession("Models/model.onnx");
        }

        // HomeController.cs


        // HomeController.cs
        public IActionResult PrepareOrder()
        {
            // Assuming retrieval of cart details is necessary, simulate here
            Order newOrder = new Order
            {
                CustomerId = 1,
                Time = DateTime.Now.Hour * 100 + DateTime.Now.Minute, // HHMM format
                Amount = 200,
                DayOfWeek = DateTime.Now.DayOfWeek.ToString(),
                EntryMode = "PIN",
                TransactionType = "Online",
                TransactionCountry = "USA",
                ShippingCountry = "USA",
                Bank = "HSBC",
                CardType = "Visa"
            };

            // Forward to PredictFraud to check if the order is fraudulent
            return PredictFraud(newOrder);
        }


        // HomeController.cs
        public IActionResult PredictFraud(Order order)
        {
            // Create a list to hold input data
            var inputData = new List<float>
    {
        order.Time ?? 0, // Ensure these are non-nullable or have fallbacks
        order.Amount ?? 0,
        DateTime.Now.Year, // Assuming you use the current date, adapt as necessary
        DateTime.Now.Month,
        DateTime.Now.Day,
        order.DayOfWeek == "Mon" ? 1 : 0,
        order.DayOfWeek == "Sat" ? 1 : 0,
        order.DayOfWeek == "Sun" ? 1 : 0,
        order.DayOfWeek == "Thu" ? 1 : 0,
        order.DayOfWeek == "Tue" ? 1 : 0,
        order.DayOfWeek == "Wed" ? 1 : 0,
        order.EntryMode == "PIN" ? 1 : 0,
        order.EntryMode == "Tap" ? 1 : 0,
        order.TransactionType == "Online" ? 1 : 0,
        order.TransactionType == "POS" ? 1 : 0,
        order.TransactionCountry == "India" ? 1 : 0,
        order.TransactionCountry == "Russia" ? 1 : 0,
        order.TransactionCountry == "USA" ? 1 : 0,
        order.TransactionCountry == "United Kingdom" ? 1 : 0,
        order.ShippingCountry == "India" ? 1 : 0,
        order.ShippingCountry == "Russia" ? 1 : 0,
        order.ShippingCountry == "USA" ? 1 : 0,
        order.ShippingCountry == "United Kingdom" ? 1 : 0,
        order.Bank == "HSBC" ? 1 : 0,
        order.Bank == "Halifax" ? 1 : 0,
        order.Bank == "Lloyds" ? 1 : 0,
        order.Bank == "Metro" ? 1 : 0,
        order.Bank == "Monzo" ? 1 : 0,
        order.Bank == "RBS" ? 1 : 0,
        order.CardType == "Visa" ? 1 : 0
    };

            // Create a tensor from the list of input data
            var inputTensor = new DenseTensor<float>(inputData.ToArray(), new[] { 1, inputData.Count });

            // Create NamedOnnxValue from tensor
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input", inputTensor)
            };

            // Run the model
            using (var results = _session.Run(inputs))
            {
                var resultTensor = results.First().AsTensor<float>();
                float fraudProbability = resultTensor[0];
                bool isFraud = fraudProbability > 0.5f; // Assuming a threshold, adjust as needed

                if (isFraud)
                {
                    return View("Review");
                }
                else
                {
                    return View("Confirmation");
                }
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
        public IActionResult AdminProducts()
        {
            return View();
        }
    }
}


