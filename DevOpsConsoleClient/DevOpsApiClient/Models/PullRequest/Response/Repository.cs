using System;
using System.Collections.Generic;
using System.Text;

namespace DevOpsApiClient.Models.PullRequest.Response
{
    public class Repository
    {
        public string Name { get; set; }

        public PullRequest[] PullRequests { get; set; }
    }
}