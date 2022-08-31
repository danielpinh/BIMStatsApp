using System;
using System.Windows;
using LiveCharts;
using System.Collections.Generic;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Autodesk.Navisworks.Api;
using App = Autodesk.Navisworks.Api.Application;
using NavisApp.ViewModels;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using NavisApp.Utils;
using System.Windows.Media;

namespace NavisApp
{
    /// <summary>
    /// Data Viewer App Main Window.
    /// </summary>
    public partial class BIMStatsAppMVVM : Window, IDisposable
    {
        public static BIMStatsAppMVVM MainView { get; set; }
        public static List<string> SerieTitles { get; set; } 
        public static SeriesCollection Series { get; set; }
        public static Func<double, string> DateFormatter => BIMStatsUIService.DateFormatterConverter;
        public static Func<double, string> CurrencyFormatter => BIMStatsUIService.CurrencyFormatterConverter;
        public static PlannedExecutedChartSettingsMVVM PlannedExecutedChartSettingsMVVM { get; set; }
        public static CostViewModel CostViewModel { get; set; }
        public static ChartValues<DateModel> ChartValues { get; set; } = new ChartValues<DateModel>();
        public BIMStatsAppMVVM()
        {
            PlannedExecutedChartSettingsMVVM = new PlannedExecutedChartSettingsMVVM();

            //ViewModels
            CostViewModel = new CostViewModel();
            PlannedExecutedChartSettingsMVVM.StartDateViewModel = new DateViewModel();
            PlannedExecutedChartSettingsMVVM.EndDateViewModel = new DateViewModel();
            PlannedExecutedChartSettingsMVVM.GradationXAxisViewModels = new ObservableCollection<GradationXAxisViewModel>();
            PlannedExecutedChartSettingsMVVM.GradationXAxisViewModelSelected = new GradationXAxisViewModel();
            PlannedExecutedChartSettingsMVVM.ExcludedParameterViewModels = new ObservableCollection<ParameterViewModel>();
            PlannedExecutedChartSettingsMVVM.AddedParameterViewModels = new ObservableCollection<ParameterViewModel>();
            PlannedExecutedChartSettingsMVVM.ParameterViewModelSelected = new ParameterViewModel();

            //Properties
            PlannedExecutedChartSettingsMVVM.PlannedStartChecked = new IsCheckedViewModel();
            PlannedExecutedChartSettingsMVVM.ActualStartChecked = new IsCheckedViewModel();
            PlannedExecutedChartSettingsMVVM.PlannedEndChecked = new IsCheckedViewModel();
            PlannedExecutedChartSettingsMVVM.ActualEndChecked = new IsCheckedViewModel();

            SerieTitles = new List<string>();

            MainView = this;
            InitializeComponent();
            InitializeCommands();

            //DateTime dTime = Utils.GetSimulationCurrentDate();

            PlannedExecutedChart.LegendLocation = LegendLocation.Right;
            SCurveCartesianChart.LegendLocation = LegendLocation.Right;

            DataContext = this;

            PlannedExecutedChartSettingsMVVM.EndDateViewModel.Date = DateTime.Now;
            PlannedExecutedChartSettingsMVVM.StartDateViewModel.Date = DateTime.Now;

            PlannedExecutedChartSettingsMVVM.EndDateTextBox.DataContext = PlannedExecutedChartSettingsMVVM.EndDateViewModel;
            PlannedExecutedChartSettingsMVVM.StartDateTextBox.DataContext = PlannedExecutedChartSettingsMVVM.StartDateViewModel;

            BIMStatsUIService.PopulateComboBoxAxisGradation();
            BIMStatsUIService.PopulateListViewParameters();

            BIMStatsUIService.AddParameterToPlannedExecutedChart("Custo total | Fim executado");
            BIMStatsUIService.AddParameterToPlannedExecutedChart("Custo total | Fim planejado");
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime earlierDateTime = DateTimeUtils.GetEarlierDate(NavisApp.Properties.NavisworksParameters.PlannedEndParameter);

                PlannedExecutedChartSettingsMVVM.EndDateViewModel.Date = DateTime.Now;

                PlannedExecutedChartSettingsMVVM.StartDateViewModel.Date = earlierDateTime;

                PlannedExecutedChartSettingsMVVM.GradacaoXAxis_CB.SelectedIndex = 2;

                MainView.PlannedExecutedChartXAxis.Separator.Step = 1;
                MainView.SCurveCartesianChartXAxis.Separator.Step = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            BIMStatsAppMVVM.MainView.PlannedExecutedChartXAxis.Separator.StrokeThickness = 0;
            BIMStatsAppMVVM.MainView.PlannedExecutedChartYAxis.Separator.StrokeThickness = 1;
            BIMStatsAppMVVM.MainView.PlannedExecutedChartYAxis.Separator.Stroke = new SolidColorBrush(Colors.LightGray);

            BIMStatsAppMVVM.MainView.SCurveCartesianChartXAxis.Separator.StrokeThickness = 0;
            BIMStatsAppMVVM.MainView.SCurveCartesianChartYAxis.Separator.StrokeThickness = 1;
            BIMStatsAppMVVM.MainView.SCurveCartesianChartYAxis.Separator.Stroke = new SolidColorBrush(Colors.LightGray);


            BIMStatsAppMVVM.MainView.ExecutedPartialCostGaugeChart.LabelFormatter = BIMStatsAppMVVM.CurrencyFormatter;
            BIMStatsAppMVVM.MainView.PlannedPartialCostGaugeChart.LabelFormatter = BIMStatsAppMVVM.CurrencyFormatter;
            BIMStatsAppMVVM.MainView.ExecutedPartialCostGaugeChart.HighFontSize = 11;
            BIMStatsAppMVVM.MainView.PlannedPartialCostGaugeChart.HighFontSize = 11;
            BIMStatsAppMVVM.MainView.ExecutedPartialCostGaugeChart.InnerRadius = 40;
            BIMStatsAppMVVM.MainView.PlannedPartialCostGaugeChart.InnerRadius = 40;

            BIMStatsUIService.SetGaugeChartValues();
            BIMStatsUIService.RefreshPlannedExecutedChart();
            BIMStatsUIService.RefreshSCurveChart();
        }
        private void InitializeCommands()
        {
            this.ShowInTaskbar = true;
            this.Topmost = true;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.WindowStyle = WindowStyle.None;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
            }
            catch { }
        }
        public void Dispose()
        {
            this.Close();
        }
        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Minimize_Button_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Settings_Button_Click(object sender, RoutedEventArgs e)
        {
            PlannedExecutedChartSettingsMVVM.Show();
            PlannedExecutedChartSettingsMVVM.Focus();
        }
        private void PlanCostCartesianChart_DataClick(object sender, ChartPoint chartPoint)
        {
            try
            {
                DateModel dateModel = chartPoint.Instance as DateModel;
                NavisUtils.HideUnselectedItems(App.ActiveDocument, dateModel.ModelItems);
            }
            catch { }
        }
        private void SCurveChart_Settings_Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
    #region View Models
    public class DateModel
    {
        public DateTime DateTime { get; set; }
        public double Sequence { get; set; }
        public double Cost { get; set; }
        public string DisplayName { get; set; }
        public List<ModelItem> ModelItems { get; set; }
    }
    #endregion
}
