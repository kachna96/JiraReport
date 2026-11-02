using Fluxor;

namespace JiraReport.Client.Store.JiraIssues
{
	public static class Reducers
	{
		[ReducerMethod]
		public static JiraIssuesState ReduceJiraIssuesResultAction(JiraIssuesState _, FetchJiraIssuesResultAction action) =>
			new(action.IssueCollection.Issues ?? [], action.IssueCollection.TotalTimeSpendInSeconds, action.IssueCollection.TotalTime, loading: false);

		[ReducerMethod]
		public static JiraIssuesState ReduceSetLoadingAction(JiraIssuesState state, FetchJiraIssuesSetLoadingAction action) =>
			state with
			{
				Loading = action.Loading
			};

		[ReducerMethod]
		public static JiraIssuesState ReduceSetSelectedJiraIssuesAction(JiraIssuesState state, JiraIssuesActionSetSelectedIssues action) =>
			state with
			{
				SelectedIssues = action.SelectedIssues
			};

		[ReducerMethod]
		public static JiraIssuesState ReduceSetJiraIssuesAction(JiraIssuesState state, JiraIssuesActionSetJiraIssues action)
		{
			var list = state.Issues.ToList();
			var index = list.FindIndex(x => x.Key == action.JiraIssue.Key);
			if (index >= 0)
			{
				list[index] = action.JiraIssue;
			}

			var selected = state.SelectedIssues.ToList();
			var selectedIndex = selected.FindIndex(x => x.Key == action.JiraIssue.Key);
			if (selectedIndex >= 0)
			{
				selected[selectedIndex] = action.JiraIssue;
			}

			var totalSeconds = list.Sum(x => x.TimeSpendInSeconds);

			return state with
			{
				Issues = list,
				SelectedIssues = selected.ToHashSet(),
				TotalTimeSpendInSeconds = totalSeconds,
				TotalTime = TimeSpan.FromSeconds(totalSeconds)
			};
		}
	}
}
