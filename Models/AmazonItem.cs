using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonProductAdvertisingAPI.Models
{
    public class AmazonItem
    {
        public Item Item { get; set; }      
    }

    public class Item
    {
        public String ASIN { get; set; }
        public long SalesRank { get; set; }
        public ItemAttributes ItemAttributes { get; set; }
        public EditorialReviews EditorialReviews { get; set; }
        public AmazonImage LargeImage { get; set; }
    }

    public class ItemAttributes
    {
        public String Binding { get; set; }
        public String Color { get; set; }
        public String[] Feature { get; set; }
    }

    public class EditorialReviews
    {
        public EditorialReview EditorialReview { get; set; }
    }

    public class EditorialReview
    {
        public String Content { get; set; }
    }

    public class AmazonImage
    {
        public String Url { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
