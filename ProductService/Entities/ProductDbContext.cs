using Entities.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) :
            base(options)
        {
            
        }

        public DbSet<ProductModel> Products { get; set; }
        public DbSet<Wishlist> Wishlist { get; set; }
        public DbSet<Cart> Carts { get; set; }
    }
}
