using Fluxor;
using JiraReport.Shared;
using System.Net.Http.Json;

namespace JiraReport.Client.Store.Translations
{
	public class Effects
    {
        private readonly HttpClient _httpClient;

        public Effects(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [EffectMethod]
        public async Task HandleFetchDataAction(FetchTranslationsAction _, IDispatcher dispatcher)
        {
            var issueCollection = await _httpClient.GetFromJsonAsync<ReportTranslationCollection>($"api/jiraissues/translations");
            dispatcher.Dispatch(new FetchTranslationsResultAction(issueCollection));
        }
    }
}
