namespace JiraReport.Client.Store.JiraIssues
{
    public class FetchJiraIssuesAction
    {
        public DateTime From { get; }

        public DateTime To { get; }

        public FetchJiraIssuesAction(DateTime from, DateTime to)
        {
            From = from;
            To = to;
        }
    }
}
