using IntexLegoSecure.Models;
using IntexLegoSecure.Models.ViewModels;

namespace IntexLegoSecure.ViewModels
{
    public class FraudRoleViewModel
    {
        public IQueryable<Order> Orders { get; set; }

        public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();


        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
