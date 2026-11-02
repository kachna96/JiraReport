namespace JiraReport.Shared
{
    public class JiraIssue
    {
        public string Description { get; init; } = string.Empty;

        public string Project { get; init; } = string.Empty;

        public string Key { get; init; } = string.Empty;

        public string Reporter { get; init; } = string.Empty;

        public string Summary { get; init; } = string.Empty;

        public string Status { get; init; } = string.Empty;

        public string IssueType { get; init; } = string.Empty;

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
                TimeSpendInSeconds = (long)Math.Round(TimeSpend.TotalSeconds);
            }
        }

        public Uri? Uri { get; set; }

        public bool Checked { get; set; }

        public void SetUri(Uri baseUri)
        {
            ArgumentNullException.ThrowIfNull(baseUri);
            var baseStr = baseUri.OriginalString.TrimEnd('/');
            Uri = new Uri($"{baseStr}/browse/{Key}");
        }
    }
}
