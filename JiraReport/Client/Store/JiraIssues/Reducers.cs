using Fluxor;

namespace JiraReport.Client.Store.JiraIssues
{
    public static class Reducers
    {
        [ReducerMethod]
        public static JiraIssuesState ReduceJiraIssuesResultAction(JiraIssuesState _, FetchJiraIssuesResultAction action) =>
            new(action.IssueCollection.Issues, action.IssueCollection.TotalTimeSpendInSeconds, action.IssueCollection.TotalTime, loading: false);

        [ReducerMethod]
        public static JiraIssuesState ReduceSetLoadingAction(JiraIssuesState state, FetchJiraIssuesSetLoadingAction action) =>
            state with
            {
                Loading = action.Loading
            };
    }
}
