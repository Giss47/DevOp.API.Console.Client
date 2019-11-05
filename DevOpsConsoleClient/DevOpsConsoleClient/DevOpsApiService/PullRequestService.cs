using DevOpsApiClient.Models;
using DevOpsApiClient.Models.PullRequest.Request;
using DevOpsApiClient.Models.PullRequest.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hawk.Console.Client.HawkApiService
{
    public static class PullRequestService
    {
        public static async Task<PullRequestResponse> GetAllPR(Dictionary<string, string> orgTokList, Settings settings)
        {
            PullRequestRequest payload = new PullRequestRequest();

            foreach (var OrgTok in orgTokList)
            {
                payload.Organizations.Add(new DevOpsApiClient.Models.PullRequest.Request.Organization { Name = OrgTok.Key, AccessToken = OrgTok.Value });
            }

            PullRequestResponse response = await DevOpsApiClient.Main.GetPullRequestsAsync(payload, settings);

            return response;
        }
    }
}