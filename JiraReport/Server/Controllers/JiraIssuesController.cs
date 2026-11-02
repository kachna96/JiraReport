using System.Globalization;
using Atlassian.Jira;
using JiraReport.Server.Mappers;
using JiraReport.Server.Options;
using JiraReport.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JiraReport.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class JiraIssuesController : ControllerBase
	{
		private readonly IIssueService _issueService;
		private readonly IOptionsMonitor<JiraConfigurationOptions> _jiraOptions;
		private readonly ReportTranslationCollection _reportTranslationCollection;

		public JiraIssuesController(IIssueService issueService, IOptionsMonitor<JiraConfigurationOptions> jiraOptions, ReportTranslationCollection reportTranslationCollection)
		{
			_issueService = issueService;
			_jiraOptions = jiraOptions;
			_reportTranslationCollection = reportTranslationCollection;
		}

		[HttpGet]
		[ProducesResponseType(typeof(JiraIssueCollection), 200)]
		[ProducesResponseType(typeof(string), 400)]
		public async Task<ActionResult<JiraIssueCollection>> Get(string from, string to, CancellationToken cancellationToken = default)
		{
			if (!DateTime.TryParseExact(from, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime fromDate) ||
				!DateTime.TryParseExact(to, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime toDate))
			{
				return BadRequest("Date time is not in correct format. Expected format: yyyy/MM/dd");
			}

			var username = _jiraOptions.CurrentValue.Username;
			var baseUri = _jiraOptions.CurrentValue.BaseUri;

			var issues = await _issueService.GetIssuesFromJqlAsync(
				$"worklogAuthor = '{username}' and worklogDate >= '{fromDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture)}' and worklogDate <= '{toDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture)}'",
				maxIssues: 500,
				token: cancellationToken);

			using var throttler = new SemaphoreSlim(8);
			var tasks = issues.Select(async issue =>
			{
				await throttler.WaitAsync(cancellationToken);
				try
				{
					var worklog = await issue.GetWorklogsAsync(cancellationToken);
					var secondsSpendSum = worklog
						.Where(x => string.Equals(x.Author, username, StringComparison.OrdinalIgnoreCase))
						.Where(x => x.StartDate.HasValue && x.StartDate.Value.Date >= fromDate.Date && x.StartDate.Value.Date <= toDate.Date)
						.Sum(x => x.TimeSpentInSeconds);

					var mappedIssue = issue.ToJiraIssue();
					mappedIssue.TimeSpendInSeconds = secondsSpendSum;
					mappedIssue.TimeSpend = TimeSpan.FromSeconds(secondsSpendSum);
					if (baseUri != null)
					{
						mappedIssue.SetUri(baseUri);
					}

					return mappedIssue;
				}
				finally
				{
					throttler.Release();
				}
			});

			var resultIssues = await Task.WhenAll(tasks);
			return Ok(resultIssues.ToCollection());
		}

		[HttpGet("translations")]
		[ProducesResponseType(typeof(ReportTranslationCollection), 200)]
		public ActionResult<ReportTranslationCollection> GetTranslations()
		{
			return Ok(_reportTranslationCollection);
		}
	}
}
