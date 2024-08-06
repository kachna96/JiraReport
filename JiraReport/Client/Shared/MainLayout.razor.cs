using Fluxor;
using JiraReport.Client.Store.Exceptions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using MudBlazor.Interfaces;

namespace JiraReport.Client.Shared
{
    public partial class MainLayout
    {
        private bool _isDarkMode;
        private MudThemeProvider _mudThemeProvider;

        [Inject]
        private IState<ExceptionsState> ExceptionsState { get; set; }

        [Inject]
        private IDispatcher Dispatcher { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isDarkMode = await _mudThemeProvider.GetSystemPreference();
                StateHasChanged();
            }

            ExceptionsState.StateChanged += OnExceptionStateChanged;
            NavigationManager.LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object _1, LocationChangedEventArgs _2)
        {
            if (ExceptionsState.Value.Exceptions.Count == 0)
            {
                return;
            }

            Dispatcher.Dispatch(new ResetExceptionsAction());
        }

        private void OnExceptionStateChanged(object _1, EventArgs _2)
        {
            (this as IMudStateHasChanged)?.StateHasChanged();
        }

        protected override async ValueTask DisposeAsyncCore(bool disposing)
        {
            if (disposing)
            {
                ExceptionsState.StateChanged -= OnExceptionStateChanged;
                NavigationManager.LocationChanged -= OnLocationChanged;
            }

            await base.DisposeAsyncCore(disposing);
        }
    }
}
