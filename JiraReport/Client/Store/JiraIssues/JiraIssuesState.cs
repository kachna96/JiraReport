using Fluxor;
using JiraReport.Shared;

namespace JiraReport.Client.Store.JiraIssues
{
	[FeatureState]
	public record JiraIssuesState
	{
		public IEnumerable<JiraIssue> Issues { get; set; } = new List<JiraIssue>();

		public HashSet<JiraIssue> SelectedIssues { get; set; } = new HashSet<JiraIssue>();

		public long TotalTimeSpendInSeconds { get; }

		public TimeSpan TotalTime { get; init; }

		public bool Loading { get; init; } = false;

		public bool Initialized => Issues.Any();

		public decimal BonusValue { get; set; }

		private JiraIssuesState() { }

		public JiraIssuesState(IEnumerable<JiraIssue> jiraIssues, long totalTimeSpendInSeconds, TimeSpan totalTime, bool loading)
		{
			Issues = jiraIssues;
			SelectedIssues = jiraIssues.ToHashSet();
			TotalTimeSpendInSeconds = totalTimeSpendInSeconds;
			TotalTime = totalTime;
			Loading = loading;
		}
	}
}
