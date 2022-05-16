namespace JiraReport.Client.Store.JiraIssuesFilter
{
	public class SetJiraIssuesFilterAction
	{
		public string Name { get; }

		public string ContractorId { get; }

		public string TaxId { get; }

		public string Residence { get; }

		public decimal HourRate { get; }

		public decimal BonusInAdvance { get; }

		public decimal OtherPayments { get; }

		public string Currency { get; }

		public decimal ReportedHours { get; }

		public string Language { get; }

		public SetJiraIssuesFilterAction(string name, string contractorId, string taxId, string residence, decimal hourRate, decimal bonusInAdvance, decimal otherPayments, string currency, decimal reportedHours, string language)
		{
			Name = name;
			ContractorId = contractorId;
			TaxId = taxId;
			Residence = residence;
			HourRate = hourRate;
			BonusInAdvance = bonusInAdvance;
			OtherPayments = otherPayments;
			Currency = currency;
			ReportedHours = reportedHours;
			Language = language;
		}
	}
}
