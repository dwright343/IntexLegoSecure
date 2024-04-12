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

            try
            {
                _session = new InferenceSession("C:\\Users\\dayis\\source\\repos\\IntexLegoSecure\\model.onnx");
            }
            catch (Exception ex) 
            {

            }
        }

        public IActionResult PredictFraud()
        {
            var record = new Order customerOrder();
            
            var fraud_dict = new Dictionary<int, string>
            {
                {0, "Not fraud" },
                {1, "Fraud" }
            };

            var input = new List<float>
            {
                (float)record.Time,
                (float)record.Amount,

                record.DayOfWeek == "Mon" ? 1 : 0,
                record.DayOfWeek == "Sat" ? 1 : 0,
                record.DayOfWeek == "Sun" ? 1 : 0,
                record.DayOfWeek == "Thu" ? 1 : 0,
                record.DayOfWeek == "Tue" ? 1 : 0,
                record.DayOfWeek == "Wed" ? 1 : 0,

                record.EntryMode == "PIN" ? 1 : 0,
                record.EntryMode == "Tap" ? 1 : 0,

                record.TransactionType == "Online" ? 1 : 0,
                record.TransactionType == "POS" ? 1 : 0,
                record.TransactionType == "Online" ? 1 : 0,

                record.TransactionCountry == "India" ? 1 : 0,
                record.TransactionCountry == "Russia" ? 1 : 0,
                record.TransactionCountry == "USA" ? 1 : 0,
                record.TransactionCountry == "UnitedKingdom" ? 1 : 0,

                record.ShippingCountry == "India" ? 1 : 0,
                record.ShippingCountry == "Russia" ? 1 : 0,
                record.ShippingCountry == "USA" ? 1 : 0,
                record.ShippingCountry == "UnitedKingdom" ? 1 : 0,

                record.Bank == "HSBC" ? 1 : 0,
                record.Bank == "Halifax" ? 1 : 0,
                record.Bank == "Lloyds" ? 1 : 0,
                record.Bank == "Metro" ? 1 : 0,
                record.Bank == "Monzo" ? 1 : 0,
                record.Bank == "RBS" ? 1 : 0,

                record.CardType == "Visa" ? 1 : 0
            };

            var inputTensor = new DenseTensor<float>(input.ToArray(), new[] { 1, input.Count });

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", inputTensor)
            };


            
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


