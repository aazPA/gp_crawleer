using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace PlayStoreScraper.Models
{
    /// <summary>
    /// Object to hold the App Data
    /// </summary>
    public class AppModel
    {
        public string Url                      {get;set;}
        public DateTime ScrapedDate            {get;set;}
        public string   Name                   {get;set;}
        public string   Developer              {get;set;}
        public bool     IsTopDeveloper         {get;set;}
        public string   DeveloperURL           {get;set;}
        public DateTime PublicationDate        {get;set;}
        public string   Category               {get;set;}
        public bool     IsFree                 {get;set;}
        public string   Price                  {get;set;}
        public string   CoverImageUrl          {get;set;}
        public string   Description            {get;set;}
        public double   ReviewScore            {get;set;}
        public int      ReviewTotal            {get;set;}
        public int      FiveStarsReviews       {get;set;}
        public int      FourStarsReviews       {get;set;}
        public int      ThreeStarsReviews      {get;set;}
        public int      TwoStarsReviews        {get;set;}
        public int      OneStarReviews         {get;set;}   
        /// <summary>
        /// App Size in MB. Value -1 means "Varies with device"
        /// </summary>
        public Double   AppSize                {get;set;}
        public string   Installs               {get;set;}
        public string   CurrentVersion         {get;set;}
        public string   MinimumOSVersion       {get;set;}
        public string   ContentRating          {get;set;}
        public bool     HaveInAppPurchases     {get;set;}
        public string   InAppPriceRange        {get;set;}
        public string   DeveloperEmail         {get;set;}
        public string   DeveloperWebsite       {get;set;}
        public string   DeveloperPrivacyPolicy {get;set;}

        public AppModel()
        {
            DeveloperEmail = String.Empty;
            DeveloperWebsite = String.Empty;
            DeveloperPrivacyPolicy = String.Empty;
        }

        /// <summary>
        /// Get all property names from this class
        /// </summary>
        /// <returns>Array of property names</returns>
        public static string[] GetAllPropertyNames()
        {
            List<string> list = new List<string>();

            Type appModelType = typeof(AppModel);
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(appModelType))
            {
                list.Add(prop.Name);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Get Property Value by Property Name
        /// </summary>
        /// <param name="name">Property Name</param>
        /// <returns>Property Value</returns>
        public Object GetPropertyValue(String name)
        {
            Object obj = this;

            foreach (String part in name.Split('.'))
            {
                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null)
                {
                    return null;
                }

                obj = info.GetValue(this, null);
                if (obj == null)
                {
                    return null;
                }
            }
            return obj;
        }

        /// <summary>
        /// Serialize Object as JSON string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
