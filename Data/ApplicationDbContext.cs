using IntexLegoSecure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IntexLegoSecure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }



        public virtual DbSet<Customer> Customers { get; set; }

        //public virtual DbSet<LineItem> LineItems { get; set; }

        //public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<Product> Products { get; set; }
    }
}