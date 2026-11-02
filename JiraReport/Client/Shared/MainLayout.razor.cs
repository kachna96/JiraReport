using Blazored.LocalStorage;
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
        private const string DarkModeStorageKey = "isDarkMode";
        private bool _isDarkMode;
        private MudThemeProvider? _mudThemeProvider;

        [Inject]
        private IState<ExceptionsState> ExceptionsState { get; set; } = null!;

        [Inject]
        private IDispatcher Dispatcher { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        private ILocalStorageService LocalStorage { get; set; } = null!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var storedDarkMode = await LocalStorage.GetItemAsync<bool?>(DarkModeStorageKey);
                if (storedDarkMode.HasValue)
                {
                    _isDarkMode = storedDarkMode.Value;
                }
                else if (_mudThemeProvider != null)
                {
                    _isDarkMode = await _mudThemeProvider.GetSystemDarkModeAsync();
                }

                if (_mudThemeProvider != null)
                {
                    await _mudThemeProvider.WatchSystemDarkModeAsync(OnSystemDarkModeChanged);
                }

                StateHasChanged();

                ExceptionsState.StateChanged += OnExceptionStateChanged;
                NavigationManager.LocationChanged += OnLocationChanged;
            }
        }

        private async Task DarkModeToggle()
        {
            _isDarkMode = !_isDarkMode;
            await LocalStorage.SetItemAsync(DarkModeStorageKey, _isDarkMode);
        }

        private async Task OnSystemDarkModeChanged(bool isDarkMode)
        {
            var storedDarkMode = await LocalStorage.GetItemAsync<bool?>(DarkModeStorageKey);
            if (!storedDarkMode.HasValue)
            {
                _isDarkMode = isDarkMode;
                await InvokeAsync(StateHasChanged);
            }
        }

        private void OnLocationChanged(object? _1, LocationChangedEventArgs _2)
        {
            if (ExceptionsState.Value.Exceptions.Count == 0)
            {
                return;
            }

            Dispatcher.Dispatch(new ResetExceptionsAction());
        }

        private void OnExceptionStateChanged(object? _1, EventArgs _2)
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
