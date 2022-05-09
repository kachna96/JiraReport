using Blazored.LocalStorage;
using Fluxor;
using JiraReport.Client.Store.JiraIssues;
using JiraReport.Client.Store.JiraIssuesFilter;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace JiraReport.Client.Pages
{
	public partial class Index
	{
		public string SelectedOption { get; set; }
		bool success = true;
		string[] errors = Array.Empty<string>();
		public decimal BonusValue { get; set; }
		bool c;
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

		protected override async Task OnInitializedAsync()
		{
			await LoadFiltersFromLocalStorageAsync();
			JiraIssuesState.StateChanged += JiraIssuesState_StateChanged;
			await base.OnInitializedAsync();
		}

		private void JiraIssuesState_StateChanged(object? sender, EventArgs e)
		{
			if (!JiraIssuesState.Value.Loading && JiraIssuesState.Value.Initialized)
			{
				CalculateHourMultiplier();
				StateHasChanged();
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
			await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.ReportedHours), JiraIssuesFilterState.Value.ReportedHours);
			await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.RoundingDecimals), JiraIssuesFilterState.Value.RoundingDecimals);
			await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.HourRate), JiraIssuesFilterState.Value.HourRate);
			await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.TotalPrice), JiraIssuesFilterState.Value.TotalPrice);
			await LocalStorage.SetItemAsync(nameof(JiraIssuesFilterState.Value.SelectedCurrency), JiraIssuesFilterState.Value.SelectedCurrency);
		}

		private async Task LoadFiltersFromLocalStorageAsync()
		{
			var name = await LocalStorage.GetItemAsync<string>(nameof(JiraIssuesFilterState.Value.Name));
			var contractorId = await LocalStorage.GetItemAsync<string>(nameof(JiraIssuesFilterState.Value.ContractorId));
			var taxId = await LocalStorage.GetItemAsync<string>(nameof(JiraIssuesFilterState.Value.TaxId));
			var residence = await LocalStorage.GetItemAsync<string>(nameof(JiraIssuesFilterState.Value.Residence));
			var reportedHours = await LocalStorage.GetItemAsync<decimal>(nameof(JiraIssuesFilterState.Value.ReportedHours));
			var roundingDecimals = await LocalStorage.GetItemAsync<int>(nameof(JiraIssuesFilterState.Value.RoundingDecimals));
			var hourRate = await LocalStorage.GetItemAsync<decimal>(nameof(JiraIssuesFilterState.Value.HourRate));
			var totalPrice = await LocalStorage.GetItemAsync<decimal>(nameof(JiraIssuesFilterState.Value.TotalPrice));
			var currency = await LocalStorage.GetItemAsync<string>(nameof(JiraIssuesFilterState.Value.SelectedCurrency));

			Dispatcher.Dispatch(new SetJiraIssuesFilterAction(name, contractorId, taxId, residence, reportedHours, roundingDecimals, hourRate, totalPrice, currency));
		}

		private void CalculateHourMultiplier()
		{
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
		}

		protected override void Dispose(bool disposing)
		{
			JiraIssuesState.StateChanged -= JiraIssuesState_StateChanged;
			base.Dispose(disposing);
		}
	}
}
