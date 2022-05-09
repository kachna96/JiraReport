using JiraReport.Shared;

namespace JiraReport.Client.Store.JiraIssues
{
	public class JiraIssuesActionSetSelectedIssues
	{
		public HashSet<JiraIssue> SelectedIssues { get; }

		public JiraIssuesActionSetSelectedIssues(HashSet<JiraIssue> jiraIssues)
		{
			SelectedIssues = jiraIssues;
		}
	}

	public class JiraIssuesActionSetJiraIssues
	{
		public JiraIssue JiraIssue { get; }

		public JiraIssuesActionSetJiraIssues(JiraIssue jiraIssue)
		{
			JiraIssue = jiraIssue;
		}
	}
}
