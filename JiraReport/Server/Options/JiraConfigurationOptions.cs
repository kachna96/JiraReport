namespace JiraReport.Server.Options
{
	public class JiraConfigurationOptions
	{
		public const string Section = "Jira";

		public string Username { get; init; } = string.Empty;

		public string Password { get; init; } = string.Empty;

		public Uri? BaseUri { get; init; }
	}
}
