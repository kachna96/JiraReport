using Blazored.LocalStorage;
using Fluxor;
using JiraReport.Client.Store.JiraIssues;
using JiraReport.Client.Store.JiraIssuesFilter;
using JiraReport.Client.Store.Translations;
using JiraReport.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace JiraReport.Client.Pages
{
    public partial class Index
    {
        bool success = true;
        bool firstValidation = true;
        MudForm form;

        private List<(string Key, double Hours)> ExtraHours { get; set; } = new();

        [Inject]
        private IState<JiraIssuesState> JiraIssuesState { get; set; }

        [Inject]
        private IState<JiraIssuesFilterState> JiraIssuesFilterState { get; set; }

        [Inject]
        private IState<TranslationsState> TranslationState { get; set; }

        [Inject]
        private ILocalStorageService LocalStorage { get; set; }

        [Inject]
        private IDispatcher Dispatcher { get; set; }

        [Inject]
        private ISnackbar Snackbar { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            JiraIssuesState.StateChanged += JiraIssuesState_StateChanged;
            JiraIssuesFilterState.StateChanged += JiraIssuesFilterState_StateChanged;
            Dispatcher.Dispatch(new FetchTranslationsAction());
            if (!JiraIssuesFilterState.Value.IsFilled())
            {
                await LoadFiltersFromLocalStorageAsync();
            }
            await base.OnInitializedAsync();
        }

        private void JiraIssuesFilterState_StateChanged(object? sender, EventArgs e)
        {
            if (firstValidation && JiraIssuesFilterState.Value.IsFilled())
            {
                firstValidation = false;
                form.Validate();
            }

            if (!JiraIssuesState.Value.Loading && JiraIssuesState.Value.Initialized)
            {
                CalculateHourMultiplier();
            }
        }

        private void JiraIssuesState_StateChanged(object? sender, EventArgs e)
        {
            if (!JiraIssuesState.Value.Loading && JiraIssuesState.Value.Initialized)
            {
                CalculateHourMultiplier();
            }
        }

        private void FetchJiraIssuesAsync()
        {
            Dispatcher.Dispatch(new FetchJiraIssuesSetLoadingAction(true));
            Dispatcher.Dispatch(new FetchJiraIssuesAction(JiraIssuesFilterState.Value.DateRange.Start.GetValueOrDefault(), JiraIssuesFilterState.Value.DateRange.End.GetValueOrDefault()));
        }

        private string GetTdClass(string key)
        {
            if (JiraIssuesState.Value.SelectedIssues.Any(x => x.Key == key))
            {
                return string.Empty;
            }

            return "strike-through";
        }

        private string GetBonusClass()
        {
            if (JiraIssuesState.Value.SelectedIssues.Any(x => x.Checked))
            {
                return string.Empty;
            }

            return "d-none";
        }

        private async Task SaveFiltersToLocalStorageAsync()
        {
            await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.Name), JiraIssuesFilterState.Value.Name);
            await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.ContractorId), JiraIssuesFilterState.Value.ContractorId);
            await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.TaxId), JiraIssuesFilterState.Value.TaxId);
            await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.Residence), JiraIssuesFilterState.Value.Residence);
            await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.HourRate), JiraIssuesFilterState.Value.HourRate);
            await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.BonusInAdvance), JiraIssuesFilterState.Value.BonusInAdvance);
            await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.OtherPayments), JiraIssuesFilterState.Value.OtherPayments);
            await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.ReportedHours), JiraIssuesFilterState.Value.ReportedHours);
            await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.SelectedCurrency), JiraIssuesFilterState.Value.SelectedCurrency);
            await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.Language), JiraIssuesFilterState.Value.Language);
            Snackbar.Add("Values saved to Local Storage.", Severity.Success);
        }

        private async Task LoadFiltersFromLocalStorageAsync()
        {
            var name = await LocalStorage.GetItemAsync<string>(nameof(JiraIssuesFilterState.Value.Name));
            var contractorId = await LocalStorage.GetItemAsync<string>(nameof(JiraIssuesFilterState.Value.ContractorId));
            var taxId = await LocalStorage.GetItemAsync<string>(nameof(JiraIssuesFilterState.Value.TaxId));
            var residence = await LocalStorage.GetItemAsync<string>(nameof(JiraIssuesFilterState.Value.Residence));
            var hourRate = await LocalStorage.GetItemAsync<decimal>(nameof(JiraIssuesFilterState.Value.HourRate));
            var bonusInAdvance = await LocalStorage.GetItemAsync<decimal>(nameof(JiraIssuesFilterState.Value.BonusInAdvance));
            var otherPayments = await LocalStorage.GetItemAsync<decimal>(nameof(JiraIssuesFilterState.Value.OtherPayments));
            var reportedHours = await LocalStorage.GetItemAsync<decimal>(nameof(JiraIssuesFilterState.Value.ReportedHours));
            var currency = await LocalStorage.GetItemAsync<string>(nameof(JiraIssuesFilterState.Value.SelectedCurrency));
            var language = await LocalStorage.GetItemAsync<string>(nameof(JiraIssuesFilterState.Value.Language));

            Dispatcher.Dispatch(new SetJiraIssuesFilterAction(name, contractorId, taxId, residence, hourRate, bonusInAdvance, otherPayments, currency, reportedHours, language));
        }

        private void CalculateHourMultiplier()
        {
            if (JiraIssuesState.Value.Loading || !JiraIssuesState.Value.Initialized)
            {
                return;
            }

            ExtraHours = new();
            var totalHours = Math.Round(JiraIssuesFilterState.Value.TotalPrice / JiraIssuesFilterState.Value.HourRate, 2);
            var buckets = HourRandomizer.SplitIntoBuckets((double)totalHours, JiraIssuesState.Value.SelectedIssues.Select(x => (x.Key, x.TimeSpendInHours)));

            double roundedHour = 0;
            foreach (var (key, hours) in buckets)
            {
                if (key == buckets.Last().key)
                {
                    roundedHour = (double)totalHours - ExtraHours.Sum(x => x.Hours);
                }
                else
                {
                    roundedHour = hours;
                }

                ExtraHours.Add(new(key, roundedHour));
            }

            StateHasChanged();
        }

        private void ItemHasBeenCommitted(object row)
        {
            if (row is JiraIssue jiraIssue)
            {
                Dispatcher.Dispatch(new JiraIssuesActionSetJiraIssues(jiraIssue));
            }
        }

        private void HourRateValueChanged(decimal value)
        {
            Dispatcher.Dispatch(new JiraIssuesFilterSetHourRateAction(value));
        }

        private void BonusInAdvanceValueChanged(decimal value)
        {
            Dispatcher.Dispatch(new JiraIssuesFilterSetBonusInAdvanceAction(value));
        }

        private void OtherPaymentsValueChanged(decimal value)
        {
            Dispatcher.Dispatch(new JiraIssuesFilterSetOtherPaymentsAction(value));
        }

        private void ReportedHoursValueChanged(decimal value)
        {
            Dispatcher.Dispatch(new JiraIssuesFilterSetReportedHoursAction(value));
        }

        private void SelectedItemsChanged(HashSet<JiraIssue> jiraIssues)
        {
            Dispatcher.Dispatch(new JiraIssuesActionSetSelectedIssues(jiraIssues));
        }

        private void DateRangeChanged(DateRange dateRange)
        {
            Dispatcher.Dispatch(new JiraIssuesFilterSetDateRangeAction(dateRange));

            if (!JiraIssuesState.Value.Loading && JiraIssuesState.Value.Initialized && form.IsValid)
            {
                Dispatcher.Dispatch(new FetchJiraIssuesSetLoadingAction(true));
                Dispatcher.Dispatch(new FetchJiraIssuesAction(JiraIssuesFilterState.Value.DateRange.Start.GetValueOrDefault(), JiraIssuesFilterState.Value.DateRange.End.GetValueOrDefault()));
            }
        }

        private async Task PrintAsync()
        {
            await JSRuntime.InvokeVoidAsync("print");
        }

        protected override void Dispose(bool disposing)
        {
            JiraIssuesState.StateChanged -= JiraIssuesState_StateChanged;
            JiraIssuesFilterState.StateChanged -= JiraIssuesFilterState_StateChanged;
            base.Dispose(disposing);
        }
    }
}
