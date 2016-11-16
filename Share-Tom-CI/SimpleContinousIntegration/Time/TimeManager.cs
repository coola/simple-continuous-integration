using SimpleContinousIntegration.BuildFolder;

namespace SimpleContinousIntegration.Time
{
    public class TimeManager
    {
        private readonly BuildFolderManager _buildFolderManager;
        public int CurrentWaitPeriod = 2;

        public TimeManager(BuildFolderManager buildFolderManager)
        {
            _buildFolderManager = buildFolderManager;
        }

        public void ResetWaitPeriod()
        {
            CurrentWaitPeriod = 2;
        }

        public void IncreaseWaitPeriod()
        {
            CurrentWaitPeriod = CurrentWaitPeriod < FiveMinutesInSeconds ? CurrentWaitPeriod * 2 : CurrentWaitPeriod;
        }

        private static int FiveMinutesInSeconds => 60 * 5;

        public bool ItIsTimeToBuild()
        {
            var maxCurrentLocalChangeset = _buildFolderManager.GetMaxCurrentLocalChangeset();
            if (maxCurrentLocalChangeset == null) return true;
            var latestChangesetId = _buildFolderManager.GetLatestChangesetId();
            return maxCurrentLocalChangeset < latestChangesetId;
        }
    }
}