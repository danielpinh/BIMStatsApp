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
using NavisApp.ViewModels;

namespace NavisApp
{    
    public partial class SCurveChartSettingsMVVM : Window, IDisposable
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
        public SCurveChartSettingsMVVM()
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
            DateTime dateTime = (DateTime)EndDateCalendar.SelectedDate;
            dateTime = dateTime.AddHours(23);
            dateTime = dateTime.AddMinutes(59);
            dateTime = dateTime.AddSeconds(59);

            EndDateViewModel.Date = dateTime;

            EndDateCalendar.Visibility = Visibility.Hidden;
        }

        private void StartDateCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime dateTime = (DateTime)StartDateCalendar.SelectedDate;
            dateTime = dateTime.AddHours(23);
            dateTime = dateTime.AddMinutes(59);
            dateTime = dateTime.AddSeconds(59);

            StartDateViewModel.Date = dateTime;

            StartDateCalendar.Visibility = Visibility.Hidden;
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void Gradacao_CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PlannedExecutedChartSettingsMVVM.GradationXAxisViewModelSelected = GradacaoXAxis_CB.SelectedItem as GradationXAxisViewModel;
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
            BIMStatsUIService.RefreshPlannedExecutedChart();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var item = ((sender as Button)?.Tag as ListViewItem)?.DataContext;

            PlannedExecutedChartSettingsMVVM.ParameterViewModelSelected = item as ParameterViewModel;

            EditParameterMVVM editParameterMVVM = new EditParameterMVVM();
            editParameterMVVM.ShowDialog();
        }
    }
}
