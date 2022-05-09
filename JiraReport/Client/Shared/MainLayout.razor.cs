using MudBlazor;

namespace JiraReport.Client.Shared
{
    public partial class MainLayout
    {
        private bool _isDarkMode;
        private MudThemeProvider _mudThemeProvider;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isDarkMode = await _mudThemeProvider.GetSystemPreference();
                StateHasChanged();
            }
        }
    }
}
