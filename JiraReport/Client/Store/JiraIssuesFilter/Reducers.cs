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
				TotalPrice = action.HourRate * action.ReportedHours + action.BonusInAdvance + action.OtherPayments,
				BonusInAdvance = action.BonusInAdvance,
				OtherPayments = action.OtherPayments,
				ReportedHours = action.ReportedHours
			};

		[ReducerMethod]
		public static JiraIssuesFilterState ReduceJiraIssuesFilterActionSetHourRate(JiraIssuesFilterState state, JiraIssuesFilterSetHourRateAction action) =>
			state with
			{
				HourRate = action.HourRate,
				TotalPrice = action.HourRate * state.ReportedHours + state.BonusInAdvance + state.OtherPayments
			};

		[ReducerMethod]
		public static JiraIssuesFilterState ReduceJiraIssuesFilterActionSetBonusInAdvance(JiraIssuesFilterState state, JiraIssuesFilterSetBonusInAdvanceAction action) =>
			state with
			{
				BonusInAdvance = action.BonusInAdvance,
				TotalPrice = state.HourRate * state.ReportedHours + action.BonusInAdvance + state.OtherPayments
			};

		[ReducerMethod]
		public static JiraIssuesFilterState ReduceJiraIssuesFilterActionSetOtherPayments(JiraIssuesFilterState state, JiraIssuesFilterSetOtherPaymentsAction action) =>
			state with
			{
				OtherPayments = action.OtherPayments,
				TotalPrice = state.HourRate * state.ReportedHours + state.BonusInAdvance + action.OtherPayments
			};

		[ReducerMethod]
		public static JiraIssuesFilterState ReduceJiraIssuesFilterActionSetReportedHours(JiraIssuesFilterState state, JiraIssuesFilterSetReportedHoursAction action) =>
			state with
			{
				ReportedHours = action.ReportedHours,
				TotalPrice = state.HourRate * action.ReportedHours + state.BonusInAdvance + state.OtherPayments
			};

		[ReducerMethod]
		public static JiraIssuesFilterState ReduceJiraIssuesFilterActionSetDateRange(JiraIssuesFilterState state, JiraIssuesFilterSetDateRangeAction action) =>
			state with
			{
				DateRange = action.DateRange
			};
	}
}
