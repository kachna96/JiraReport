namespace JiraReport.Client.Store.JiraIssuesFilter
{
	public class SetJiraIssuesFilterAction
	{
		public string Name { get; }

		public string ContractorId { get; }

		public string TaxId { get; }

		public string Residence { get; }

		public decimal HourRate { get; }

		public decimal TotalPrice { get; }

		public string Currency { get; }

		public SetJiraIssuesFilterAction(string name, string contractorId, string taxId, string residence, decimal hourRate, decimal totalPrice, string currency)
		{
			Name = name;
			ContractorId = contractorId;
			TaxId = taxId;
			Residence = residence;
			HourRate = hourRate;
			TotalPrice = totalPrice;
			Currency = currency;
		}
	}
}
