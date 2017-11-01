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

    // Parse each of App Urls found
    //ParseAppUrls(urls, downloadDelay, exporter, writeCallback);
    public class AppCrawler
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Response Parser
        private static PlayStoreParser parser = new PlayStoreParser();

        private static void ParseAppUrls(ISet<string> urls, int downloadDelay = 0, IExporter exporter = null,
            Action<AppModel> writeCallback = null)
        {
            log.Info("Parsing App URLs...");

            int parsedAppCount = 0;

            // Retry Counter (Used for exponential wait increasing logic)
            int retryCounter = 0;

            // Creating Instance of Web Requests Server
            WebRequests server = new WebRequests();

            foreach (string url in urls)
            {
                try
                {
                    // Building APP URL
                    string appUrl = Consts.APP_URL_PREFIX + url;

                    // Configuring server and Issuing Request
                    server.Headers.Add(Consts.ACCEPT_LANGUAGE);
                    server.Host = Consts.HOST;
                    server.Encoding = "utf-8";
                    server.EncodingDetection = WebRequests.CharsetDetection.DefaultCharset;

                    //  this is how we actually connect to all this shit
                    //  the only thing left - we need to randomize it and check if 200
                    //WebProxy proxyObject = new WebProxy("http://" + ProxyLoader.ReturnRandomProxy(), true);
                    //server.Proxy = proxyObject;

                    string response = server.Get(appUrl);

                    // Sanity Check
                    if (String.IsNullOrEmpty(response) || server.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        log.Info("Error opening app page : " + appUrl);

                        // Renewing WebRequest Object to get rid of Cookies
                        server = new WebRequests();

                        // Inc. retry counter
                        retryCounter++;

                        log.Info("Retrying:" + retryCounter);

                        // Checking for maximum retry count
                        double waitTime;
                        if (retryCounter >= 11)
                        {
                            waitTime = TimeSpan.FromMinutes(35).TotalMilliseconds;
                        }
                        else
                        {
                            // Calculating next wait time ( 2 ^ retryCounter seconds)
                            waitTime = TimeSpan.FromSeconds(Math.Pow(2, retryCounter)).TotalMilliseconds;
                        }

                        // Hiccup to avoid google blocking connections in case of heavy traffic from the same IP
                        Thread.Sleep(Convert.ToInt32(waitTime));
                    }
                    else
                    {
                        // Reseting retry counter
                        retryCounter = 0;

                        // Parsing App Data
                        AppModel parsedApp = parser.ParseAppPage(response, appUrl);

                        // Export the App Data
                        if (exporter != null)
                        {
                            log.Info("Parsed App: " + parsedApp.Name);

                            exporter.Write(parsedApp);
                        }

                        // Pass the App Data to callback method
                        if (writeCallback != null)
                        {
                            writeCallback(parsedApp);
                        }

                        // Default action is print to screen
                        if (exporter == null && writeCallback == null)
                        {
                            Console.WriteLine(parsedApp);
                        }

                        ++parsedAppCount;

                        // Apply download delay
                        if (downloadDelay > 0)
                        {
                            Thread.Sleep(downloadDelay);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    Console.WriteLine(url);
                }
            }

            log.Info("Finished. Parsed App count: " + parsedAppCount + "\n");
        }
    }
}
