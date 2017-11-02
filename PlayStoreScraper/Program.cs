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

            //  get them from the database as well
            string[] categories = { "store/apps/collection/topselling_free", "store/apps/collection/topselling_paid", "store/apps/collection/topselling_new_free" };

            //string[] fieldNames = {
            //    "Url", "ScrapedDate", "Name", "Developer", "IsTopDeveloper", "DeveloperURL", "PublicationDate", 
            //    "Category", "IsFree", "Price", "CoverImageUrl", "Description", "ReviewScore", "ReviewTotal", 
            //    "FiveStarsReviews", "FourStarsReviews", "ThreeStarsReviews", "TwoStarsReviews", 
            //    "OneStarReviewCount", "AppSize", "Installs", "CurrentVersion", "MinimumOSVersion", "ContentRating", 
            //    "HaveInAppPurchases", "InAppPriceRange", "DeveloperEmail", "DeveloperWebsite", "DeveloperPrivacyPolicy"
            //};

            //string outputFilePath = @"./result";

            PlayStoreScraper.CrawlByCategories(categories, 1000);
            //  This one is working
            //PlayStoreScraper.CrawlByKeywords(keywords, 1000);

            Console.ReadLine();
        }
    }
}
