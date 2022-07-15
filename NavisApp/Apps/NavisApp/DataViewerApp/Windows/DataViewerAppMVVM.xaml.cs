using System;
using System.Windows;
using LiveCharts;
using System.Collections.Generic;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace NavisApp
{
    /// <summary>
    /// Data Viewer App Main Window.
    /// </summary>
    public partial class DataViewerAppMVVM : Window, IDisposable
    {
        public static DataViewerAppMVVM MainView { get; set; }
        public static List<string> SerieTitles { get; set; } 
        public static SeriesCollection Series { get; set; }
        public static Func<double, string> Formatter { get; set; }
        public static ChartSettingsMVVM ChartSettingsMVVM { get; set; }
        public static CostViewModel CostViewModel { get; set; }
        public static string LastParameterCostChart { get; set; } = "";
        public DataViewerAppMVVM()
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

            CostViewModel.TotalCost = Utils.GetTotalCost();

            ChartSettingsMVVM.EndDateViewModel.Date = DateTime.Now;
            ChartSettingsMVVM.StartDateViewModel.Date = DateTime.Now;

            ChartSettingsMVVM.EndDateTextBox.DataContext = ChartSettingsMVVM.EndDateViewModel;
            ChartSettingsMVVM.StartDateTextBox.DataContext = ChartSettingsMVVM.StartDateViewModel;

            DataViewerUIService.PopulateXAxisGradation();
            DataViewerUIService.PopulateParametersListView();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime earlierDateTime = DataViewerUIService.GetEarlierDate(NavisApp.Properties.NavisworksParameters.PlannedEndParameter);

            ChartSettingsMVVM.EndDateViewModel.Date = DateTime.Now;

            ChartSettingsMVVM.StartDateViewModel.Date = earlierDateTime;

            ChartSettingsMVVM.GradacaoXAxis_CB.SelectedIndex = 1;

            DataViewerAppMVVM.MainView.MyXAxis.Separator.Step = 1;
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
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
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
    }

    #region View Models
    public class DateModel
    {
        public DateTime DateTime { get; set; }
        public double Cost { get; set; }
        public double CustomDate { get; set; }
        public string DisplayName { get; set; }
    }
    public class IsCheckedViewModel : INotifyPropertyChanged
    {
        private bool _IsChecked { get; set; }
        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                _IsChecked = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
    public class DateViewModel : INotifyPropertyChanged
    {      
        private DateTime _Date { get; set; }
        public DateTime Date
        {
            get { return _Date; }
            set
            {
                _Date = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }        
       
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
    public class GradationXAxisViewModel : INotifyPropertyChanged
    {
        private string _Gradation { get; set; }
        public string Gradation
        {
            get { return _Gradation; }
            set
            {
                _Gradation = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
    public class CostViewModel : INotifyPropertyChanged
    {
        private double _PartialCost { get; set; }
        public double PartialCost
        {
            get { return _PartialCost; }
            set
            {
                _PartialCost = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }
        private double _TotalCost { get; set; }
        public double TotalCost
        {
            get { return _TotalCost; }
            set
            {
                _TotalCost = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
    #endregion
}
