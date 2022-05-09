namespace JiraReport.Shared
{
	public class JiraIssue
	{
		public string Description { get; init; }

		public string Project { get; init; }

		public string Key { get; init; }

		public string Reporter { get; init; }

		public string Summary { get; init; }

		public string Status { get; init; }

		public string IssueType { get; init; }

		public long TimeSpendInSeconds { get; set; }

		public TimeSpan TimeSpend { get; set; }

		public double TimeSpendInHours
		{
			get
			{
				return TimeSpend.TotalHours;
			}
			set
			{
				TimeSpend = TimeSpan.FromHours(value);
			}
		}

		public Uri Uri { get; set; }

		public bool Checked { get; set; }

		public void SetUri(Uri baseUri)
		{
			Uri = new Uri(baseUri, $"browse/{Key}");
		}

		//public double GetTimeSpendInHours(IEnumerable<(string Key, double Hours)> extraHours)
		//{
		//	var entry = extraHours.FirstOrDefault(x => x.Key == Key);

		//	if (entry == null)
		//	{
		//		return TimeSpendInHours;
		//	}
		//}
	}
}
