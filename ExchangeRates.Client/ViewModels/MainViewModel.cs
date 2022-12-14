using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ExchangeRates.Client.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private string _selectedCurrency;
        private DateTime _startDate;
        private DateTime _endDate;

        public MainViewModel()
        {
            EndDate = DateTime.Now;
            StartDate = DateTime.Now.AddYears(-5);
            _selectedCurrency = _currencyOptions.First().Value;
        }

        public MainViewModel(string selectedCurrency, DateTime startDate, DateTime endDate)
        {
            EndDate = endDate;
            StartDate = startDate;
            SelectedCurrency = selectedCurrency;
        }

        public ObservableCollection<RateViewModel> Rates { get; set; } = new ObservableCollection<RateViewModel>();

        private readonly ObservableCollection<KeyValuePair<string, string>> _currencyOptions =
            new ObservableCollection<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("USD","USD"),
                    new KeyValuePair<string, string>("EUR","EUR"),
                    new KeyValuePair<string, string>("RUB","RUB"),
                    new KeyValuePair<string, string>("BTC","BTC")
                };

        public ObservableCollection<KeyValuePair<string, string>> CurrencyOptions
        {
            get => _currencyOptions;
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        public string SelectedCurrency
        {
            get => _selectedCurrency;
            set
            {
                _selectedCurrency = value;
                OnPropertyChanged(nameof(SelectedCurrency));
            }
        }
    }
}
