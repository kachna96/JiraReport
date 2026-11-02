using Fluxor;
using JiraReport.Shared;

namespace JiraReport.Client.Store.JiraIssues
{
    [FeatureState]
    public record JiraIssuesState
    {
        public IEnumerable<JiraIssue> Issues { get; set; } = new List<JiraIssue>();

        public HashSet<JiraIssue> SelectedIssues { get; set; } = new HashSet<JiraIssue>();

        public long TotalTimeSpendInSeconds { get; init; }

        public TimeSpan TotalTime { get; init; }

        public bool Loading { get; init; } = false;

        public bool Initialized { get; init; } = false;

        public decimal BonusValue { get; set; }

        private JiraIssuesState() { }

        public JiraIssuesState(IEnumerable<JiraIssue> jiraIssues, long totalTimeSpendInSeconds, TimeSpan totalTime, bool loading, bool initialized = true)
        {
            var list = jiraIssues as IReadOnlyList<JiraIssue> ?? jiraIssues.ToList();
            Issues = list;
            SelectedIssues = list.ToHashSet();
            TotalTimeSpendInSeconds = totalTimeSpendInSeconds;
            TotalTime = totalTime;
            Loading = loading;
            Initialized = initialized;
        }
    }
}
