using Blazored.LocalStorage;
using Fluxor;
using JiraReport.Client.Store.JiraIssues;
using JiraReport.Client.Store.JiraIssuesFilter;
using JiraReport.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace JiraReport.Client.Pages
{
	public partial class Index
	{
		bool success = true;
		string[] errors = Array.Empty<string>();
		MudForm form;

		private List<(string Key, double Hours)> ExtraHours { get; set; } = new();

		[Inject]
		private IState<JiraIssuesState> JiraIssuesState { get; set; }

		[Inject]
		private IState<JiraIssuesFilterState> JiraIssuesFilterState { get; set; }

		[Inject]
		private ILocalStorageService LocalStorage { get; set; }

		[Inject]
		private IDispatcher Dispatcher { get; set; }

		[Inject]
		private ISnackbar Snackbar { get; set; }

		protected override async Task OnInitializedAsync()
		{
			await LoadFiltersFromLocalStorageAsync();
			JiraIssuesState.StateChanged += JiraIssuesState_StateChanged;
			JiraIssuesFilterState.StateChanged += JiraIssuesFilterState_StateChanged;
			await base.OnInitializedAsync();
		}

		private void JiraIssuesFilterState_StateChanged(object? sender, EventArgs e)
		{
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

		private async Task SaveFiltersToLocalStorageAsync()
		{
			await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.Name), JiraIssuesFilterState.Value.Name);
			await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.ContractorId), JiraIssuesFilterState.Value.ContractorId);
			await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.TaxId), JiraIssuesFilterState.Value.TaxId);
			await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.Residence), JiraIssuesFilterState.Value.Residence);
			await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.HourRate), JiraIssuesFilterState.Value.HourRate);
			await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.TotalPrice), JiraIssuesFilterState.Value.TotalPrice);
			await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.SelectedCurrency), JiraIssuesFilterState.Value.SelectedCurrency);
			Snackbar.Add("Values saved to Local Storage.", Severity.Success);
		}

		private async Task LoadFiltersFromLocalStorageAsync()
		{
			var name = await LocalStorage.GetItemAsync<string>(nameof(JiraIssuesFilterState.Value.Name));
			var contractorId = await LocalStorage.GetItemAsync<string>(nameof(JiraIssuesFilterState.Value.ContractorId));
			var taxId = await LocalStorage.GetItemAsync<string>(nameof(JiraIssuesFilterState.Value.TaxId));
			var residence = await LocalStorage.GetItemAsync<string>(nameof(JiraIssuesFilterState.Value.Residence));
			var hourRate = await LocalStorage.GetItemAsync<decimal>(nameof(JiraIssuesFilterState.Value.HourRate));
			var totalPrice = await LocalStorage.GetItemAsync<decimal>(nameof(JiraIssuesFilterState.Value.TotalPrice));
			var currency = await LocalStorage.GetItemAsync<string>(nameof(JiraIssuesFilterState.Value.SelectedCurrency));

			Dispatcher.Dispatch(new SetJiraIssuesFilterAction(name, contractorId, taxId, residence, hourRate, totalPrice, currency));
		}

		private void CalculateHourMultiplier()
		{
			if (JiraIssuesState.Value.Loading || !JiraIssuesState.Value.Initialized)
			{
				return;
			}

			ExtraHours = new();
			var totalHours = JiraIssuesFilterState.Value.TotalPrice / JiraIssuesFilterState.Value.HourRate;
			var reportedHours = JiraIssuesState.Value.SelectedIssues.Sum(x => x.TimeSpendInHours);
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

		private void TotalPriceValueChanged(decimal value)
		{
			Dispatcher.Dispatch(new JiraIssuesFilterSetTotalPriceAction(value));
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

		protected override void Dispose(bool disposing)
		{
			JiraIssuesState.StateChanged -= JiraIssuesState_StateChanged;
			base.Dispose(disposing);
		}
	}
}
