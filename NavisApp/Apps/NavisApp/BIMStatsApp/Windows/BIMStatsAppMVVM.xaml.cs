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
        public static Func<double, string> Formatter => BIMStatsUIService.FormatterConverter;
        public static ChartSettingsMVVM ChartSettingsMVVM { get; set; }
        public static CostViewModel CostViewModel { get; set; }
        public static ChartValues<DateModel> ChartValues { get; set; } = new ChartValues<DateModel>();
        public BIMStatsAppMVVM()
        {
            ChartSettingsMVVM = new ChartSettingsMVVM();

            //ViewModels
            CostViewModel = new CostViewModel();
            ChartSettingsMVVM.StartDateViewModel = new DateViewModel();
            ChartSettingsMVVM.EndDateViewModel = new DateViewModel();
            ChartSettingsMVVM.GradationXAxisViewModels = new ObservableCollection<GradationXAxisViewModel>();
            ChartSettingsMVVM.GradationXAxisViewModelSelected = new GradationXAxisViewModel();
            ChartSettingsMVVM.ExcludedParameterViewModels = new ObservableCollection<ParameterViewModel>();
            ChartSettingsMVVM.AddedParameterViewModels = new ObservableCollection<ParameterViewModel>();
            ChartSettingsMVVM.ParameterViewModelSelected = new ParameterViewModel();

            //Properties
            ChartSettingsMVVM.PlannedStartChecked = new IsCheckedViewModel();
            ChartSettingsMVVM.ActualStartChecked = new IsCheckedViewModel();
            ChartSettingsMVVM.PlannedEndChecked = new IsCheckedViewModel();
            ChartSettingsMVVM.ActualEndChecked = new IsCheckedViewModel();

            SerieTitles = new List<string>();

            MainView = this;
            InitializeComponent();
            InitializeCommands();

            //DateTime dTime = Utils.GetSimulationCurrentDate();

            PlanCostCartesianChart.LegendLocation = LegendLocation.Right;

            DataContext = this;

            CostViewModel.TotalCost = NavisUtils.GetTotalCost();

            ChartSettingsMVVM.EndDateViewModel.Date = DateTime.Now;
            ChartSettingsMVVM.StartDateViewModel.Date = DateTime.Now;

            ChartSettingsMVVM.EndDateTextBox.DataContext = ChartSettingsMVVM.EndDateViewModel;
            ChartSettingsMVVM.StartDateTextBox.DataContext = ChartSettingsMVVM.StartDateViewModel;

            BIMStatsUIService.PopulateComboBoxAxisGradation();
            BIMStatsUIService.PopulateListViewParameters();

            BIMStatsUIService.AddParameterToCostChart("Custo total | Fim executado");
            BIMStatsUIService.AddParameterToCostChart("Custo total | Fim planejado");
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime earlierDateTime = DateTimeUtils.GetEarlierDate(NavisApp.Properties.NavisworksParameters.PlannedEndParameter);

                ChartSettingsMVVM.EndDateViewModel.Date = DateTime.Now;

                ChartSettingsMVVM.StartDateViewModel.Date = earlierDateTime;

                ChartSettingsMVVM.GradacaoXAxis_CB.SelectedIndex = 2;

                MainView.MyXAxis.Separator.Step = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            BIMStatsUIService.RefreshCostChart();
        }
        private void InitializeCommands()
        {
            this.ShowInTaskbar = true;
            this.Topmost = true;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.ResizeMode = ResizeMode.NoResize;
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
            ChartSettingsMVVM.Show();
            ChartSettingsMVVM.Focus();
        }
        private void PlanCostCartesianChart_DataClick(object sender, ChartPoint chartPoint)
        {
            try
            {
                DateModel dateModel = chartPoint.Instance as DateModel;
                NavisUtils.HideUnselectedItems(App.ActiveDocument, dateModel.ModelItems);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
