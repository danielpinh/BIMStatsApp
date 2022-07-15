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
using System.Collections;
using System.Reflection;

namespace NavisApp
{
    public class DataViewerApp : AddInPlugin
    {
        public override int Execute(params string[] parameters)
        {
            try
            {
                DataViewerAppMVVM dataViewerAppMVVM = new DataViewerAppMVVM();
                dataViewerAppMVVM.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return 0;            
        }
    }
}   

