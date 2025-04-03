using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentValidator.Utils
{
    public static class GetDownloadLocation
    {
        // Static field to store the GUID for the lifetime of the program
        private static readonly string _downloadsFolderPath = CreateDownloadsFolderPath();

        public static string GetDownloadsFolderPath()
        {
            return _downloadsFolderPath;
        }

        private static string CreateDownloadsFolderPath()
        {
            // Retrieve the Downloads folder path
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string downloadsPath = Path.Combine(userProfile, "Downloads", Guid.NewGuid().ToString());

            // Ensure the directory exists (only created once)
            Directory.CreateDirectory(downloadsPath);
            return downloadsPath;
        }
    }

}
