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
            NetworkCredential netCred = new NetworkCredential(
                 "coola",
                 "CoolaHaslo123");
            BasicAuthCredential basicCred = new BasicAuthCredential(netCred);
            TfsClientCredentials tfsCred = new TfsClientCredentials(basicCred) {AllowInteractive = false};

            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(
                new Uri("https://keringdev.visualstudio.com"),
                tfsCred);

            tpc.Authenticate();

            return tpc.HasAuthenticated;
        }
    }
}
