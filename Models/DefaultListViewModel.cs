using System.Collections.Generic;

namespace IntexLegoSecure.Models.ViewModels
{
    public class DefaultListViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();
        public string? CurrentPrimaryColor { get; set; }
        public List<string> Categories { get; set; } // Add this property
    }
}