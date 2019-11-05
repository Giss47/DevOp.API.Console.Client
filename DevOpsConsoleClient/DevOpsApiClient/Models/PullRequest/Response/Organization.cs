namespace DevOpsApiClient.Models.PullRequest.Response
{
    public class Organization
    {
        public string OrganizationName { get; set; }

        public bool Success { get; set; }

        public Repository[] Repositories { get; set; }
    }
}