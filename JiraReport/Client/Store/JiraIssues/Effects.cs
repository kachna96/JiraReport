using Fluxor;
using JiraReport.Shared;
using System.Net.Http.Json;

namespace JiraReport.Client.Store.JiraIssues
{
    public class Effects
    {
        private readonly HttpClient _httpClient;

        public Effects(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [EffectMethod]
        public async Task HandleFetchDataAction(FetchJiraIssuesAction action, IDispatcher dispatcher)
        {
            var issueCollection = await _httpClient.GetFromJsonAsync<JiraIssueCollection>($"api/jiraissues?from={action.From}&to={action.To}");
            dispatcher.Dispatch(new FetchJiraIssuesResultAction(issueCollection));
        }
    }
}
