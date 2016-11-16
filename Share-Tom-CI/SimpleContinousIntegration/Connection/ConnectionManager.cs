using System;
using System.Net;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using SimpleContinousIntegration.Log;

namespace SimpleContinousIntegration.Connection
{
    public class ConnectionManager
    {
        private readonly ConnectionInfo _connectionInfo;

        public ConnectionManager(ConnectionInfo connectionInfo)
        {
            if (connectionInfo.ServiceAddress.IsNullOrEmpty() || connectionInfo.UserName.IsNullOrEmpty() ||
                connectionInfo.Password.IsNullOrEmpty())
            {
                throw new ArgumentException(
                    $"Some of given arguments are null or empty serviceAddress = {connectionInfo.ServiceAddress}," +
                    $" userName = {connectionInfo.UserName}," +
                    $" passWord = {connectionInfo.Password}.");
            }
            _connectionInfo = connectionInfo;
        }

        public ConnectionManager(string serviceAddress, string userName, string password)
        {
            _connectionInfo = new ConnectionInfo
            {
                ServiceAddress = serviceAddress,
                UserName = userName,
                Password = password
            };
        }
        
        public TfsTeamProjectCollection GetTfsTeamProjectCollection()
        {
            LogManager.Log("Establishing connection to TFS", TextColor.Red);
            var networkCredential = new NetworkCredential(_connectionInfo.UserName, _connectionInfo.Password);
            var basicAuthCredential = new BasicAuthCredential(networkCredential);
            var tfsClientCredentials = new TfsClientCredentials(basicAuthCredential) {AllowInteractive = false};
            var tfsTeamProjectCollection = new TfsTeamProjectCollection(
                new Uri(_connectionInfo.ServiceAddress),
                tfsClientCredentials);

            LogManager.Log("TFS Connection established correctly.", TextColor.Green);

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