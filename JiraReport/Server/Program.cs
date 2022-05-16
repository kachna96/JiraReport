using Atlassian.Jira.AspNetCore;
using JiraReport.Server.Options;
using JiraReport.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
var translationCollection = new ReportTranslationCollection();
builder.Configuration.Bind(translationCollection);
builder.Services.AddSingleton(translationCollection);
var jiraOptions = new JiraConfigurationOptions();
var jiraOptionsSection = builder.Configuration.GetSection(JiraConfigurationOptions.Section);
builder.Services.Configure<JiraConfigurationOptions>(jiraOptionsSection);
jiraOptionsSection.Bind(jiraOptions);
builder.Services.AddJiraWithBasicAuth(new JiraWithBasicAuthOptions()
{
	BaseUri = jiraOptions.BaseUri,
	Username = jiraOptions.Username,
	Password = jiraOptions.Password
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
