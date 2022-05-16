using JiraReport.Shared;

namespace JiraReport.Client.Store.Translations
{
	public class FetchTranslationsResultAction
	{
		public ReportTranslationCollection TranslationCollection { get; set; }

		public FetchTranslationsResultAction(ReportTranslationCollection issueCollection)
		{
			TranslationCollection = issueCollection;
		}
	}
}
