using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Coola.VisualStudioServices.SimpleContinousIntegration
{
    public class MailManager
    {
        private readonly int _brokenCommitNumber;
        private readonly string _nameOfPersonWhoBrokeTheCommit;

        public MailManager(int brokenCommitNumber, string nameOfPersonWhoBrokeTheCommit)
        {
            _brokenCommitNumber = brokenCommitNumber;
            _nameOfPersonWhoBrokeTheCommit = nameOfPersonWhoBrokeTheCommit;
        }

        private string Body =>
                            $@"
                            Hello {_nameOfPersonWhoBrokeTheCommit}
                            Your commit number {_brokenCommitNumber} has broken build.
                            Please Fix it ASAP";

   
        public void SendNoBuildMessageUsingVisualStudioServices(TfsTeamProjectCollection getTfsTeamProjectCollection)
        {
            var workItemStore = getTfsTeamProjectCollection.GetService<WorkItemStore>();
            var teamProject = workItemStore.Projects["CITestProject"];
            var workItemType = teamProject.WorkItemTypes["Task"];
         
            var userStory = new WorkItem(workItemType)
            {
                Title = "Bad Build",
                Description = Body
            };

            userStory.Fields["System.AssignedTo"].Value = _nameOfPersonWhoBrokeTheCommit;
          
            userStory.Save();
        }
    }
}
