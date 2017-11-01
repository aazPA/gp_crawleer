using HtmlAgilityPack;
using PlayStoreScraper;
using PlayStoreScraper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace PlayStoreScraper
{
    public class PlayStoreParser
    {
        /// <summary>
        /// Parse App URLS from the Search Result page
        /// </summary>
        /// <param name="response">HTML of the Search Result Page</param>
        /// <returns>Url of apps it finds</returns>
        public IEnumerable<String> ParseAppUrls (string response)
        {
            // Loading Html Document with Play Store content
            HtmlDocument map = new HtmlDocument ();
            map.LoadHtml (response);

            // Checking for nodes
            HtmlNodeCollection nodes = map.DocumentNode.SelectNodes (Consts.APP_URLS);

            if (nodes == null || nodes.Count == 0)
                yield break;

            // Reaching Nodes of Interest
            foreach (var node in nodes)
            {
                // Checking if this node contains the url of an app page
                if ((node.Attributes["href"] != null) && (node.Attributes["href"].Value.Contains ("details?id=")))
                {
                    yield return node.Attributes["href"].Value;
                }
            }
        }

        /// <summary>
        /// Parses ALL data about the app on the App's page
        /// </summary>
        /// <param name="response">HTML Response of the App's landing page</param>
        /// <param name="pageUrl">URL of the App's landing page</param>
        /// <returns>Parsed app data structure</returns>
        public AppModel ParseAppPage (string response, string pageUrl)
        {
            AppModel parsedApp = new AppModel ();

            // Updating App Url
            parsedApp.Url = pageUrl;

            // Updating Reference Date
            parsedApp.ScrapedDate = DateTime.Now;

            // Loading HTML Document
            HtmlDocument map = new HtmlDocument ();
            map.LoadHtml (response);

            // Parsing App Name
            HtmlNode currentNode = map.DocumentNode.SelectSingleNode (Consts.APP_NAME);
            parsedApp.Name       = currentNode == null ? String.Empty : currentNode.InnerText.Trim ();

            // Parsing Cover Img Url
            currentNode           = map.DocumentNode.SelectSingleNode (Consts.APP_COVER_IMG);
            parsedApp.CoverImageUrl = currentNode == null ? String.Empty : currentNode.Attributes["src"].Value;

            // Parsing App Category
            currentNode          = map.DocumentNode.SelectSingleNode (Consts.APP_CATEGORY);

            if (currentNode != null)
            {
                string catLink = currentNode.Attributes["href"].Value;

                if (catLink.IndexOf ('/') >= 0)
                {
                    string[] catLinkSplit = catLink.Split ('/');

                    parsedApp.Category = catLinkSplit.Last ();
                }
            }
            else
            {
                parsedApp.Category = String.Empty;
            }


            var kek = map.DocumentNode;

            // Parsing App Developer/Author
            currentNode         = map.DocumentNode.SelectSingleNode (Consts.APP_DEV);
            parsedApp.Developer = currentNode == null ? String.Empty : currentNode.InnerText.Trim ();

            // Parsing If the Developer is a Top Developer
            currentNode              = map.DocumentNode.SelectSingleNode (Consts.APP_TOP_DEV);
            parsedApp.IsTopDeveloper = currentNode == null ? false : true;

            // Parsing App Developer Url
            currentNode         = map.DocumentNode.SelectSingleNode (Consts.APP_DEV_URL);

            if (currentNode != null && currentNode.Attributes["content"] != null)
            {
                parsedApp.DeveloperURL = Consts.APP_URL_PREFIX + currentNode.Attributes["content"].Value;
            }
            else
            {
                parsedApp.DeveloperURL = String.Empty;
            }

            // Parsing Free x Paid App
            currentNode               = map.DocumentNode.SelectSingleNode (Consts.APP_PRICE);

            if (currentNode.Attributes["content"] != null)
            {
                string contentValue = currentNode.Attributes["content"].Value;
                parsedApp.IsFree    = contentValue.Equals ("0") ? true : false;
            }
            else
            {
                parsedApp.IsFree = true;
            }

            // Parsing App Price
            if (parsedApp.IsFree)
            {
                parsedApp.Price = String.Empty;
            }
            else
            {
                parsedApp.Price = currentNode.Attributes["content"].Value;
            }

            // Parsing App Description
            currentNode           = map.DocumentNode.SelectSingleNode (Consts.APP_DESCRIPTION);
            parsedApp.Description = currentNode == null ? String.Empty : currentNode.InnerText.Trim ();

            // Checking for In app Purchases 
            if (map.DocumentNode.SelectSingleNode (Consts.APP_IAP_MESSAGE) != null)
            {
                parsedApp.HaveInAppPurchases = true;
            }
            else
            {
                parsedApp.HaveInAppPurchases = false;
            }

            // Parsing App's Score
            //Score score = new Score ();

            // Total Score
            currentNode = map.DocumentNode.SelectSingleNode (Consts.APP_SCORE_VALUE);
            parsedApp.ReviewScore = ParseDouble(currentNode, "content");
            
            // Rating Count
            currentNode = map.DocumentNode.SelectSingleNode (Consts.APP_SCORE_COUNT);
            parsedApp.ReviewTotal = (int) ParseDouble(currentNode, "content");

            // Parsing Five  Stars Count
            currentNode = map.DocumentNode.SelectSingleNode (Consts.APP_FIVE_STARS);
            parsedApp.FiveStarsReviews = (int) ParseDouble(currentNode);

            // Parsing Four Stars Count
            currentNode = map.DocumentNode.SelectSingleNode (Consts.APP_FOUR_STARS);
            parsedApp.FourStarsReviews = (int) ParseDouble(currentNode);

            // Parsing Three Stars Count
            currentNode = map.DocumentNode.SelectSingleNode (Consts.APP_THREE_STARS);
            parsedApp.ThreeStarsReviews = (int) ParseDouble(currentNode);

            // Parsing Two Stars Count
            currentNode = map.DocumentNode.SelectSingleNode (Consts.APP_TWO_STARS);
            parsedApp.TwoStarsReviews = (int) ParseDouble(currentNode);

            // Parsing One Stars Count
            currentNode = map.DocumentNode.SelectSingleNode (Consts.APP_ONE_STARS);
            parsedApp.OneStarReviews = (int) ParseDouble(currentNode);

            // Parsing Publishing Date
            currentNode = map.DocumentNode.SelectSingleNode(Consts.APP_PUBLISH_DATE);

            if (currentNode != null)
            {
                parsedApp.PublicationDate = ParseDate(currentNode.InnerText.Replace("-", String.Empty).Trim());
            }

            // Parsing App Size
            currentNode     = map.DocumentNode.SelectSingleNode (Consts.APP_SIZE);

            if (currentNode != null)
            {
                string stringSize = currentNode.InnerText.Trim ();
                Double appSize;

                // Checking if the app size is measured in MBs, Gbs or Kbs
                if (stringSize.EndsWith ("M", StringComparison.InvariantCultureIgnoreCase)) // MegaBytes
                {
                    // TryParse raises no exception. Its safer
                    if (Double.TryParse (stringSize.Replace ("M","").Replace ("m", "") , out appSize))
                    {
                        parsedApp.AppSize = appSize;
                    }
                }
                else if (stringSize.EndsWith ("G", StringComparison.InvariantCultureIgnoreCase)) // Gigabytes
                {
                    // TryParse raises no exception. Its safer
                    if (Double.TryParse(stringSize.Replace ("G", "").Replace ("g", "") , out appSize))
                    {
                        parsedApp.AppSize = appSize * 1024; // Normalizing Gygabites to Megabytes
                    }
                }
                else if (stringSize.EndsWith ("K", StringComparison.InvariantCultureIgnoreCase)) // Kbs
                {
                    // TryParse raises no exception. Its safer
                    if (Double.TryParse (stringSize.Replace ("K", "").Replace ("k", ""), out appSize))
                    {
                        parsedApp.AppSize = appSize / 1024; // Normalizing Kbs to Megabytes
                    }
                }
                else
                {
                    parsedApp.AppSize = -1; // Meaning that "App Size Varies Per App"
                }
            }

            // Parsing App's Current Version
            currentNode              = map.DocumentNode.SelectSingleNode (Consts.APP_VERSION);
            parsedApp.CurrentVersion = currentNode == null ? String.Empty : currentNode.InnerText.Trim ();
            
            // Parsing App's Instalation Count
            currentNode              = map.DocumentNode.SelectSingleNode (Consts.APP_INSTALLS);
            parsedApp.Installs   = currentNode == null ? String.Empty : currentNode.InnerText.Trim ();

            // Parsing App's Content Rating
            currentNode              = map.DocumentNode.SelectSingleNode (Consts.APP_CONTENT_RATING);
            parsedApp.ContentRating  = currentNode == null ? String.Empty : currentNode.InnerText.Trim ();

             // Parsing App's OS Version Required
            currentNode                = map.DocumentNode.SelectSingleNode (Consts.APP_OS_REQUIRED);
            parsedApp.MinimumOSVersion = currentNode == null ? String.Empty : currentNode.InnerText.Trim ();

            // Parsing In-App products price range
            currentNode = map.DocumentNode.SelectSingleNode(Consts.APP_IAP_PRICE);
            parsedApp.InAppPriceRange = currentNode == null ? String.Empty : currentNode.InnerText.Trim();

            // Parsing Developer Links (e-mail / website)
            foreach (var devLink in map.DocumentNode.SelectNodes (Consts.APP_DEV_LINKS))
            {
                // Parsing Inner Text
                string tagText = devLink.InnerText.ToUpper ().Trim ();

                // Checking for Email
                if (tagText.IndexOf ("EMAIL", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    parsedApp.DeveloperEmail   = devLink.Attributes["href"].Value.Replace ("mailto:", String.Empty).Trim();
                }
                else if (tagText.IndexOf ("WEBSITE", StringComparison.InvariantCultureIgnoreCase) >= 0) // Developer Website
                {
                    parsedApp.DeveloperWebsite = ExtractUrlFromGoogleUrl(HttpUtility.HtmlDecode (devLink.Attributes["href"].Value.Trim()));
                }
                else if (tagText.IndexOf("PRIVACY", StringComparison.InvariantCultureIgnoreCase) >= 0) // Privacy Policy
                {
                    parsedApp.DeveloperPrivacyPolicy = ExtractUrlFromGoogleUrl(HttpUtility.HtmlDecode (devLink.Attributes["href"].Value.Trim()));
                }
            }

            return parsedApp;
        }

        private static string ExtractUrlFromGoogleUrl(string url)
        {
            Match match = Regex.Match(url, @"http[s]*://www\.google\.com/url\?q=(http[s]*://.+)&sa=.*");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Parses the page of an app for the url of another apps,
        /// it gathers both "Related Apps" and "More from Developer" apps
        /// </summary>
        /// <param name="response">HTML responde of the app's landing page</param>
        /// <returns>Url of app pages it finds</returns>
        public IEnumerable<String> ParseExtraApps (string response)
        {
            HtmlDocument map = new HtmlDocument ();
            map.LoadHtml (response);

            foreach (HtmlNode extraAppNode in map.DocumentNode.SelectNodes (Consts.EXTRA_APPS))
            {
                if (extraAppNode.Attributes["href"] != null)
                {
                    yield return extraAppNode.Attributes["href"].Value;
                }
            }
        }

        /// <summary>
        /// Safely parses a double value out of an HtmlNode attribute
        /// </summary>
        /// <param name="node">HtmlNode of interest</param>
        /// <param name="attrName">Name of the attribute that contains the double value</param>
        /// <returns>Parsed value (if any) or 0.0 in case of error</returns>
        private double ParseDouble (HtmlNode node, string attrName)
        {
            if (node != null && node.Attributes[attrName] != null)
            {
                double parsedScore;

                if (Double.TryParse (node.Attributes[attrName].Value, out parsedScore))
                {
                    return parsedScore;
                }
            }

            return 0.0;
        }
        
        /// <summary>
        /// Safely parses a double value out of an HtmlNode inner text
        /// </summary>
        /// <param name="node">HtmlNode of interest</param>
        /// <returns>Parsed value (if any) or 0.0 in case of error</returns>
        private double ParseDouble (HtmlNode node)
        {
            double parsedValue;

            // Removing Dots from value to make it easier to parse its correct value
            string normalizedInnerText = node.InnerText.Replace (".", String.Empty);

            if (Double.TryParse (normalizedInnerText, out parsedValue))
            {
                return parsedValue;
            }

            return 0.0;
        }

        /// <summary>
        /// Parses and builds a date out of a string following Google Play Store
        /// format
        /// </summary>
        /// <param name="dateString">String of date to be parsed</param>
        /// <returns>Assembled DateTime instance</returns>
        private DateTime ParseDate (string dateString)
        {
            string[] datePieces = dateString.Replace(",", String.Empty).Split (' ');

            // Backing values up
            string month = datePieces[0];
            string day   = datePieces[1];
            string year  = datePieces[2];

            // Normalizing Day if needed
            if (day.Length < 2)
                day = "0" + day;

            dateString = String.Join (" ",year, month, day);

            return DateTime.ParseExact (dateString, Consts.DATE_FORMAT, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
