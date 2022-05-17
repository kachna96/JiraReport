using Fluxor;
using JiraReport.Client.Aspects;
using JiraReport.Client.Store.Exceptions;
using JiraReport.Shared;
using System.Net.Http.Json;

namespace JiraReport.Client.Store.JiraIssues
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
        [ExceptionStateBind]
        public async Task HandleFetchDataAction(FetchJiraIssuesAction action, IDispatcher dispatcher)
        {
            var issueCollection = await _httpClient.GetFromJsonAsync<JiraIssueCollection>($"api/jiraissues?from={action.From}&to={action.To}");
            dispatcher.Dispatch(new FetchJiraIssuesResultAction(issueCollection));
        }

        public void Log(Exception exception)
        {
            _dispatcher.Dispatch(new AddExceptionAction(exception));
        }
    }
}
