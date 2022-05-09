namespace JiraReport.Client.Store.JiraIssues
{
    public class FetchJiraIssuesSetLoadingAction
    {
        public bool Loading { get; }

        public FetchJiraIssuesSetLoadingAction(bool loading)
        {
            Loading = loading;
        }
    }
}
