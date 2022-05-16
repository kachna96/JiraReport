using Fluxor;
using JiraReport.Shared;

namespace JiraReport.Client.Store.Translations
{
	[FeatureState]
	public record TranslationsState
	{
		public ReportTranslationCollection TranslationCollection { get; init; }

		public bool Loading { get; init; } = true;

		public bool Initialized => TranslationCollection.Translations.Any();

		private TranslationsState() { }

		public TranslationsState(ReportTranslationCollection translationCollection)
		{
			TranslationCollection = translationCollection;
		}
	}
}
