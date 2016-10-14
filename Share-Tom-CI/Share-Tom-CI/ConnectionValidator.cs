using System;
using System.Net;
using Microsoft.TeamFoundation.Client;

namespace Share_Tom_CI
{
    public class ConnectionValidator
    {
        public bool Validate()
        {
            var tpc = ConnectionManager.GetTfsTeamProjectCollection();

            tpc.Authenticate();

            return tpc.HasAuthenticated;
        }
    }
}