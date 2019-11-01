using System;
using System.Collections.Generic;
using System.Text;

namespace DevOpsApiClient.Models
{
    public class Settings
    {
        public string BaseUrl { get; set; }

        public string ApiKey { get; set; }

        public Settings(string baseUrl = null, string apiKey = null)
        {
            BaseUrl = baseUrl ?? Environment.GetEnvironmentVariable("API_BASE_URL") ?? throw new ArgumentNullException(nameof(baseUrl));
            ApiKey = apiKey ?? Environment.GetEnvironmentVariable("API_KEY") ?? throw new ArgumentNullException(nameof(apiKey));
        }
    }
}