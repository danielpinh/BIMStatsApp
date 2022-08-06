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
    public partial class EditParameterMVVM : Window, IDisposable
    {
        public static ObservableCollection<EditParameterViewModel> EditParameterViewModels { get;set; }

        /// <summary>
        /// Edit parameters window.
        /// </summary>
        public static ParameterViewModel ParameterViewModel { get; set; }  
        public EditParameterMVVM()
        {
            EditParameterViewModels = new ObservableCollection<EditParameterViewModel>();

            InitializeComponent();
            InitializeCommands();
            DataContext = this;

            EditParameterViewModels.Add(new EditParameterViewModel { GraphType = "LineSeries", GraphTypeLabel = "Linha" });
            EditParameterViewModels.Add(new EditParameterViewModel { GraphType = "ColumnSeries", GraphTypeLabel = "Coluna" });

            GraphType_CB.ItemsSource = EditParameterViewModels;
            GraphType_CB.DisplayMemberPath = "GraphTypeLabel";
        }
        private void EditParameter_Window_MouseDown(object sender, MouseButtonEventArgs e)
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
        private void Apply_Button_Click(object sender, RoutedEventArgs e)
        {
            EditParameterViewModel graphType = GraphType_CB.SelectedItem as EditParameterViewModel;
            ChartSettingsMVVM.ParameterViewModelSelected.GraphType = graphType.GraphType;
            this.Close();
        }
        private void EditParameter_Window_Loaded(object sender, RoutedEventArgs e)
        {
            string graphType = ChartSettingsMVVM.ParameterViewModelSelected.GraphType;

            foreach (var item in EditParameterViewModels)
            {
                if (item.GraphType == graphType)
                {
                    GraphType_CB.SelectedItem = item;
                }
            }

            ParameterName_TextBlock.Text = ChartSettingsMVVM.ParameterViewModelSelected.Name;
        }
    }
}
