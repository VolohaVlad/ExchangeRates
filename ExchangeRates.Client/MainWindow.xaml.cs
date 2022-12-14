using ExchangeRates.Client.Interfaces;
using ExchangeRates.Client.Properties;
using ExchangeRates.Client.ViewModels;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace ExchangeRates.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IApiService _apiService;

        public MainViewModel ViewModel { get; private set; }

        public MainWindow(IApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            ViewModel = new MainViewModel(Settings.Default.Currency, Settings.Default.Start, Settings.Default.End);
            DataContext = ViewModel;
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

                ViewModel.SetNewRates(rates);
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
