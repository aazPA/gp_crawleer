using PlayStoreScraper.Exporters;
using PlayStoreScraper.ProxyHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayStoreScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            //  Initialize all of the possible proxies
            ProxyLoader.InitializeProxies();

            //  These keywords should be given to me. Each keyword has a different proxy
            //  Before running a keyword, you should check if proxy is working (anything other than 200 means that we should try another one)
            //  get these keywords from the database
            string[] keywords = { "Russia" };

            string[] categories = { "store/apps/collection/topselling_free" };

            //string[] fieldNames = {
            //    "Url", "ScrapedDate", "Name", "Developer", "IsTopDeveloper", "DeveloperURL", "PublicationDate", 
            //    "Category", "IsFree", "Price", "CoverImageUrl", "Description", "ReviewScore", "ReviewTotal", 
            //    "FiveStarsReviews", "FourStarsReviews", "ThreeStarsReviews", "TwoStarsReviews", 
            //    "OneStarReviewCount", "AppSize", "Installs", "CurrentVersion", "MinimumOSVersion", "ContentRating", 
            //    "HaveInAppPurchases", "InAppPriceRange", "DeveloperEmail", "DeveloperWebsite", "DeveloperPrivacyPolicy"
            //};

            //string outputFilePath = @"./result";

            IExporter exporter = null;

            // Export to CSV file.
            // NOTE: You can also implement your own Exporter class.
            // Please see IExporter and CSVExporter class for example.
            //exporter = new CSVExporter(outputFilePath + ".csv", fieldNames);

            // Start crawling for the keywords with maximum 30 Apps result and download delay 1 second.
            PlayStoreScraper.CrawlByKeywords(keywords, 1000);

            Console.ReadLine();
        }
    }
}
