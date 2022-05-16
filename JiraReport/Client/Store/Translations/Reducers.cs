using Fluxor;

namespace JiraReport.Client.Store.Translations
{
	public class Reducers
	{
		[ReducerMethod]
		public static TranslationsState ReduceFetchTranslationsResultAction(TranslationsState state, FetchTranslationsResultAction action) =>
			state with
			{
				TranslationCollection = action.TranslationCollection,
				Loading = false
			};
	}
}
