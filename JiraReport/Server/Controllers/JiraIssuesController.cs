using Atlassian.Jira;
using AutoMapper;
using JiraReport.Server.Options;
using JiraReport.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace JiraReport.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class JiraIssuesController : ControllerBase
	{
		private readonly IIssueService _issueService;
		private readonly IMapper _mapper;
		private readonly IOptionsMonitor<JiraConfigurationOptions> _jiraOptions;
		private readonly ReportTranslationCollection _reportTranslationCollection;

		public JiraIssuesController(IIssueService issueService, IMapper mapper, IOptionsMonitor<JiraConfigurationOptions> jiraOptions, ReportTranslationCollection reportTranslationCollection)
		{
			_issueService = issueService;
			_mapper = mapper;
			_jiraOptions = jiraOptions;
			_reportTranslationCollection = reportTranslationCollection;
		}

		[HttpGet]
		[ProducesResponseType(typeof(JiraIssueCollection), 200)]
		public async Task<JiraIssueCollection> Get(string from, string to, CancellationToken cancellationToken = default)
		{
			if (!DateTime.TryParse(from, out DateTime fromDate) || !DateTime.TryParse(to, out DateTime toDate))
			{
				throw new ArgumentException("Date time is not in correct format");
			}

			var resultIssues = new List<JiraIssue>();
			var issues = await _issueService.GetIssuesFromJqlAsync(
				$"worklogAuthor = '{_jiraOptions.CurrentValue.Username}' and worklogDate >= '{fromDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture)}' and worklogDate <= '{toDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture)}'",
				token: cancellationToken);

			foreach (var issue in issues)
			{
				var worklog = await issue.GetWorklogsAsync(cancellationToken);
				var secondsSpendSum = worklog
					.Where(x => x.Author == _jiraOptions.CurrentValue.Username)
					.Where(x => x.StartDate >= fromDate)
					.Where(x => x.StartDate <= toDate)
					.Sum(x => x.TimeSpentInSeconds);

				var mappedIssue = _mapper.Map<JiraIssue>(issue);
				mappedIssue.TimeSpendInSeconds = secondsSpendSum;
				mappedIssue.TimeSpend = TimeSpan.FromSeconds(secondsSpendSum);
				mappedIssue.SetUri(_jiraOptions.CurrentValue.BaseUri);

				resultIssues.Add(mappedIssue);
			}

			return _mapper.Map<JiraIssueCollection>(resultIssues);
		}

		[HttpGet("translations")]
		[ProducesResponseType(typeof(ReportTranslationCollection), 200)]
		public ReportTranslationCollection GetTranslations()
		{
			return _reportTranslationCollection;
		}
	}
}
