using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonProductAdvertisingAPI.DataLayer
{
    public class Product
    {
        public int Id { get; set; }
        public string Ean { get; set; }
        public string Description { get; set; }
        public string Features { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
    }
}