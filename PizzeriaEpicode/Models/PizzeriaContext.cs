using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace PizzeriaEpicode.Models
{
    public partial class PizzeriaContext : DbContext
    {
        public PizzeriaContext()
            : base("name=PizzeriaContext")
        {
        }

        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.PriceAtTheTimeOfOrder)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Order>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Product>()
                .Property(e => e.Ingredients)
                .IsUnicode(false);
        }
    }
}
