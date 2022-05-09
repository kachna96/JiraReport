using Fluxor;

namespace JiraReport.Client.Store.JiraIssuesFilter
{
	public static class Reducers
	{
		[ReducerMethod]
		public static JiraIssuesFilterState ReduceSetJiraIssuesFilterAction(JiraIssuesFilterState state, SetJiraIssuesFilterAction action) =>
			state with
			{
				Name = action.Name,
				ContractorId = action.ContractorId,
				HourRate = action.HourRate,
				Residence = action.Residence,
				TaxId = action.TaxId,
				DateRange = state.DateRange,
				SelectedCurrency = action.Currency,
				TotalPrice = action.TotalPrice
			};

		[ReducerMethod]
		public static JiraIssuesFilterState ReduceJiraIssuesFilterActionSetHourRate(JiraIssuesFilterState state, JiraIssuesFilterSetHourRateAction action) =>
			state with
			{
				HourRate = action.HourRate
			};

		[ReducerMethod]
		public static JiraIssuesFilterState ReduceJiraIssuesFilterActionSetTotalPrice(JiraIssuesFilterState state, JiraIssuesFilterSetTotalPriceAction action) =>
			state with
			{
				TotalPrice = action.TotalPrice
			};

		[ReducerMethod]
		public static JiraIssuesFilterState ReduceJiraIssuesFilterActionSetDateRange(JiraIssuesFilterState state, JiraIssuesFilterSetDateRangeAction action) =>
			state with
			{
				DateRange = action.DateRange
			};
	}
}
