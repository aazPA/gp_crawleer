using PlayStoreScraper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayStoreScraper.Exporters
{
    /// <summary>
    /// Interface for Exporter class
    /// </summary>
    interface IExporter
    {
        /// <summary>
        /// Handle the opening of output resource.
        /// This method will be called by PlayStoreScraper before the crawler starts.
        /// </summary>
        void Open();

        /// <summary>
        /// Handle the closing of output resource.
        /// This method will be called by PlayStoreScraper after the crawler stops.
        /// </summary>
        void Close();

        /// <summary>
        /// Write the parsed App data to output resource.
        /// This method will be called by PlayStoreScraper after parsing an App page.
        /// </summary>
        /// <param name="appModel">AppModel object containing App data</param>
        void Write(AppModel appModel);
    }
}
