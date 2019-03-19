using Flurl.Http;
using Newtonsoft.Json;
using AmazonProductAdvertisingAPI.Models;
using AmazonProductAdvertisingAPI.DataLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace AmazonProductAdvertisingAPI.Import
{

    /*
     * This class shows how to make a simple authenticated call to the
     * Amazon Product Advertising API.
     *
     * See the README.html that came with this sample for instructions on
     * configuring and running the sample.
     */
    public class AmazonSwapSyncer
    {
        public void Sync()
        {
            Console.WriteLine("Sincronizzazione prodotti da Amazon....");
            SynchronizeProductsInfo(GetItemsFromAmazonSwapDB());
            Console.WriteLine("Fine sincronizzazione prodotti da Amazon....");
        }

        private void SynchronizeProductsInfo(IList<SwapItem> swapItems)
        {
            Console.WriteLine("Totale prodotti: " + swapItems.Count);
            int index = 0;
            int errors = 0;
            int globalIndex = 0;
            var storeDbContext = new StoreDbContext();
            foreach (var swapItem in swapItems)
            {
                var currentProduct = storeDbContext.Products.FirstOrDefault(p => p.Ean == swapItem.Ean);
                if (currentProduct != null)
                {
                    try
                    {
                        var amazonItem = JsonConvert.DeserializeObject <AmazonItem> (swapItem.Content);
                        //ottiene la descrizione
                        currentProduct.Description = amazonItem.Item.EditorialReviews.EditorialReview.Content;
                        //ottiene le caratteristiche techiche del prodotto
                        currentProduct.Features = String.Join(" * ", amazonItem.Item.ItemAttributes.Feature);
                        //ottiene la url dell'immagine di grandi dimensioni
                        currentProduct.ImageUrl = amazonItem.Item.LargeImage.Url;
                    }
                    catch (Exception)
                    {
                        errors++;
                    }
                }

                index++;
                globalIndex++;

                if (index == 500)
                {
                    index = 0;
                    storeDbContext.SaveChanges();
                    Console.WriteLine("Prodotti elaborati: " + globalIndex);
                }
            }
            storeDbContext.SaveChanges();

            Console.WriteLine("Prodotti elaborati: " + globalIndex);
            Console.WriteLine("Errori riscontrati: " + errors);
        }

        private IList<SwapItem> GetItemsFromAmazonSwapDB()
        {
            using (var amazonSwapDbContext = new AmazonSwapDbContext())
            {
                return (from si in amazonSwapDbContext.SwapItems select si).ToList();
            }
        }
    }
}

