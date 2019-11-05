using System;

namespace DevOpsApiClient.Models.PullRequest.Response
{
    public class PullRequest
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string DisplayName { get; set; }

        public Uri Url { get; set; }
    }
}