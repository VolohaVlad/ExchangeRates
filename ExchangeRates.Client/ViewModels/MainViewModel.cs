using ExchangeRates.Client.Models;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace ExchangeRates.Client.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private string _selectedCurrency;
        private DateTime _startDate;
        private DateTime _endDate;
        private IList<RateViewModel> _rates = new List<RateViewModel>();

        public SeriesCollection SeriesCollection { get; private set; }
        public Func<double, string> YFormatter { get; private set; }
        public Func<double, string> XFormatter { get; private set; }
        public double Max { get; private set; }
        public double Min { get; private set; }
        public string YAxisName { get; private set; }

        public IEnumerable<KeyValuePair<string, string>> CurrencyOptions => 
            new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("USD","USD"),
                new KeyValuePair<string, string>("EUR","EUR"),
                new KeyValuePair<string, string>("RUB","RUB"),
                new KeyValuePair<string, string>("BTC","BTC")
            };

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

        public MainViewModel(string selectedCurrency, DateTime startDate, DateTime endDate)
        {
            EndDate = endDate;
            StartDate = startDate;
            SelectedCurrency = selectedCurrency;

            var dayConfig = Mappers.Xy<RateViewModel>()
                  .X(dayModel => dayModel.Date.Ticks)
                  .Y(dayModel => dayModel.Value)
                  .Fill(item => item.Value == Max || item.Value == Min
                                    ? new SolidColorBrush(Color.FromRgb(238, 83, 80))
                                    : default)
                  .Stroke(item => item.Value == Max || item.Value == Min
                                    ? new SolidColorBrush(Color.FromRgb(238, 83, 80))
                                    : default);

            SeriesCollection = new SeriesCollection(dayConfig);

            YFormatter = value => value.ToString();
            XFormatter = value => new DateTime((long)value).ToString("dd:MM:yy");
        }

        public void SetNewRates(IEnumerable<Rate> rates)
        {

            _rates = new List<RateViewModel>(rates.Select(x => new RateViewModel
            {
                Date = x.Date,
                Amount = x.Amount,
                Currency = x.Currency,
                Value = x.Value,
            }));

            Max = _rates.Max(x => x.Value);
            Min = _rates.Min(x => x.Value);
            YAxisName = SelectedCurrency == "BTC"
                ? $"USD per {_rates.FirstOrDefault()?.Amount}"
                : $"BelRub per {_rates.FirstOrDefault()?.Amount}";
            SeriesCollection.Clear();

            SeriesCollection.Add(new LineSeries
            {
                Values = new ChartValues<RateViewModel>(_rates),
            });
        }
    }
}
