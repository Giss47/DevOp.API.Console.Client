using DevOpsApiClient.Models;
using Hawk.Console.Client.FileService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hawk.Console.Client
{
    public class ReadWriteOps : IReadWriteOps
    {
        private static readonly string HawkDataFilesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DevOpsConsole");
        private static readonly string SettingsPath = Path.Combine(HawkDataFilesPath, "Settings.json");
        private static readonly string OrgTokenPath = Path.Combine(HawkDataFilesPath, "OrgToken.json");

        /// <summary>
        /// Reads API URL and KEY from a local json file asynchronous
        /// </summary>
        /// <returns>
        /// Setting object with basurl and apiKey Fileds
        /// </returns>
        public Settings ReadSettings()
        {
            return LoadObjectFromJsonFile<Settings>(SettingsPath);
        }

        /// <summary>
        /// Reads list of Organizations and access Tokens from a local json file asynchronous
        /// </summary>
        /// <returns>
        /// A dictionary with Organization name as a key and access token as a value
        /// </returns>
        public Dictionary<string, string> ReadOrgTok()
        {
            return LoadObjectFromJsonFile<Dictionary<string, string>>(OrgTokenPath);
        }

        /// <summary>
        /// Write API uri and key to the local a json file
        /// </summary>
        /// <param name="org"></param>
        /// <param name="tok"></param>
        public void SaveSettings(string baseUrl, string apikey)
        {
            SaveObjectToJsonFile(new Settings(baseUrl, apikey), SettingsPath);
        }

        /// <summary>
        /// Write Organization name and Token to the local a json file
        /// </summary>
        /// <param name="org"></param>
        /// <param name="tok"></param>
        public void SaveOrgTok(Dictionary<string, string> orgTokList)
        {
            SaveObjectToJsonFile(orgTokList, OrgTokenPath);
        }

        public static void SaveObjectToJsonFile<T>(T value, string fileName)
        {
            var json = JsonConvert.SerializeObject(value);
            File.WriteAllText(fileName, json);
        }

        public static T LoadObjectFromJsonFile<T>(string fileName)
        {
            var json = File.ReadAllText(fileName);

            return string.IsNullOrEmpty(json)
                ? default
                : JsonConvert.DeserializeObject<T>(json);
        }
    }
}