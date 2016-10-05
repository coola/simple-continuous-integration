using System;
using System.Net;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Share_Tom_CI
{
    public class ConnectionValidator
    {
        public bool Validate()
        {
            Uri collectionUri = new Uri("https://keringdev.visualstudio.com/");

            NetworkCredential credential = new NetworkCredential("", "");
            TfsTeamProjectCollection teamProjectCollection = new TfsTeamProjectCollection(collectionUri, credential);
            teamProjectCollection.EnsureAuthenticated();

            WorkItemStore workItemStore = teamProjectCollection.GetService<WorkItemStore>();

            WorkItemCollection workItemCollection = workItemStore.Query("QUERY HERE");

            foreach (var item in workItemCollection)
            {
                //Do something here.
            }
            return false;
        }
    }
}
