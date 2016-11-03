using System;
using System.Net;
using Microsoft.TeamFoundation.Client;

namespace Coola.VisualStudioServices.SimpleContinousIntegration
{
    public class ConnectionManager
    {
        private readonly ConnectionInfo _connectionInfo;

        public ConnectionManager(ConnectionInfo connectionInfo)
        {
            _connectionInfo = connectionInfo;
        }

        public TfsTeamProjectCollection GetTfsTeamProjectCollection()
        {
            var networkCredential = new NetworkCredential(
                _connectionInfo.UserName , _connectionInfo.Password
                );
            var basicAuthCredential = new BasicAuthCredential(networkCredential);
            var tfsClientCredentials = new TfsClientCredentials(basicAuthCredential) { AllowInteractive = false };
            var tfsTeamProjectCollection = new TfsTeamProjectCollection(
                new Uri(_connectionInfo.ServiceAddress),
                tfsClientCredentials);
            return tfsTeamProjectCollection;
        }

        public bool Validate()
        {
            var tfsTeamProjectCollection = GetTfsTeamProjectCollection();

            tfsTeamProjectCollection.Authenticate();

            return tfsTeamProjectCollection.HasAuthenticated;
        }
    }
}