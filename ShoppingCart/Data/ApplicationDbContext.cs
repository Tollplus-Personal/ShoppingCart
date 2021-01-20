using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ShoppingCart.Models;

namespace ShoppingCart.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        //public DbSet<ShoppingCart.Models.Product> Product { get; set; }
        public DbSet<ShoppingCart.Models.Order> Order { get; set; }
        public DbSet<ShoppingCart.Models.OrderDetails> orderDetails { get; set; }
        public DbSet<ShoppingCart.Models.Cart> Cart { get; set; }
    }
}
