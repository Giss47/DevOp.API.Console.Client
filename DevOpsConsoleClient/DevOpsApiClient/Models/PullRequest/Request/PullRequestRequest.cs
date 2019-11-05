using System.Collections.Generic;

namespace DevOpsApiClient.Models.PullRequest.Request
{
    public class PullRequestRequest
    {
        public List<Organization> Organizations { get; set; } = new List<Organization>();
    }
}