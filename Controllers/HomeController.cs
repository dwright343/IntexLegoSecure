using IntexLegoSecure.Models.ViewModels;
using IntexLegoSecure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using IntexLegoSecure.ViewModels;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Security.Cryptography.Xml;


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
            return View();
        }


        private I_Repository _repo;
        public HomeController(I_Repository temp)
        {
            _repo = temp;

            _session = new InferenceSession("C:\\Users\\dayis\\source\\repos\\IntexLegoSecure\\model.onnx");                     
        }

        public IActionResult PredictFraud(Order newOrder)
        {            
            var fraud_dict = new Dictionary<int, string>
            {
                {0, "Not fraud" },
                {1, "Fraud" }
            };

            float day = newOrder.Date.Day;


            var input = new List<float>
            {
                (float)newOrder.Time,
                (float)newOrder.Amount,

                day,

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
                return View();
            }
            else
            {
                return View();
            }
            
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


