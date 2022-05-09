
using JiraReport.Shared;

namespace JiraReport.Client.Store.JiraIssues
{
    public class FetchJiraIssuesResultAction
    {
        public JiraIssueCollection IssueCollection { get; }

        public FetchJiraIssuesResultAction(JiraIssueCollection issueCollection)
        {
            IssueCollection = issueCollection;
        }
    }
}