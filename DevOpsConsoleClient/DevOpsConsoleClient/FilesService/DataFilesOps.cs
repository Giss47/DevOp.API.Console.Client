using System;
using System.IO;

namespace Hawk.Console.Client.FileService
{
    public class DataFilesOps
    {
        private static readonly string appDataDirPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>
        /// Check for settings and Organisation/Token files if they exist localy, {user}/AppData/Roaming, if not, creates them
        /// </summary>
        public void CreateDataFiles()
        {
            var appDataDirPath = Path.Combine(DataFilesOps.appDataDirPath, "DevOpsConsole");
            if (!Directory.Exists(appDataDirPath))
            {
                Directory.CreateDirectory(appDataDirPath);
            }

            CreateFiles(appDataDirPath, new string[] { "Settings.json", "OrgToken.json" });
        }

        private void CreateFiles(string directory, string[] fileNames)
        {
            foreach (var fileName in fileNames)
            {
                var filePath = Path.Combine(directory, fileName);
                using FileStream fs1 = new FileStream(filePath, FileMode.OpenOrCreate);
            }
        }
    }
}