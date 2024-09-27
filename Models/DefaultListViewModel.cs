using IntexLegoSecure.Models.ViewModels;

namespace IntexLegoSecure.Models.ViewModels
{
    public class DefaultListViewModel
    {
        public IEnumerable<Product> Products { get; set;}
        public PaginationInfo PaginationInfo { get; set;} = new PaginationInfo();
        public string? CurrentPrimaryColor { get; set;}
    }
}