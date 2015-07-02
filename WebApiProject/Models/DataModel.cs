namespace WebApiProject.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Tables;
    using Views;

    public partial class DataModel : DbContext
    {
        public DataModel()
            : base("name=DataModel")
        {
        }

        // tables
        public virtual DbSet<Accounts> Accounts { get; set; }
        public virtual DbSet<Genders> Genders { get; set; }
        public virtual DbSet<Goods> Goods { get; set; }
        public virtual DbSet<Purchases> Purchases { get; set; }
        public virtual DbSet<Shoppers> Shoppers { get; set; }

        // stored procedures
        public virtual DbSet<getShoppers> getShoppers { get; set; }
        public virtual DbSet<getPurchases> getPurchases { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accounts>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Genders>()
                .Property(e => e.Text)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Goods>()
                .Property(e => e.Price)
                .HasPrecision(10, 4);
        }
    }
}
