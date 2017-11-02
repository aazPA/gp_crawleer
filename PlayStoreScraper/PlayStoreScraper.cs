using System;
using System.Linq;
using System.Web.Util;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Security.Permissions;
using System.Security;
using System.ComponentModel;
using System.Reflection;
using PlayStoreScraper.Exporters;
using PlayStoreScraper.Models;
using WebUtilsLib;
using PlayStoreScraper.ProxyHandler;

namespace PlayStoreScraper
{
    class PlayStoreScraper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Response Parser
        private static PlayStoreParser parser = new PlayStoreParser();

        /// <summary>
        /// Crawl Play Store based on keywords given and export the result to the DB
        /// </summary>
        /// <param name="keywords">Array of keywords</param>
        /// <param name="downloadDelay">Download Delay in milliseconds</param>
        /// <param name="writeCallback">Callback method for writing the App Data</param>
        public static void CrawlByKeywords(string[] keywords, int downloadDelay = 0,
            Action<AppModel> writeCallback = null)
        {
            // Collect App Urls from keywords
            foreach (string keyword in keywords)
            {
                List<AppShortDescription> fullParsedList = CollectAppsShortInformationFromKeywords(keyword);

                // Apply download delay
                if (downloadDelay > 0)
                {
                    Thread.Sleep(downloadDelay);
                }

                Console.WriteLine(fullParsedList);
            }
        }

        /// <summary>
        /// Crawl Play Store based on categories given and export the result to DB
        /// </summary>
        /// <param name="categories">Array of categories</param>
        /// <param name="downloadDelay">Download Delay in milliseconds</param>
        /// <param name="writeCallback">Callback method for writing the App Data</param>
        public static void CrawlByCategories(string[] categories, int downloadDelay = 0,
            Action<AppModel> writeCallback = null)
        {
            // Collect App Urls from keywords
            foreach (string category in categories)
            {
                List<AppShortDescription> fullParsedList = CollectAppsShortInformationFromCategories(category);

                // Apply download delay
                if (downloadDelay > 0)
                {
                    Thread.Sleep(downloadDelay);
                }

                Console.WriteLine(fullParsedList);
            }
        }

        public static List<AppShortDescription> CollectAppsShortInformationFromCategories(string category)
        {
            List<AppShortDescription> parsedApps_list = new List<AppShortDescription>();


            log.Info("Crawling Category : [ " + category + " ]");

            int numberOfCyclesCompleted = 0;
            while (numberOfCyclesCompleted < Consts.CATEGORY_NUMBER_OF_CYCLES)
            {
                string crawlUrl = String.Format(Consts.CRAWL_URL_CATEGORY, category, "Russia");
                string postData = String.Format(Consts.POST_DATA_CATEGORY, Consts.CATEGORY_NUMBER_OF_APPS_PER_CYCLE * numberOfCyclesCompleted);
                numberOfCyclesCompleted++;
                //Console.WriteLine(postDataTest);


                // HTML Response
                string response = string.Empty;

                // Executing Web Requests
                using (WebRequests server = new WebRequests())
                {
                    // Creating Request Object
                    server.Host = Consts.HOST;

                    //  this is how we actually connect to all this shit
                    //  the only thing left - we need to randomize it and check if 200
                    //WebProxy proxyObject = new WebProxy("http://" + ProxyLoader.ReturnRandomProxy(), true);
                    //server.Proxy = proxyObject;

                    int insertedAppCount = 0;
                    int skippedAppCount = 0;
                    int errorsCount = 0;

                    // Executing Request
                    response = server.Post(crawlUrl, postData);

                    // Checking Server Status
                    if (server.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        log.Error("Http Error - Status Code: " + server.StatusCode);

                        errorsCount++;

                        if (errorsCount > Consts.MAX_REQUEST_ERRORS)
                        {
                            log.Info("Crawl Stopped: MAX_REQUEST_ERRORS reached");
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }


                    //var kek1 = parser.ParseAppUrls(response);


                    // Parsing Links out of Html Page
                    foreach (AppShortDescription asd in parser.ParseAppUrls(response))
                    {
                        if (!parsedApps_list.Contains(asd))
                        {
                            parsedApps_list.Add(asd);

                            log.Info("Inserted App: " + asd);

                            ++insertedAppCount;
                        }
                        else
                        {
                            ++skippedAppCount;
                            log.Info("Duplicated App. Skipped: " + asd);
                        }
                    }

                    exit:
                    log.Info("Inserted App Count: " + insertedAppCount);
                    log.Info("Skipped App Count: " + skippedAppCount);
                    log.Info("Error Count: " + errorsCount + "\n");
                }
            }

            return parsedApps_list;
        }


            public static List<AppShortDescription> CollectAppsShortInformationFromKeywords(string keyword)
        {
            List<AppShortDescription> parsedApps_list = new List<AppShortDescription> ();


            log.Info("Crawling Search Term : [ " + keyword + " ]");

            string crawlUrl = String.Format(Consts.CRAWL_URL_KEYWORD_INITIAL, keyword, "Russia", "ru");

            string postData = Consts.POST_DATA_KEYWORD_INITAL;

            // HTML Response
            string response = string.Empty;

            // Executing Web Requests
            using (WebRequests server = new WebRequests())
            {
                // Creating Request Object
                server.Host = Consts.HOST;

                //  this is how we actually connect to all this shit
                //  the only thing left - we need to randomize it and check if 200
                //WebProxy proxyObject = new WebProxy("http://" + ProxyLoader.ReturnRandomProxy(), true);
                //server.Proxy = proxyObject;

                int insertedAppCount = 0;
                int skippedAppCount = 0;
                int errorsCount = 0;

                do
                {
                    // Executing Request
                    response = server.Post(crawlUrl, postData);

                    // Checking Server Status
                    if (server.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        log.Error("Http Error - Status Code: " + server.StatusCode);

                        errorsCount++;

                        if (errorsCount > Consts.MAX_REQUEST_ERRORS)
                        {
                            log.Info("Crawl Stopped: MAX_REQUEST_ERRORS reached");
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }


                    //var kek1 = parser.ParseAppUrls(response);


                    // Parsing Links out of Html Page
                    foreach (AppShortDescription asd in parser.ParseAppUrls(response))
                    {
                        if (!parsedApps_list.Contains(asd))
                        {
                            parsedApps_list.Add(asd);

                            log.Info("Inserted App: " + asd);

                            ++insertedAppCount;

                            //if (maxAppUrls > 0 && insertedAppCount >= maxAppUrls)
                            //{
                            //    goto exit;
                            //}
                        }
                        else
                        {
                            ++skippedAppCount;
                            log.Info("Duplicated App. Skipped: " + asd);
                        }
                    }

                    // Get pagTok value that will be used to fetch next stream data.
                    // If not found, that means we have reached the end of stream.
                    ClusterAndToken cat_cl = getPageAndClusterTokens(response);
                    if (cat_cl == null)
                    {
                        break;
                    }
                    else
                    {
                        crawlUrl = Consts.CRAWL_URL_KEYWORD_CLUSTER;
                        postData = String.Format(Consts.POST_DATA_KEYWORD_CLUSTER, cat_cl.clp, cat_cl.pagTok);
                    }
                    Console.WriteLine("Inserted apps: " + insertedAppCount + ".");

                } while (true);

            exit:
                log.Info("Inserted App Count: " + insertedAppCount);
                log.Info("Skipped App Count: " + skippedAppCount);
                log.Info("Error Count: " + errorsCount + "\n");
            }

            return parsedApps_list;
        }

        
        
        /// <summary>
        /// Get Page Token and Cluster Token for play store streaming search result.
        /// </summary>
        /// <param name="response">Response body</param>
        /// <returns>Page Token and Cluster Token</returns>
        protected static ClusterAndToken getPageAndClusterTokens(string response)
        {

            ClusterAndToken cat_cl = null;

            string pagTok = string.Empty;
            string clpTok = string.Empty;

            //  I NEED A BETTER REGEX THAT WOULD ONLY TAKE THESE GUYS. FOR NOW IT TAKES ALL DIFFERENT KIND OF SHIT, SO I GOTTA SPLIT IT ._.
            Regex pagTokenRegex = new Regex(@"-p6B+.+\:S\:.{11}", RegexOptions.Compiled);
            Regex clpTokenRegex = new Regex(@"ggE+.+\:S\:.{11}", RegexOptions.Compiled);

            Match pagTokenMatch = pagTokenRegex.Match(response);
            Match clpTokenMatch = clpTokenRegex.Match(response);

            if (pagTokenMatch.Success && clpTokenMatch.Success)
            {
                cat_cl = new ClusterAndToken();

                string dirtyPagToken = pagTokenMatch.Value.Replace("\\\\u003d", "=");
                string [] splitDirtyPagToken = dirtyPagToken.Split(new string[] { "\\x22" }, StringSplitOptions.None);
                string cleanPagToken = splitDirtyPagToken[0];
                cat_cl.pagTok = cleanPagToken;


                string dirtyClusterToken = clpTokenMatch.Value;
                string[] splitDirtyClusterToken = dirtyClusterToken.Split(new string[] { "\">" }, StringSplitOptions.None);
                string cleanClusterToken = splitDirtyClusterToken[0];
                cat_cl.clp = cleanClusterToken;


                return cat_cl;
            }
            else
            {
                return cat_cl;
            }
        }
    }
}
