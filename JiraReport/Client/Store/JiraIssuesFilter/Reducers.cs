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
				ReportedHours = action.ReportedHours,
				HourRate = action.HourRate,
				Residence = action.Residence,
				RoundingDecimals = action.RoundingDecimals,
				TaxId = action.TaxId,
				DateRange = state.DateRange,
				SelectedCurrency = action.Currency,
				TotalPrice = action.TotalPrice
			};
	}
}
