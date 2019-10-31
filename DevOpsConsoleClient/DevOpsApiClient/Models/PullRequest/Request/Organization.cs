using System;
using System.Collections.Generic;
using System.Text;

namespace DevOpsApiClient.Models.PullRequest.Request
{
    public class Organization
    {
        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                ValidateInput(value);
                name = value;
            }
        }

        private string accessToken;

        public string AccessToken
        {
            get { return accessToken; }
            set
            {
                ValidateInput(value);
                accessToken = value;
            }
        }

        public Organization()
        {
        }

        public Organization(string name, string accessToken)
        {
            this.Name = name;
            this.AccessToken = accessToken;
        }

        private void ValidateInput(
            string input,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException($"No valid input given for {memberName}. Can´t be null, only whitespaces or empty.");
            }
        }
    }
}