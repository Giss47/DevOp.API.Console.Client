using System;
using System.Collections.Generic;
using System.Text;

namespace DevOpsApiClient.Models.PullRequest.Request
{
    public class PullRequestRequest
    {
        public List<Organization> Organizations { get; set; } = new List<Organization>();
    }
}