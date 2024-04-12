using IntexLegoSecure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IntexLegoSecure.Pages
{
    public class CartModel : PageModel
    {
        private I_Repository _repo;
        public Cart Cart { get; set; }
        public CartModel(I_Repository temp, Cart cartService)
        {
            _repo = temp;
            Cart = cartService;
        }

        public string ReturnUrl { get; set; } = "/";
        public void OnGet(string returnUrl)
        {
            ReturnUrl = returnUrl ?? "/";
            //Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
        }

        public IActionResult OnPost(int productId, string returnUrl) 
        {
            Product product = _repo.Products
                .FirstOrDefault(x => x.ProductId == productId);

            if (product != null)
            {
                //Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart(); //if there is already a cart there. If not, we create a new cart
                Cart.AddItem(product, 1); // and then add that item to the cart 
                //HttpContext.Session.SetJson("cart", Cart);
            }

            return RedirectToPage (new {returnUrl = returnUrl });
        }

        public IActionResult OnPostRemove(int productId, string returnUrl)
        {
            Cart.RemoveLine(Cart.Lines.First(x => x.Product.ProductId == productId).Product); 

            return RedirectToPage(new { returnUrl = returnUrl });
        }
    }
}

//using IntexLegoSecure.Infrastructure;
//using IntexLegoSecure.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;

//namespace IntexLegoSecure.Pages
//{
//    public class CartModel : PageModel
//    {
//        private I_Repository _repo;
//        public Cart Cart { get; set; }
//        public CartModel(I_Repository temp, Cart cartService)
//        {
//            _repo = temp;
//            Cart = cartService;
//        }

//        public string ReturnUrl { get; set; } = "/";
//        public void OnGet(string returnUrl)
//        {
//            ReturnUrl = returnUrl ?? "/";
//            //Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
//        }

//        public IActionResult OnPost(int defaultId, string returnUrl)
//        {
//            Default_Model def = _repo.Defaults_table
//                .FirstOrDefault(x => x.DefaultId == defaultId);

//            if (def != null)
//            {
//                //Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart(); //if there is already a cart there. If not, we create a new cart
//                Cart.AddItem(def, 1); // and then add that item to the cart 
//                //HttpContext.Session.SetJson("cart", Cart);
//            }

//            return RedirectToPage(new { returnUrl = returnUrl });
//        }

//        public IActionResult OnPostRemove(int defaultId, string returnUrl)
//        {
//            Cart.RemoveLine(Cart.Lines.First(x => x.Default_Model.DefaultId == defaultId).Default_Model); // this might be wrong

//            return RedirectToPage(new { returnUrl = returnUrl });
//        }
//    }
//}
