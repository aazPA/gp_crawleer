using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PlayStoreScraper.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace PlayStoreScraper.Exporters
{
    class JSONExporter : FileExporter, IExporter
    {
        public string OutputFilePath { get; set; }
        public string[] FieldNames { get; set; }

        private StreamWriter writer;

        // Hold field names to be written
        private string[] _fieldNames;

        private string delimiter = "";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="OutputFilePath">Output File Path</param>
        /// <param name="FieldNames">
        ///     Array of Field Names to be exported. This is useful for setting the ordering of fields.
        ///     If this value is null, all of field names in AppModel will be exported.
        /// </param>
        public JSONExporter(string OutputFilePath, string[] FieldNames = null)
        {
            this.OutputFilePath = OutputFilePath;
            this.FieldNames = FieldNames;
        }

        public void Open()
        {
            // Validating Write Permissions on output path
            if (!ValidateFilePermissions(OutputFilePath))
            {
                throw new IOException("Insuficient Permissions - Cannot write on path : " + OutputFilePath);
            }

            writer = new StreamWriter(OutputFilePath);
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

            writer.Write("[");  // Write opening bracket
        }

        public void Close()
        {
            writer.Write("]");  // Write closing bracket
            writer.Close();
            Console.WriteLine("JSON Export has finished");
        }

        public void Write(AppModel appModel)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { 
                ContractResolver = new JSONIncludeContractResolver(new HashSet<string>(_fieldNames)) };

            writer.Write(delimiter + JsonConvert.SerializeObject(appModel, jsonSettings));

            delimiter = ",";
        }

        /// <summary>
        /// ContractResolver class for serializing certain property names
        /// </summary>
        protected class JSONIncludeContractResolver : DefaultContractResolver
        {
            private ISet<string> properties;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="properties">Set of property names that will be serialized</param>
            public JSONIncludeContractResolver(ISet<string> properties)
            {
                this.properties = properties;
            }

            protected override JsonProperty CreateProperty(System.Reflection.MemberInfo member, Newtonsoft.Json.MemberSerialization memberSerialization)
            {
                JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);

                if (!properties.Contains(jsonProperty.PropertyName))
                {
                    jsonProperty.ShouldSerialize = instance => { return false; };
                }

                return jsonProperty; 
            }
        }
    }
}
