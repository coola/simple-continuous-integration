using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace Share_Tom_CI
{
    public class CodeManager
    {
        public bool GetCode()
        {
            var tpc = ConnectionManager.GetTfsTeamProjectCollection();

            tpc.Authenticate();

            VersionControlServer vcServer = tpc.GetService<VersionControlServer>();
            ItemSet itemSet = vcServer.GetItems("$/", RecursionType.OneLevel);
            foreach (Item item in itemSet.Items)
            {
                Debug.WriteLine(item.ServerItem);
            }

            // Can retrieve REST client from same TfsTeamProjectCollection instance
            TfvcHttpClient tfvcClient = tpc.GetClient<TfvcHttpClient>();
            List<TfvcItem> tfvcItems = tfvcClient.GetItemsAsync("$/", VersionControlRecursionType.Full).Result;
            foreach (TfvcItem item in tfvcItems)
            {
                Debug.WriteLine(item.Path);
            }

            //tpc.Connect(ConnectOptions.None);

            return tpc.HasAuthenticated;
        }
    }
}