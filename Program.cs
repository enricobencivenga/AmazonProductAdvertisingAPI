using System;
using AmazonProductAdvertisingAPI.Import;

namespace AmazonProductAdvertisingAPI
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Importazione prodotti da Amazon...");
            var amazonSwapImportManager = new AmazonSwapImportManager();
            Console.WriteLine("Sincronizzazione prodotti da Amazon...");
            var amazonSwapSyncer = new AmazonSwapSyncer();

            Console.ReadKey();
        }
    }
}
