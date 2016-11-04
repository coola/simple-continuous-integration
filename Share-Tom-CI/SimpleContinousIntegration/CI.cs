
namespace SimpleContinousIntegration
{
    public class CI
    {
        private readonly string _serviceAddress;
        private readonly string _projectFolderPath;
        private readonly string _userName;
        private readonly string _passWord;
        private readonly string _localBuildFolder;
        private readonly int? _changesetId;

        public CI(string serviceAddress, string projectFolderPath, string userName, string passWord, string localBuildFolder, int? changesetId)
        {
            _serviceAddress = serviceAddress;
            _projectFolderPath = projectFolderPath;
            _userName = userName;
            _passWord = passWord;
            _localBuildFolder = localBuildFolder;
            _changesetId = changesetId;
        }

        public void RetrieveCodeAndBuildAndRunTestsAndSaveResults()
        {
            var connectionManager = new ConnectionManager(new ConnectionInfo
            {
                ServiceAddress = _serviceAddress,
                UserName = _userName,
                Password = _passWord
            });

            var tfsTeamProjectCollection = connectionManager.GetTfsTeamProjectCollection();

            var codeManager = new CodeManager(tfsTeamProjectCollection, _projectFolderPath, _localBuildFolder);
        }
    }
}
