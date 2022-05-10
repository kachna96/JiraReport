using Fluxor;
using MudBlazor;
using TeixeiraSoftware.Finance;

namespace JiraReport.Client.Store.JiraIssuesFilter
{
	[FeatureState]
	public record JiraIssuesFilterState
	{
		public DateRange DateRange { get; set; }

		public string Name { get; set; }

		public string ContractorId { get; set; }

		public string TaxId { get; set; }

		public string Residence { get; set; }

		public decimal HourRate { get; set; }

		public decimal BonusInAdvance { get; set; }

		public decimal OtherPayments { get; set; }

		public decimal TotalPrice { get; set; }

		public decimal ReportedHours { get; set; }

		public IReadOnlyCollection<Currency> Currencies { get; } = Currency.AllCurrencies;

		public string SelectedCurrency { get; set; }

		public JiraIssuesFilterState()
		{
			var currentDate = DateTime.Now;
			var startDate = currentDate.AddMonths(-1).AddDays(1 - currentDate.Day);
			var endDate = startDate.AddMonths(1).AddDays(-1);
			DateRange = new DateRange(startDate, endDate);
		}
	}
}
