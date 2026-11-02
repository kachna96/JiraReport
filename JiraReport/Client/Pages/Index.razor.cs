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

        private Dictionary<string, decimal> _extraHoursDict = new();
        private HashSet<string> _selectedKeys = new();
        private decimal _totalPaddedHours;
        private (double TimeSpendInHours, bool Checked) _editBackup;

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
            var start = JiraIssuesFilterState.Value.DateRange.Start;
            var end = JiraIssuesFilterState.Value.DateRange.End;
            if (!start.HasValue || !end.HasValue)
            {
                return;
            }

            Dispatcher.Dispatch(new FetchJiraIssuesSetLoadingAction(true));
            Dispatcher.Dispatch(new FetchJiraIssuesAction(start.Value, end.Value));
        }

        private bool IsIssueSelected(string? key)
        {
            return !string.IsNullOrEmpty(key) && _selectedKeys.Contains(key);
        }

        private string GetTdClass(string? key)
        {
            return IsIssueSelected(key) ? string.Empty : "strike-through";
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
            return _extraHoursDict.TryGetValue(key, out var hours) ? (double)hours : 0;
        }

        public decimal GetExtraPrice(string? key)
        {
            if (string.IsNullOrEmpty(key) || !_extraHoursDict.TryGetValue(key, out var hours))
            {
                return 0m;
            }

            return Math.Round(hours * JiraIssuesFilterState.Value.HourRate, 2, MidpointRounding.AwayFromZero);
        }

        public double GetTotalPaddedHours()
        {
            return (double)_totalPaddedHours;
        }

        public decimal GetTotalPrice()
        {
            return Math.Round(_totalPaddedHours * JiraIssuesFilterState.Value.HourRate, 2, MidpointRounding.AwayFromZero);
        }

        private async Task SaveFiltersToLocalStorageAsync()
        {
            var filter = JiraIssuesFilterState.Value;
            await Task.WhenAll(
                LocalStorage.SetItemAsync(nameof(filter.Name), filter.Name).AsTask(),
                LocalStorage.SetItemAsync(nameof(filter.ContractorId), filter.ContractorId).AsTask(),
                LocalStorage.SetItemAsync(nameof(filter.TaxId), filter.TaxId).AsTask(),
                LocalStorage.SetItemAsync(nameof(filter.Residence), filter.Residence).AsTask(),
                LocalStorage.SetItemAsync(nameof(filter.HourRate), filter.HourRate).AsTask(),
                LocalStorage.SetItemAsync(nameof(filter.BonusInAdvance), filter.BonusInAdvance).AsTask(),
                LocalStorage.SetItemAsync(nameof(filter.OtherPayments), filter.OtherPayments).AsTask(),
                LocalStorage.SetItemAsync(nameof(filter.ReportedHours), filter.ReportedHours).AsTask(),
                LocalStorage.SetItemAsync(nameof(filter.SelectedCurrency), filter.SelectedCurrency).AsTask(),
                LocalStorage.SetItemAsync(nameof(filter.Language), filter.Language).AsTask());

            Snackbar.Add("Values saved to Local Storage.", Severity.Success);
        }

        private async Task LoadFiltersFromLocalStorageAsync()
        {
            var filter = JiraIssuesFilterState.Value;
            var nameTask = LocalStorage.GetItemAsync<string>(nameof(filter.Name)).AsTask();
            var contractorIdTask = LocalStorage.GetItemAsync<string>(nameof(filter.ContractorId)).AsTask();
            var taxIdTask = LocalStorage.GetItemAsync<string>(nameof(filter.TaxId)).AsTask();
            var residenceTask = LocalStorage.GetItemAsync<string>(nameof(filter.Residence)).AsTask();
            var hourRateTask = LocalStorage.GetItemAsync<decimal>(nameof(filter.HourRate)).AsTask();
            var bonusInAdvanceTask = LocalStorage.GetItemAsync<decimal>(nameof(filter.BonusInAdvance)).AsTask();
            var otherPaymentsTask = LocalStorage.GetItemAsync<decimal>(nameof(filter.OtherPayments)).AsTask();
            var reportedHoursTask = LocalStorage.GetItemAsync<decimal>(nameof(filter.ReportedHours)).AsTask();
            var currencyTask = LocalStorage.GetItemAsync<string>(nameof(filter.SelectedCurrency)).AsTask();
            var languageTask = LocalStorage.GetItemAsync<string>(nameof(filter.Language)).AsTask();

            await Task.WhenAll(
                nameTask,
                contractorIdTask,
                taxIdTask,
                residenceTask,
                hourRateTask,
                bonusInAdvanceTask,
                otherPaymentsTask,
                reportedHoursTask,
                currencyTask,
                languageTask);

            Dispatcher.Dispatch(new SetJiraIssuesFilterAction(
                nameTask.Result ?? string.Empty,
                contractorIdTask.Result ?? string.Empty,
                taxIdTask.Result ?? string.Empty,
                residenceTask.Result ?? string.Empty,
                hourRateTask.Result,
                bonusInAdvanceTask.Result,
                otherPaymentsTask.Result,
                currencyTask.Result ?? string.Empty,
                reportedHoursTask.Result,
                languageTask.Result ?? string.Empty));
        }

        private void CalculateHourMultiplier()
        {
            if (JiraIssuesState.Value.Loading || !JiraIssuesState.Value.Initialized)
            {
                return;
            }

            var selectedIssues = JiraIssuesState.Value.SelectedIssues;
            if (selectedIssues == null || selectedIssues.Count == 0)
            {
                _selectedKeys = new();
                _extraHoursDict = new();
                _totalPaddedHours = 0m;
                return;
            }

            var selectedKeys = new HashSet<string>(selectedIssues.Count);
            var weights = new List<(string key, decimal hours)>(selectedIssues.Count);
            foreach (var issue in selectedIssues)
            {
                selectedKeys.Add(issue.Key);
                weights.Add((issue.Key, (decimal)issue.TimeSpendInHours));
            }
            _selectedKeys = selectedKeys;

            if (JiraIssuesFilterState.Value.HourRate <= 0)
            {
                _extraHoursDict = new();
                _totalPaddedHours = 0m;
                return;
            }

            var totalHours = Math.Round(
                JiraIssuesFilterState.Value.TotalPrice / JiraIssuesFilterState.Value.HourRate,
                2,
                MidpointRounding.AwayFromZero);

            var buckets = HourRandomizer.SplitIntoBuckets(totalHours, weights);

            var dict = new Dictionary<string, decimal>(buckets.Count);
            decimal sumHours = 0m;
            foreach (var (key, hours) in buckets)
            {
                if (dict.TryGetValue(key, out var existing))
                {
                    dict[key] = existing + hours;
                }
                else
                {
                    dict[key] = hours;
                }
                sumHours += hours;
            }

            _extraHoursDict = dict;
            _totalPaddedHours = Math.Round(sumHours, 2, MidpointRounding.AwayFromZero);
        }

        private void BackupItem(object row)
        {
            if (row is JiraIssue jiraIssue)
            {
                _editBackup = (jiraIssue.TimeSpendInHours, jiraIssue.Checked);
            }
        }

        private void ResetItemToOriginalValues(object row)
        {
            if (row is JiraIssue jiraIssue)
            {
                jiraIssue.TimeSpendInHours = _editBackup.TimeSpendInHours;
                jiraIssue.Checked = _editBackup.Checked;
            }
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

            if (!JiraIssuesState.Value.Loading
                && JiraIssuesState.Value.Initialized
                && (form?.IsValid ?? false)
                && dateRange.Start.HasValue
                && dateRange.End.HasValue)
            {
                Dispatcher.Dispatch(new FetchJiraIssuesSetLoadingAction(true));
                Dispatcher.Dispatch(new FetchJiraIssuesAction(dateRange.Start.Value, dateRange.End.Value));
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
