using IntexLegoSecure.Models.ViewModels;
using IntexLegoSecure.Models;
using IntexLegoSecure.Models.ViewModels;

namespace IntexLegoSecure.Models.ViewModels
{
    public class ProductListViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();
        public string? CurrentPrimaryColor { get; set; }
        public List<string> Categories { get; set; } // Add this property

        public string CurrentFilter { get; set; } // Add this property
    }
}