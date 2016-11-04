namespace SimpleContinousIntegration
{
    public class ResultsManager
    {
        private readonly string _buildFolderPath;
        private readonly bool _buildResult;
        private readonly bool _testsRunResult;
        public const string _buildOKFileName = "Build.OK.txt";
        private const string _buildFailedFileName = "Build.FAILED.txt";
        private const string _testsOKFileName = "Tests.OK.txt";
        private const string _testsFailedFileName = "Tests.FAILED.txt";

        public ResultsManager(string buildFolderPath, bool buildResult, bool testsRunResult)
        {
            _buildFolderPath = buildFolderPath;
            _buildResult = buildResult;
            _testsRunResult = testsRunResult;
        }

        public void SaveResults()
        {
            CreateFile(_buildResult ? _buildOKFileName : _buildFailedFileName);
            CreateFile(_testsRunResult ? _testsOKFileName : _testsFailedFileName);
        }

        private void CreateFile( string fileName)
        {
             System.IO.File.Create(System.IO.Path.Combine(_buildFolderPath, fileName));
        }
    }
}
