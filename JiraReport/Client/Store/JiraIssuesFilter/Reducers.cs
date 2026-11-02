using Fluxor;
using TeixeiraSoftware.Finance;

namespace JiraReport.Client.Store.JiraIssuesFilter
{
	public static class Reducers
	{
		[ReducerMethod]
		public static JiraIssuesFilterState ReduceSetJiraIssuesFilterAction(JiraIssuesFilterState state, SetJiraIssuesFilterAction action)
		{
			var hourRate = action.HourRate > 0 ? action.HourRate : 1;
			var reportedHours = action.ReportedHours > 0 ? action.ReportedHours : 1;

			return state with
			{
				Name = action.Name,
				ContractorId = action.ContractorId,
				HourRate = hourRate,
				Residence = action.Residence,
				TaxId = action.TaxId,
				DateRange = state.DateRange,
				SelectedCurrency = string.IsNullOrEmpty(action.Currency) ? Currency.CZK.Symbol : action.Currency,
				TotalPrice = hourRate * reportedHours + action.BonusInAdvance + action.OtherPayments,
				BonusInAdvance = action.BonusInAdvance,
				OtherPayments = action.OtherPayments,
				ReportedHours = reportedHours,
				Language = string.IsNullOrEmpty(action.Language) ? "CZ" : action.Language
			};
		}

		[ReducerMethod]
		public static JiraIssuesFilterState ReduceJiraIssuesFilterActionSetHourRate(JiraIssuesFilterState state, JiraIssuesFilterSetHourRateAction action)
		{
			var hourRate = action.HourRate > 0 ? action.HourRate : 1;
			return state with
			{
				HourRate = hourRate,
				TotalPrice = hourRate * state.ReportedHours + state.BonusInAdvance + state.OtherPayments
			};
		}

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
		public static JiraIssuesFilterState ReduceJiraIssuesFilterActionSetReportedHours(JiraIssuesFilterState state, JiraIssuesFilterSetReportedHoursAction action)
		{
			var reportedHours = action.ReportedHours > 0 ? action.ReportedHours : 1;
			return state with
			{
				ReportedHours = reportedHours,
				TotalPrice = state.HourRate * reportedHours + state.BonusInAdvance + state.OtherPayments
			};
		}

		[ReducerMethod]
		public static JiraIssuesFilterState ReduceJiraIssuesFilterActionSetDateRange(JiraIssuesFilterState state, JiraIssuesFilterSetDateRangeAction action) =>
			state with
			{
				DateRange = action.DateRange
			};
	}
}
