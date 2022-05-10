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

	public class JiraIssuesFilterSetBonusInAdvanceAction
	{
		public decimal BonusInAdvance { get; }

		public JiraIssuesFilterSetBonusInAdvanceAction(decimal bonusInAdvance)
		{
			BonusInAdvance = bonusInAdvance;
		}
	}

	public class JiraIssuesFilterSetOtherPaymentsAction
	{
		public decimal OtherPayments { get; }

		public JiraIssuesFilterSetOtherPaymentsAction(decimal otherPayments)
		{
			OtherPayments = otherPayments;
		}
	}

	public class JiraIssuesFilterSetReportedHoursAction
	{
		public decimal ReportedHours { get; }

		public JiraIssuesFilterSetReportedHoursAction(decimal reportedHours)
		{
			ReportedHours = reportedHours;
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
