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
        MudForm? form;

        private List<(string Key, double Hours)> ExtraHours { get; set; } = [];
        private Dictionary<string, double> ExtraHoursDict { get; set; } = new();

        [Inject]
        private IState<JiraIssuesState> JiraIssuesState { get; set; } = null!;

        [Inject]
        private IState<JiraIssuesFilterState> JiraIssuesFilterState { get; set; } = null!;

        [Inject]
        private IState<TranslationsState> TranslationState { get; set; } = null!;

        [Inject]
        private ILocalStorageService LocalStorage { get; set; } = null!;

        [Inject]
        private IDispatcher Dispatcher { get; set; } = null!;

        [Inject]
        private ISnackbar Snackbar { get; set; } = null!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = null!;

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
            if (firstValidation && JiraIssuesFilterState.Value.IsFilled() && form != null)
            {
                firstValidation = false;
                _ = form.ValidateAsync();
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
            Dispatcher.Dispatch(new FetchJiraIssuesAction(
                JiraIssuesFilterState.Value.DateRange.Start.GetValueOrDefault(),
                JiraIssuesFilterState.Value.DateRange.End.GetValueOrDefault()));
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

        public double GetExtraHours(string? key)
        {
            if (string.IsNullOrEmpty(key)) return 0;
            return ExtraHoursDict.TryGetValue(key, out var hours) ? hours : 0;
        }

        public double GetTotalPaddedHours()
        {
            var selected = JiraIssuesState.Value.SelectedIssues;
            if (selected == null || !selected.Any()) return 0;
            return selected.Sum(x => GetExtraHours(x.Key));
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

            Dispatcher.Dispatch(new SetJiraIssuesFilterAction(
                name ?? string.Empty,
                contractorId ?? string.Empty,
                taxId ?? string.Empty,
                residence ?? string.Empty,
                hourRate,
                bonusInAdvance,
                otherPayments,
                currency ?? string.Empty,
                reportedHours,
                language ?? string.Empty));
        }

        private void CalculateHourMultiplier()
        {
            if (JiraIssuesState.Value.Loading || !JiraIssuesState.Value.Initialized)
            {
                return;
            }

            var selectedIssues = JiraIssuesState.Value.SelectedIssues;
            if (JiraIssuesFilterState.Value.HourRate <= 0 || selectedIssues == null || !selectedIssues.Any())
            {
                ExtraHours = [];
                ExtraHoursDict = new();
                StateHasChanged();
                return;
            }

            var totalHours = Math.Round(JiraIssuesFilterState.Value.TotalPrice / JiraIssuesFilterState.Value.HourRate, 2);
            var selectedList = selectedIssues.ToList();
            var totalLogged = selectedList.Sum(x => x.TimeSpendInHours);

            if (totalLogged <= 0)
            {
                ExtraHours = selectedList.Select(x => (x.Key, 0.0)).ToList();
                ExtraHoursDict = selectedList.ToDictionary(x => x.Key, _ => 0.0);
                StateHasChanged();
                return;
            }

            var buckets = HourRandomizer.SplitIntoBuckets((double)totalHours, selectedList.Select(x => (x.Key, x.TimeSpendInHours)));

            var newExtraHours = new List<(string Key, double Hours)>(buckets.Count);
            var dict = new Dictionary<string, double>(buckets.Count);
            double accumulated = 0;

            for (int i = 0; i < buckets.Count; i++)
            {
                var (key, hours) = buckets[i];
                double roundedHour;
                if (i == buckets.Count - 1)
                {
                    roundedHour = (double)totalHours - accumulated;
                }
                else
                {
                    roundedHour = hours;
                    accumulated += roundedHour;
                }

                newExtraHours.Add((key, roundedHour));
                dict[key] = roundedHour;
            }

            ExtraHours = newExtraHours;
            ExtraHoursDict = dict;
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

            if (!JiraIssuesState.Value.Loading && JiraIssuesState.Value.Initialized && (form?.IsValid ?? false))
            {
                Dispatcher.Dispatch(new FetchJiraIssuesSetLoadingAction(true));
                Dispatcher.Dispatch(new FetchJiraIssuesAction(
                    JiraIssuesFilterState.Value.DateRange.Start.GetValueOrDefault(),
                    JiraIssuesFilterState.Value.DateRange.End.GetValueOrDefault()));
            }
        }

        private async Task PrintAsync()
        {
            await JSRuntime.InvokeVoidAsync("print");
        }

        private static ReportTranslation GetDefaultTranslation() => new()
        {
            Headline = "Seznam objednávek",
            Name = "Jméno dodavatele",
            ContractorId = "IČO",
            TaxId = "DIČ",
            Residence = "Sídlo",
            BillingPeriod = "Fakturované období",
            BillingPeriodHourRate = "Smluvená sazba pro fakturované období",
            OrderNumber = "Číslo objednávky",
            OrderName = "Jméno objednávky",
            TimeSpentOnOrder = "Čas strávený na objednávce",
            TotalCostOfOrder = "Celková suma za danou objednávku",
            Bonus = "Bonus",
            Total = "Celkem"
        };

        protected override async ValueTask DisposeAsyncCore(bool disposing)
        {
            JiraIssuesState.StateChanged -= JiraIssuesState_StateChanged;
            JiraIssuesFilterState.StateChanged -= JiraIssuesFilterState_StateChanged;
            await base.DisposeAsyncCore(disposing);
        }
    }
}
