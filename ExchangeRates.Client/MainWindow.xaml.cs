using ExchangeRates.Client.Interfaces;
using ExchangeRates.Client.Properties;
using ExchangeRates.Client.ViewModels;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using ScottPlot.Renderable;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace ExchangeRates.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IApiService _apiService;

        public MainViewModel ViewModel { get; private set; }

        public SeriesCollection SeriesCollection { get; private set; }
        public Func<double, string> YFormatter { get; private set; }
        public Func<double, string> XFormatter { get; private set; }

        public double Max { get; private set; }
        public double Min { get; private set; }

        public string YAxisName { get; private set; }

        public MainWindow(IApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            DataContext = this;
            ViewModel = new MainViewModel(Settings.Default.Currency, Settings.Default.Start, Settings.Default.End);

            var dayConfig = Mappers.Xy<ChartModel>()
                   .X(dayModel => dayModel.DateTime.Ticks)
                   .Y(dayModel => dayModel.Value)
                   .Fill(item =>
                   {
                       if (item.Value == Max)
                       {
                           return new SolidColorBrush(Color.FromRgb(238, 83, 80));
                       }
                       if (item.Value == Min)
                       {
                           return new SolidColorBrush(Color.FromRgb(255, 255, 0));
                       }

                       return default;
                   })
                   .Stroke(item =>
                   {
                       if (item.Value == Max)
                       {
                           return new SolidColorBrush(Color.FromRgb(238, 83, 80));
                       }
                       if (item.Value == Min)
                       {
                           return new SolidColorBrush(Color.FromRgb(255, 255, 0));
                       }

                       return default;
                   });

            SeriesCollection = new SeriesCollection(dayConfig);

            YFormatter = value => value.ToString();
            XFormatter = value => new DateTime((long)value).ToString("yyyy-MM:dd");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            button.IsEnabled = false;
            try
            {
                ValidateInput();

                var rates = _apiService.GetRatesAsync(ViewModel.SelectedCurrency, ViewModel.StartDate, ViewModel.EndDate)
                    .ConfigureAwait(false).GetAwaiter().GetResult();

                if (rates.Count() < 2)
                {
                    throw new ArgumentException("Can't show chart for one day");
                }

                ViewModel.Rates = new ObservableCollection<RateViewModel>(rates.Select(x => new RateViewModel
                {
                    Date = x.Date,
                    Amount = x.Amount,
                    Currency = x.Currency,
                    Value = x.Value,
                }));

                var points = ViewModel.Rates.Select(x =>
                    new ChartModel
                    (
                        DateTime.Parse(x.Date, CultureInfo.InvariantCulture),
                         ((double)x.Value.Value
                    )));
                Max = points.Max(x => x.Value);
                Min = points.Min(x => x.Value);
                YAxisName = ViewModel.SelectedCurrency == "BTC"
                    ? $"USD per {ViewModel.Rates.FirstOrDefault()?.Amount}"
                    : $"BelRub per {ViewModel.Rates.FirstOrDefault()?.Amount}";
                SeriesCollection.Clear();

                SeriesCollection.Add(new LineSeries
                {
                    Values = new ChartValues<ChartModel>(points),
                });
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch
            {
                MessageBox.Show("Someting got wrong!");
            }
            finally
            {
                button.IsEnabled = true;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Settings.Default.Currency = ViewModel.SelectedCurrency;
            Settings.Default.Start = ViewModel.StartDate;
            Settings.Default.End = ViewModel.EndDate;
            Settings.Default.Save();

            base.OnClosing(e);
        }

        private void ValidateInput()
        {
            if (ViewModel.StartDate >= ViewModel.EndDate)
            {
                throw new ArgumentException("Start Date can be less then End Date");
            }

            if (ViewModel.StartDate < DateTime.Now.AddYears(-5).AddDays(-1) || ViewModel.EndDate < DateTime.Now.AddYears(-5).AddDays(-1))
            {
                throw new ArgumentException("Please enter dates between the last 5 years");
            }
        }
    }
}
