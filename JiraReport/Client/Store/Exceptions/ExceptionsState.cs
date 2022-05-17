using Fluxor;

namespace JiraReport.Client.Store.Exceptions
{
    [FeatureState]
    public record ExceptionsState
    {
        public List<Exception> Exceptions { get; set; } = new();

        private ExceptionsState() { }
    }
}
