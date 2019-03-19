namespace AmazonProductAdvertisingAPI.DataLayer
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class StoreDbContext : DbContext
    {
        public StoreDbContext()
            : base("name=StoreDbConnectionString")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Product> Products { get; set; }
    }
}
