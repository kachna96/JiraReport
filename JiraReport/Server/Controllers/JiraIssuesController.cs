﻿using Atlassian.Jira;
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

		public JiraIssuesController(IIssueService issueService, IMapper mapper, IOptionsMonitor<JiraConfigurationOptions> jiraOptions)
		{
			_issueService = issueService;
			_mapper = mapper;
			_jiraOptions = jiraOptions;
		}

		[HttpGet]
		[ProducesResponseType(typeof(JiraIssueCollection), 200)]
		public async Task<JiraIssueCollection> Get(DateTime from, DateTime to, CancellationToken cancellationToken = default)
		{
			var resultIssues = new List<JiraIssue>();
			var issues = await _issueService.GetIssuesFromJqlAsync(
				$"worklogAuthor = '{_jiraOptions.CurrentValue.Username}' and worklogDate >= '{from.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture)}' and worklogDate <= '{to.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture)}'",
				token: cancellationToken);

			foreach (var issue in issues)
			{
				var worklog = await issue.GetWorklogsAsync(cancellationToken);
				var secondsSpendSum = worklog
					.Where(x => x.Author == _jiraOptions.CurrentValue.Username)
					.Where(x => x.StartDate >= from)
					.Where(x => x.StartDate <= to)
					.Sum(x => x.TimeSpentInSeconds);

				var mappedIssue = _mapper.Map<JiraIssue>(issue);
				mappedIssue.TimeSpendInSeconds = secondsSpendSum;
				mappedIssue.TimeSpend = TimeSpan.FromSeconds(secondsSpendSum);
				mappedIssue.SetUri(_jiraOptions.CurrentValue.BaseUri);

				resultIssues.Add(mappedIssue);
			}

			return _mapper.Map<JiraIssueCollection>(resultIssues);
		}
	}
}
