namespace JiraReport.Shared
{
    public record JiraIssueCollection
    {
        public IEnumerable<JiraIssue> Issues { get; init; }

        public long TotalTimeSpendInSeconds { get; init; }

        public TimeSpan TotalTime { get; set; }
    }
}
