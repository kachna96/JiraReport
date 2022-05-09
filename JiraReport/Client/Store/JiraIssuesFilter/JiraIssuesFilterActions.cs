using MudBlazor;

namespace JiraReport.Client.Store.JiraIssuesFilter
{
	public class JiraIssuesFilterSetHourRateAction
	{
		public decimal HourRate { get; }

		public JiraIssuesFilterSetHourRateAction(decimal hourRate)
		{
			HourRate = hourRate;
		}
	}

	public class JiraIssuesFilterSetTotalPriceAction
	{
		public decimal TotalPrice { get; }

		public JiraIssuesFilterSetTotalPriceAction(decimal totalPrice)
		{
			TotalPrice = totalPrice;
		}
	}

	public class JiraIssuesFilterSetDateRangeAction
	{
		public DateRange DateRange { get; }

		public JiraIssuesFilterSetDateRangeAction(DateRange dateRange)
		{
			DateRange = dateRange;
		}
	}
}
