using Fluxor;
using TeixeiraSoftware.Finance;

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
				HourRate = action.HourRate > 0 ? action.HourRate : 1,
				Residence = action.Residence,
				TaxId = action.TaxId,
				DateRange = state.DateRange,
				SelectedCurrency = string.IsNullOrEmpty(action.Currency) ? Currency.CZK.Symbol : action.Currency,
				TotalPrice = action.HourRate * action.ReportedHours + action.BonusInAdvance + action.OtherPayments,
				BonusInAdvance = action.BonusInAdvance,
				OtherPayments = action.OtherPayments,
				ReportedHours = action.ReportedHours > 0 ? action.ReportedHours : 1,
				Language = string.IsNullOrEmpty(action.Language) ? "CZ" : action.Language
			};

		[ReducerMethod]
		public static JiraIssuesFilterState ReduceJiraIssuesFilterActionSetHourRate(JiraIssuesFilterState state, JiraIssuesFilterSetHourRateAction action) =>
			state with
			{
				HourRate = action.HourRate > 0 ? action.HourRate : 1,
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
				ReportedHours = action.ReportedHours > 0 ? action.ReportedHours : 1,
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
