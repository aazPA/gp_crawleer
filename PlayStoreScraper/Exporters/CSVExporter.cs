using PlayStoreScraper.Exporters;
using PlayStoreScraper.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace PlayStoreScraper.Exporters
{
    class CSVExporter : FileExporter, IExporter
    {
        public string OutputFilePath { get; set; }
        public string[] FieldNames { get; set; }
        public bool IsWriteHeaders { get; set; }
        public bool IsAppend { get; set; }

        private StreamWriter writer;

        // Hold field names to be written
        private string[] _fieldNames;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="OutputFilePath">Output File Path</param>
        /// <param name="FieldNames">
        ///     Array of Field Names to be exported. This is useful for setting the ordering of fields.
        ///     If this value is null, all of field names in AppModel will be exported.
        /// </param>
        /// <param name="IsWriteHeaders">
        ///     If true, application will write column headers before writing data (default = true)
        /// </param>
        /// <param name="IsAppend">If true, append the data to CSV file instead of overwriting it.</param>
        public CSVExporter(string OutputFilePath, string[] FieldNames = null, bool IsWriteHeaders=true, bool IsAppend=false)
        {
            this.OutputFilePath = OutputFilePath;
            this.FieldNames = FieldNames;
            this.IsWriteHeaders = IsWriteHeaders;
            this.IsAppend = IsAppend;
        }

        public void Open()
        {
            // Validating Write Permissions on output path
            if (!ValidateFilePermissions(OutputFilePath))
            {
                throw new IOException("Insuficient Permissions - Cannot write on path : " + OutputFilePath);
            }

            writer = new StreamWriter(OutputFilePath, IsAppend);
            writer.AutoFlush = true;

            Console.WriteLine("Result will be written to: " + OutputFilePath);

            if (FieldNames != null)
            {
                _fieldNames = FieldNames;

                ValidateFieldNames(_fieldNames);
                
            }
            else
            {
                _fieldNames = AppModel.GetAllPropertyNames();
            }

            if (IsWriteHeaders)
            {
                writer.WriteLine("\"" + string.Join("\",\"", _fieldNames) + "\"");
            }
        }

        public void Close()
        {
            writer.Close();
            Console.WriteLine("CSV Export has finished");
        }

        public void Write(AppModel appModel)
        {
            string delimiter = "";
            string line = "";

            foreach (string fieldName in _fieldNames)
            {
                object value = appModel.GetPropertyValue(fieldName);

                if (value != null && value.GetType() == typeof(DateTime))
                {
                    // ReferenceDate is formatted as DateTime.
                    // We have to do a fieldname check because there is no Date type in C#.
                    if (fieldName.Equals("ReferenceDate"))
                    {
                        value = ((DateTime)value).ToString("yyyy-MM-dd HH:mm");
                    }
                    else
                    {
                        value = ((DateTime)value).ToString("yyyy-MM-dd");
                    }
                }

                line += delimiter + "\"" + value + "\"";

                delimiter = ",";
            }

            writer.WriteLine(line);
        }
    }
}
