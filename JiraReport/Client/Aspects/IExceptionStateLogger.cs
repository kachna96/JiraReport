namespace JiraReport.Client.Aspects
{
    public interface IExceptionStateLogger
    {
        void Log(Exception exception);
    }
}
