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
                //DateTime dateTime = new DateTime(2022, 12, 28);

                //MessageBox.Show(dateTime.ToString());

                //long dateTimeTicks = dateTime.Ticks;

                //long monthTicks = (long)TimeSpan.FromDays(30).Ticks;

                //long dateTimePerMonth = (long)dateTimeTicks / monthTicks;

                ////MessageBox.Show(dateTimeTicks.ToString() + $" dividido por 30 Ticks {monthTicks.ToString()} é igual a:\n\n" + dateTimePerMonth.ToString());

                //long novoValor = (long)dateTimePerMonth * monthTicks;

                ////MessageBox.Show(dateTimePerMonth.ToString() + "multiplicado por 30 Ticks é igual a:\n\n" + novoValor.ToString());

                //DateTime myDate = new DateTime(novoValor);

                //MessageBox.Show(myDate.ToString());

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

