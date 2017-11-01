using PlayStoreScraper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PlayStoreScraper.ProxyHandler
{
    class ProxyLoader
    {
        public static List<string> stringifiedProxies = new List<string>();



        public static void InitializeProxies ()
        {
            stringifiedProxies.Add("85.91.96.56:8080");
            stringifiedProxies.Add("36.67.64.129:65103");
            stringifiedProxies.Add("185.64.220.113:65103");
            stringifiedProxies.Add("194.106.219.34:3128");
            stringifiedProxies.Add("42.115.88.42:65103");
        }


        public static string ReturnRandomProxy ()
        {
            var random = new Random();
            int index = random.Next(stringifiedProxies.Count);
            return stringifiedProxies[index];
        }
    }
}
