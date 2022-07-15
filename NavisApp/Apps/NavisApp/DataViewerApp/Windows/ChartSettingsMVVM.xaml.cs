using System;
using System.Windows;
using Autodesk.Windows;
using Autodesk.Navisworks.Api.Plugins;
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Timeliner;
using System.Diagnostics;
using System.Text;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using LiveCharts.Definitions.Series;

namespace NavisApp
{    
    public partial class ChartSettingsMVVM : Window, IDisposable
    {
        /// <summary>
        /// Cost chart settings window.
        /// </summary>
        public static DateViewModel StartDateViewModel { get; set; }
        public static DateViewModel EndDateViewModel { get; set; }
        public static IsCheckedViewModel PlannedStartChecked { get; set; } 
        public static IsCheckedViewModel ActualStartChecked { get; set; }
        public static IsCheckedViewModel PlannedEndChecked { get; set; } 
        public static IsCheckedViewModel ActualEndChecked { get; set; }
        public static IsCheckedViewModel ChartSeparator { get; set; }
        public static ObservableCollection<GradationXAxisViewModel> GradationXAxisViewModels { get; set; }
        public static ObservableCollection<ParameterViewModel> AddedParameterViewModels { get; set; }
        public static ObservableCollection<ParameterViewModel> ExcludedParameterViewModels { get; set; }
        public static GradationXAxisViewModel GradationXAxisViewModelSelected { get; set; }
        public static ParameterViewModel ParameterViewModelSelected { get; set; }

        public ChartSettingsMVVM()
        {
            InitializeComponent();
            InitializeCommands();

            DataContext = this;
        }
        private void ChartSettingMVVMWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void ChartSettingMVVMWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        public void Dispose()
        {
            this.Close();
        }
        private void InitializeCommands()
        {
            this.ShowInTaskbar = true;
            this.Topmost = true;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStyle = WindowStyle.None;
        }

        private void StartDate_Button_Click(object sender, RoutedEventArgs e)
        {
            if (StartDateCalendar.Visibility == Visibility.Visible)
            {
                StartDateCalendar.Visibility = Visibility.Hidden;
            }
            else
            {
                StartDateCalendar.Visibility = Visibility.Visible;
            }
        }

        private void EndDateButton_Click(object sender, RoutedEventArgs e)
        {
            if (EndDateCalendar.Visibility == Visibility.Visible)
            {
                EndDateCalendar.Visibility = Visibility.Hidden;
            }
            else
            {
                EndDateCalendar.Visibility = Visibility.Visible;
            }
        }

        private void EndDateCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            EndDateViewModel.Date = (DateTime)EndDateCalendar.SelectedDate;
            EndDateCalendar.Visibility = Visibility.Hidden;

            //List<string> serieTitles = new List<string>(DataViewerAppMVVM.SerieTitles);

            //foreach (var title in serieTitles)
            //{
            //    DataViewerUIService.RefreshCostChart(NavisApp.Properties.NavisworksParameters.TotalCost, title, StartDateViewModel.Date, EndDateViewModel.Date, "ColumnSeries");
            //}
        }

        private void StartDateCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            StartDateViewModel.Date = (DateTime)StartDateCalendar.SelectedDate;
            StartDateCalendar.Visibility = Visibility.Hidden;

            //List<string> serieTitles = new List<string>(DataViewerAppMVVM.SerieTitles);

            //foreach (var title in serieTitles)
            //{
            //    DataViewerUIService.RefreshCostChart(NavisApp.Properties.NavisworksParameters.TotalCost, title, StartDateViewModel.Date, EndDateViewModel.Date, "ColumnSeries");
            //}
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void Gradacao_CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //DataViewerUIService.RestoreChartZoom();
            //ChartSettingsMVVM.GradationXAxisViewModelSelected = GradacaoXAxis_CB.SelectedItem as GradationXAxisViewModel;
            //DataViewerUIService.GroupXAxisChanged();
        }

        private void AddParameter_Button_Click(object sender, RoutedEventArgs e)
        {
            List<ParameterViewModel> pvmSelecteds = new List<ParameterViewModel>();
            foreach (var item in ExcludedParameters_ListView.SelectedItems)
            {
                ParameterViewModel pvmSelected = item as ParameterViewModel;
                pvmSelecteds.Add(pvmSelected);
            }

            foreach (var pvm in pvmSelecteds)
            {
                List<ParameterViewModel> pvms = new List<ParameterViewModel>(ExcludedParameterViewModels);
                int index = pvms.FindIndex(p => p.ParameterName == pvm.ParameterName);

                AddedParameterViewModels.Add(ExcludedParameterViewModels[index]);
                ExcludedParameterViewModels.Remove(ExcludedParameterViewModels[index]);
            }
        }

        private void RemoveParameter_Button_Click(object sender, RoutedEventArgs e)
        {
            List<ParameterViewModel> pvmSelecteds = new List<ParameterViewModel>();
            foreach (var item in AddedParameters_ListView.SelectedItems)
            {
                ParameterViewModel pvmSelected = item as ParameterViewModel;
                pvmSelecteds.Add(pvmSelected);
            }

            foreach (var pvm in pvmSelecteds)
            {
                List<ParameterViewModel> pvms = new List<ParameterViewModel>(AddedParameterViewModels);
                int index = pvms.FindIndex(p => p.ParameterName == pvm.ParameterName);

                ExcludedParameterViewModels.Add(AddedParameterViewModels[index]);
                AddedParameterViewModels.Remove(AddedParameterViewModels[index]);
            }
        }

        private void Apply_Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var pvm in AddedParameterViewModels)
            {
                MessageBox.Show(pvm.ParameterName + 
                    "\n" + pvm.DateParameterName +
                    "\n" + pvm.GraphType +
                    "\n" + StartDateViewModel.Date.ToString() + 
                    "\n" + EndDateViewModel.Date.ToString());


                DataViewerUIService.RefreshCostChart(
                    pvm.ParameterName,
                    pvm.DateParameterName,
                    StartDateViewModel.Date,
                    EndDateViewModel.Date,
                    pvm.GraphType);
            }

            //DataViewerUIService.RemoveSerieTitlesByName(NavisApp.Properties.NavisworksParameters.PlannedStartParameter);
            //DataViewerUIService.RemoveSeriesByName(NavisApp.Properties.NavisworksParameters.PlannedStartParameter);
            //DataViewerUIService.ParametersChanged(AddedParameterViewModels, ExcludedParameterViewModels);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var item = ((sender as Button)?.Tag as ListViewItem)?.DataContext;

            ChartSettingsMVVM.ParameterViewModelSelected = item as ParameterViewModel;

            EditParameterMVVM editParameterMVVM = new EditParameterMVVM();
            editParameterMVVM.Show();
        }
    }
    public class ParameterViewModel
    {
        private string _Name { get; set; }
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        private string _ParameterName { get; set; }
        public string ParameterName
        {
            get { return _ParameterName; }
            set
            {
                _ParameterName = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }
        private string _DateParameterName { get; set; }
        public string DateParameterName
        {
            get { return _DateParameterName; }
            set
            {
                _DateParameterName = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        private string _GraphType { get; set; }
        public string GraphType
        {
            get { return _GraphType; }
            set
            {
                _GraphType = value;
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

}
