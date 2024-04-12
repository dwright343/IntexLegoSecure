
using IntexLegoSecure.Data;

namespace IntexLegoSecure.Models
{
    public class EF_Repository : I_Repository
    {
        private ApplicationDbContext _defaultContext;
        public EF_Repository(ApplicationDbContext temp)
        {
            _defaultContext = temp;
        }

        public IEnumerable<Product> Products => _defaultContext.Products;
        public IQueryable<Order> Orders => _defaultContext.Orders;
    }
}