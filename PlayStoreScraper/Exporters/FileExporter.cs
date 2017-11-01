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
    abstract class FileExporter
    {
        public static void ValidateFieldNames(string[] fieldNames)
        {
            foreach (string fieldName in fieldNames)
            {
                if (string.IsNullOrWhiteSpace(fieldName))
                {
                    throw new ArgumentException("Field Name can't be empty or null");
                }
            }
        }

        public static bool ValidateFilePermissions(string filePath)
        {
            string directoryName = Directory.GetParent(filePath).FullName;

            PermissionSet permissionSet = new PermissionSet(PermissionState.None);

            FileIOPermission writePermission = new FileIOPermission(FileIOPermissionAccess.Write, directoryName);

            permissionSet.AddPermission(writePermission);

            if (permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
