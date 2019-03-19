using Flurl.Http;
using Newtonsoft.Json;
using AmazonProductAdvertisingAPI.DataLayer;
using AmazonProductAdvertisingAPI.Models;
using AmazonProductAdvertisingAPI.Utilities;
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
    public class AmazonSwapImportManager
    {

        private String ACCESS_KEY_ID;
        private String SECRET_KEY;
        private String ASSOCIATE_TAG;
        private String ENDPOINT;

        SignedRequestHelper helper;

        public AmazonSwapImportManager()
        {
            try
            {
                // Inizializza parametri Amazon
                ACCESS_KEY_ID = ConfigurationManager.AppSettings["amazon:credentials:accessKeyId"];
                SECRET_KEY = ConfigurationManager.AppSettings["amazon:credentials:secretKey"];
                ASSOCIATE_TAG = ConfigurationManager.AppSettings["amazon:store:associateTag"];
                ENDPOINT = ConfigurationManager.AppSettings["amazon:store:endpoint"];
                helper = new SignedRequestHelper(ACCESS_KEY_ID, SECRET_KEY, ENDPOINT);
            }
            catch (Exception)
            {
                //log here!
                return;
            }
        }

        public void Import()
        {
            var storeDbContext = new StoreDbContext();
            var products = storeDbContext.Products.ToArray();

            if (products.Any())
            {
                Console.WriteLine("Nessun prodotto da aggiornare....");

                return;
            }

            Console.WriteLine("Importazione prodotti da Amazon....");
            ImportProductsInfo(products);
            Console.WriteLine("Fine importazione prodotti da Amazon....");

        }

        private void ImportProductsInfo(IList<Product> products)
        {
            Console.WriteLine("Totale prodotti: " + products.Count);
            int index = 0;
            int errors = 0;
            int notFound = 0;
            int globalIndex = 0;
            var amazonSwapDbContext = new AmazonSwapDbContext();
            foreach (var product in products)
            {
                var currentItem = amazonSwapDbContext.SwapItems.FirstOrDefault(a => a.Ean == product.Ean);
                if (currentItem == null || (currentItem != null && (currentItem.Content == null || currentItem.Content == "error" || currentItem.Content == "none")))
                {
                    String json = GetInfoFromAmazonServices(product.Ean);

                    if (json == "error")
                        errors++;
                    if (json == "none")
                        notFound++;
                    if (currentItem != null)
                    {
                        currentItem.Content = json;
                    }
                    else
                    {
                        amazonSwapDbContext.SwapItems.Add(new SwapItem() { Ean = product.Ean, Content = json });
                    }
                    // sleeps 1 second before next call
                    Thread.Sleep(400);
                }

                index++;
                globalIndex++;


                if (index == 100)
                {
                    index = 0;
                    amazonSwapDbContext.SaveChanges();
                    Console.WriteLine("Prodotti elaborati: " + globalIndex);
                }
            }
            amazonSwapDbContext.SaveChanges();

            Console.WriteLine("Prodotti elaborati: " + globalIndex);
            Console.WriteLine("Prodotti non trovati: " + notFound);
            Console.WriteLine("Errori riscontrati: " + errors);
        }

        private String GetInfoFromAmazonServices(String ean)
        {
            try
            {
                String requestUrl = GetProductUrlRequestByEan(ean);
                String xmlObject = GetXmlObjectFromUrl(requestUrl).Result;
                return GetJsonFromXml(xmlObject);
            }
            catch (Exception e)
            {
                return "error";
            }
        }

        private static String GetJsonFromXml(string xmlObject)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlObject);
            var node = doc.DocumentElement.GetElementsByTagName("Item");
            if (node.Item(0) == null) return "none";
            string json = JsonConvert.SerializeXmlNode(node.Item(0));
            return json;
        }

        private String GetProductUrlRequestByEan(String ean)
        {
            IDictionary<string, string> requestParams = new Dictionary<string, String>();

            requestParams.Add("Service", "AWSECommerceService");
            requestParams.Add("Operation", "ItemSearch");
            requestParams.Add("AWSAccessKeyId", ACCESS_KEY_ID);
            requestParams.Add("AssociateTag", ASSOCIATE_TAG);
            requestParams.Add("Keywords", ean);
            requestParams.Add("IdType", "EAN");
            requestParams.Add("ResponseGroup", "Accessories,AlternateVersions,BrowseNodes,EditorialReview,Images,ItemAttributes,ItemIds,OfferFull,OfferListings,Offers,OfferSummary,PromotionSummary,RelatedItems,Reviews,SalesRank,Similarities");
            requestParams.Add("SearchIndex", "All");
            requestParams.Add("RelationshipType", "AuthorityTitle");

            return helper.Sign(requestParams);
        }

        private async Task<String> GetXmlObjectFromUrl(String requestUrl)
        {
            return await requestUrl.GetStringAsync();
        }
    }
}

