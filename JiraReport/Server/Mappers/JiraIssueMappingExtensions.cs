using Atlassian.Jira;
using JiraReport.Shared;

namespace JiraReport.Server.Mappers
{
    public static class JiraIssueMappingExtensions
    {
        public static JiraIssue ToJiraIssue(this Issue issue)
        {
            ArgumentNullException.ThrowIfNull(issue);

            return new JiraIssue
            {
                Description = issue.Description,
                Project = issue.Project,
                Key = issue.Key?.Value ?? issue.Key?.ToString() ?? string.Empty,
                Reporter = issue.Reporter,
                Summary = issue.Summary,
                Status = issue.Status?.Name ?? issue.Status?.ToString() ?? string.Empty,
                IssueType = issue.Type?.Name ?? string.Empty
            };
        }

        public static JiraIssueCollection ToCollection(this IEnumerable<JiraIssue> issues)
        {
            ArgumentNullException.ThrowIfNull(issues);

            var list = issues as IReadOnlyCollection<JiraIssue> ?? issues.ToList();
            var totalSeconds = list.Sum(x => x.TimeSpendInSeconds);

            return new JiraIssueCollection
            {
                Issues = list,
                TotalTimeSpendInSeconds = totalSeconds,
                TotalTime = TimeSpan.FromSeconds(totalSeconds)
            };
        }
    }
}
