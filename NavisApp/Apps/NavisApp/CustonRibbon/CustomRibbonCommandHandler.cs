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


namespace NavisApp
{
    [Plugin("NavisApp", "E4EF13F1 - 3CD4 - 4930 - 9778 - DCAD75A8499A", DisplayName = "NavisApp")]
    [RibbonLayout("CustomRibbon.xaml")]
    [RibbonTab("ID_CustomTab_1")]
    [Command("ID_Button_1", 
        DisplayName = "DataViewer", 
        LargeIcon = "Graph.png",
        ToolTip = "Descrição do comando...")]
    public class CustomRibbonCommandHandler : CommandHandlerPlugin
    {
        /// <summary>
        /// Executes a command when a button in the ribbon is pressed.
        /// </summary>
        /// <param name="commandId">Identifies the command associated with the button 
        /// that was pressed, by the Id defined in the command attribute.</param>
        /// <param name="parameters">Not currently used by Navisworks. If command is
        /// invoked programmatically by plugin author it can be used to pass additional
        /// information.</param>
        /// <returns>Not used by Navisworks. If command is invoked programmatically by 
        /// plugin author then it can be used to return additional information.</returns>
        public override int ExecuteCommand(string commandId, params string[] parameters)
        {
            if (commandId == "ID_Button_1")
            {
                try
                {                   

                    //// Iterate over any existing Timeliner Tasks
                    //DocumentTimeliner doc_timeliner = Autodesk.Navisworks.Api.Application.MainDocument.GetTimeliner();

                    //var mapper = Mappers.Xy<DataPoint>()
                    //    .X(dp => dp.TimeStamp.Ticks)
                    //    .Y(dp => dp.Value);

                    //DataViewerAppMVVM.SeriesCollection = new LiveCharts.SeriesCollection(mapper);
                        
                    ////Coletando "PlannedStartDate"
                    //List<DataPoint> plannedStartDatas = new List<DataPoint>();
                    //foreach (TimelinerTask task in doc_timeliner.Tasks)
                    //{
                    //    DataPoint dataPoint = new DataPoint();
                    //    dataPoint.TimeStamp = (DateTime)task.PlannedStartDate;                        
                    //    plannedStartDatas.Add(dataPoint); 
                    //}

                    //DataViewerAppMVVM.SeriesCollection.Add(new ColumnSeries
                    //{
                    //    Title = "PlannedStartDate",
                    //    Values = new ChartValues<DataPoint> { plannedStartDatas[0], plannedStartDatas[1], plannedStartDatas[2] }
                    //});

                    //DataViewerAppMVVM.Labels = new[] { "Maria", "Susan", "Charles", "Frida" };
                    
                    //DataViewerAppMVVM.Formatter = value => new DateTime((long)value).ToString("MM/dd/yyyy");
                    
                    //DataViewerAppMVVM.SeriesCollection = new SeriesCollection
                    //{
                    //    new ColumnSeries
                    //    {
                    //        Title = "2015",
                    //        Values = new ChartValues<double> { 10, 50, 39, 50 }
                    //    }
                    //};

                    ////adding series will update and animate the chart automatically
                    //DataViewerAppMVVM.SeriesCollection.Add(new ColumnSeries
                    //{
                    //    Title = "2016",
                    //    Values = new ChartValues<double> { 11, 56, 42 }
                    //});

                    //////also adding values updates and animates the chart automatically
                    ////DataViewerAppMVVM.SeriesCollection[1].Values.Add(48d);


                    //DataViewerAppMVVM.SeriesCollection.Add(new ColumnSeries
                    //{
                    //    Title = "PlannedStartDate",    
                    //    Values = new ChartValues<string> { plannedStartDatas[0].ToString(), plannedStartDatas[1].ToString(), plannedStartDatas[2].ToString() }
                    //});

                    ////Coletando "PlannedEndDate"
                    //List<string> plannedEndDatas = new List<string>();
                    //foreach (TimelinerTask task in doc_timeliner.Tasks)
                    //{
                    //    plannedEndDatas.Add(task.PlannedEndDate.ToString());
                    //}

                    //DataViewerAppMVVM.SeriesCollection.Add(new ColumnSeries
                    //{
                    //    Title = "PlannedEndDate",
                    //    Values = new ChartValues<string> { plannedEndDatas[0].ToString(), plannedEndDatas[1].ToString(), plannedEndDatas[2].ToString() }
                    //});

                    ////Coletando o nome das tasks
                    //List<string> taskNames = new List<string>();
                    //foreach (TimelinerTask task in doc_timeliner.Tasks)
                    //{
                    //    taskNames.Add(task.SimulationTaskTypeName.ToString());
                    //}                  

                    //DataViewerAppMVVM.Labels = new[] { taskNames[0], taskNames[1], taskNames[2] };

                    //Open Main Window
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return 0;
        }        
    
    }
}        
