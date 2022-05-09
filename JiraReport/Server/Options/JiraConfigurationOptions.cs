namespace JiraReport.Server.Options
{
	public class JiraConfigurationOptions
	{
		public const string Section = "Jira";

		public string Username { get; init; }

		public string Password { get; init; }

		public Uri BaseUri { get; init; }
	}
}
