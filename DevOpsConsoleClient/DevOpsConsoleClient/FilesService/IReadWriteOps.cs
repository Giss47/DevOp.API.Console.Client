using DevOpsApiClient.Models;
using System.Collections.Generic;

namespace Hawk.Console.Client.FileService
{
    public interface IReadWriteOps
    {
        Settings ReadSettings();

        void SaveSettings(string api, string key);

        void SaveOrgTok(Dictionary<string, string> orgTekList);

        Dictionary<string, string> ReadOrgTok();
    }
}