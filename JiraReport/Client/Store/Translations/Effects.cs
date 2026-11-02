using Fluxor;
using JiraReport.Client.Aspects;
using JiraReport.Client.Store.Exceptions;
using JiraReport.Shared;
using System.Net.Http.Json;

namespace JiraReport.Client.Store.Translations
{
	public class Effects : IExceptionStateLogger
	{
		private readonly HttpClient _httpClient;
		private readonly IDispatcher _dispatcher;

		public Effects(HttpClient httpClient, IDispatcher dispatcher)
		{
			_httpClient = httpClient;
			_dispatcher = dispatcher;
		}

		[EffectMethod]
		public async Task HandleFetchDataAction(FetchTranslationsAction _, IDispatcher dispatcher)
		{
			try
			{
				var issueCollection = await _httpClient.GetFromJsonAsync<ReportTranslationCollection>("api/jiraissues/translations");
				dispatcher.Dispatch(new FetchTranslationsResultAction(issueCollection));
			}
			catch (Exception ex)
			{
				Log(ex);
			}
		}

		public void Log(Exception exception)
		{
			_dispatcher.Dispatch(new AddExceptionAction(exception));
		}
	}
}
