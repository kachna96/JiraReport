namespace JiraReport.Client.Store.Exceptions
{
    public class AddExceptionAction
    {
        public Exception Exception { get; }

        public AddExceptionAction(Exception exception)
        {
            Exception = exception;
        }
    }
}
