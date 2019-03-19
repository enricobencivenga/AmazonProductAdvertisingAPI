namespace AmazonProductAdvertisingAPI.DataLayer
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AmazonSwapDbContext : DbContext
    {
        public AmazonSwapDbContext()
            : base("name=AmazonSwapDbConnectionString")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<SwapItem> SwapItems { get; set; }
    }
}
