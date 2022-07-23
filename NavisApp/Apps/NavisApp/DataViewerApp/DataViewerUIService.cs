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
using System.Linq;
using System.Globalization;

namespace NavisApp
{
    public class DataViewerUIService
    {
        public static DateTime GetEarlierDate(string parameterName)
        {
            DateTime earlierDateTime = DateTime.Now;

            try
            {
                // Iterate over any existing Timeliner Tasks
                DocumentTimeliner documentTimeliner = Autodesk.Navisworks.Api.Application.MainDocument.GetTimeliner();
                List<DateTime> dateTimes = Utils.GetTimeLinerDates(documentTimeliner, parameterName);

                foreach (var dt in dateTimes)
                {
                    if (DateTime.Compare( dt, earlierDateTime) < 0)
                    {
                        earlierDateTime = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return earlierDateTime;
        }
        public static void RefreshCostChartByYear(string parameterName, string dateParameter, DateTime startDateTime, DateTime endDateTime, string seriesType)
        {
            try
            {
                // Iterate over any existing Timeliner Tasks
                DocumentTimeliner documentTimeliner = Autodesk.Navisworks.Api.Application.MainDocument.GetTimeliner();

                ChartValues<DateModel> chartValues = Utils.GetTimeLinerYearsByRangeByParameter(documentTimeliner, parameterName,
                    dateParameter, startDateTime, endDateTime);

                if (chartValues.Count > 0)
                {
                    AddSeries(parameterName, dateParameter, seriesType, chartValues);

                    DataViewerAppMVVM.Formatter = value => value.ToString();

                    DataViewerAppMVVM.MainView.MyXAxis.LabelFormatter = DataViewerAppMVVM.Formatter;

                    DataViewerAppMVVM.CostViewModel.PartialCost = Utils.GetPlannedPartialCost(documentTimeliner, startDateTime, endDateTime);

                    DataViewerAppMVVM.MainView.PlanCostCartesianChart.BringIntoView();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void RefreshCostChartBySemester(string parameterName, string dateParameter, DateTime startDateTime, DateTime endDateTime, string seriesType)
        {
            try
            {
                // Iterate over any existing Timeliner Tasks
                DocumentTimeliner documentTimeliner = Autodesk.Navisworks.Api.Application.MainDocument.GetTimeliner();

                ChartValues<DateModel> chartValues = Utils.GetTimeLinerSemestersByRangeByParametr(documentTimeliner, parameterName,
                    dateParameter, startDateTime, endDateTime);

                if (chartValues.Count > 0)
                {
                    AddSeries(parameterName, dateParameter, seriesType, chartValues);

                    DataViewerAppMVVM.Formatter = value => Utils.SemesterFormatter(new System.DateTime((long)(value * TimeSpan.FromDays(175).Ticks)));

                    DataViewerAppMVVM.MainView.MyXAxis.LabelFormatter = DataViewerAppMVVM.Formatter;

                    DataViewerAppMVVM.CostViewModel.PartialCost = Utils.GetPlannedPartialCost(documentTimeliner, startDateTime, endDateTime);

                    DataViewerAppMVVM.MainView.PlanCostCartesianChart.BringIntoView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void RefreshCostChartByQuarter(string parameterName, string dateParameter, DateTime startDateTime, DateTime endDateTime, string seriesType)
        {
            try
            {
                // Iterate over any existing Timeliner Tasks
                DocumentTimeliner documentTimeliner = Autodesk.Navisworks.Api.Application.MainDocument.GetTimeliner();

                ChartValues<DateModel> chartValues = Utils.GetTimeLinerQuartersByRangeByParameter(documentTimeliner, parameterName,
                    dateParameter, startDateTime, endDateTime);

                if (chartValues.Count > 0)
                {
                    AddSeries(parameterName, dateParameter, seriesType, chartValues);

                    DataViewerAppMVVM.Formatter = value => Utils.QuarterFormatter(new System.DateTime((long)(value * TimeSpan.FromDays(90).Ticks)));

                    DataViewerAppMVVM.MainView.MyXAxis.LabelFormatter = DataViewerAppMVVM.Formatter;

                    DataViewerAppMVVM.CostViewModel.PartialCost = Utils.GetPlannedPartialCost(documentTimeliner, startDateTime, endDateTime);

                    DataViewerAppMVVM.MainView.PlanCostCartesianChart.BringIntoView();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void RefreshCostChartByTwoWeeks(string parameterName, string dateParameter, DateTime startDateTime, DateTime endDateTime, string seriesType)
        {
            try
            {
                DocumentTimeliner documentTimeliner = Autodesk.Navisworks.Api.Application.MainDocument.GetTimeliner();

                ChartValues<DateModel> chartValues = Utils.GetTimeLinerTwoWeeksByRangeByParameter(documentTimeliner, parameterName,
                    dateParameter, startDateTime, endDateTime);

                if (chartValues.Count > 0)
                {
                    AddSeries(parameterName, dateParameter, seriesType, chartValues);

                    DataViewerAppMVVM.Formatter = value => Utils.TwoWeeksFormatter(new System.DateTime((long)(value * TimeSpan.FromDays(15).Ticks)));

                    DataViewerAppMVVM.MainView.MyXAxis.LabelFormatter = DataViewerAppMVVM.Formatter;

                    DataViewerAppMVVM.CostViewModel.PartialCost = Utils.GetPlannedPartialCost(documentTimeliner, startDateTime, endDateTime);

                    DataViewerAppMVVM.MainView.PlanCostCartesianChart.BringIntoView();
                }            
            }
            catch (Exception ex)
            {
                MessageBox.Show("RefreshCostChartByTwoWeeks\n\n" + ex.Message + ex.TargetSite);
            }
        }
        public static void RefreshCostChartByMonth(string parameterName, string dateParameter, DateTime startDateTime, DateTime endDateTime, string seriesType)
        {
            try
            {
                // Iterate over any existing Timeliner Tasks
                DocumentTimeliner documentTimeliner = Autodesk.Navisworks.Api.Application.MainDocument.GetTimeliner();

                ChartValues<DateModel> chartValues = Utils.GetTimeLinerMonthsByRangeByParameter(documentTimeliner, parameterName,
                    dateParameter, startDateTime, endDateTime);

                if (chartValues.Count > 0)
                {
                    AddSeries(parameterName, dateParameter, seriesType, chartValues);

                    DataViewerAppMVVM.Formatter = value => Utils.MonthFormatter(new System.DateTime((long)(value * TimeSpan.FromDays(30).Ticks)));

                    DataViewerAppMVVM.MainView.MyXAxis.LabelFormatter = DataViewerAppMVVM.Formatter;

                    DataViewerAppMVVM.CostViewModel.PartialCost = Utils.GetPlannedPartialCost(documentTimeliner, startDateTime, endDateTime);

                    DataViewerAppMVVM.MainView.PlanCostCartesianChart.BringIntoView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void RefreshCostChartByDay(string parameterName, string dateParameter, DateTime startDateTime, DateTime endDateTime, string seriesType)
        {
            try
            {
                // Iterate over any existing Timeliner Tasks
                DocumentTimeliner documentTimeliner = Autodesk.Navisworks.Api.Application.MainDocument.GetTimeliner();

                ChartValues<DateModel> chartValues = Utils.GetTimeLinerDaysByRangeByParameter(documentTimeliner, parameterName,
                    dateParameter, startDateTime, endDateTime);

                if (chartValues.Count > 0)
                {
                    AddSeries(parameterName, dateParameter, seriesType, chartValues);

                    DataViewerAppMVVM.Formatter = value => new System.DateTime((long)(value * TimeSpan.FromDays(1).Ticks)).ToString("dd/MM/yy");

                    DataViewerAppMVVM.MainView.MyXAxis.LabelFormatter = DataViewerAppMVVM.Formatter;

                    DataViewerAppMVVM.CostViewModel.PartialCost = Utils.GetPlannedPartialCost(documentTimeliner, startDateTime, endDateTime);

                    DataViewerAppMVVM.MainView.PlanCostCartesianChart.BringIntoView();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("RefreshCostChartByDay\n\n"+ ex.Message + ex.TargetSite);
            }
        }
        public static void RemoveSeriesByName(string parameterName)
        {
            try
            {
                ISeriesView itemToDelete = null;

                if (DataViewerAppMVVM.Series.Count > 0)
                {
                    foreach (var item in DataViewerAppMVVM.Series)
                    {
                        if (item.Title == parameterName || item.Title.Contains(parameterName))
                        {

                            itemToDelete = item;
                            break;
                        }
                    }
                }

                DataViewerAppMVVM.Series.Remove(itemToDelete);
            }
            catch { }
        }
        public static void RemoveSerieTitlesByName(string parameterName)
        {
            string itemToDelete = "";

            if (DataViewerAppMVVM.Series.Count > 0)
            {
                foreach (var item in DataViewerAppMVVM.Series)
                {
                    if (item.Title == parameterName || item.Title.Contains(parameterName))
                    {
                        itemToDelete = parameterName;
                        break;
                    }
                }
            }

            try
            {
                DataViewerAppMVVM.SerieTitles.Remove(itemToDelete);
            }
            catch { }
        }
        public static void RefreshCostChart()
        {
            try
            {
                GradationXAxisViewModel selectedGroupMode = DataViewerAppMVVM.ChartSettingsMVVM.GradacaoXAxis_CB.SelectedItem as GradationXAxisViewModel;

                switch (selectedGroupMode.Gradation)
                {
                    case "Diário":

                        try
                        {
                            var dayConfig = Mappers.Xy<DateModel>()
                                .X(dateModel => dateModel.DateTime.Ticks / TimeSpan.FromDays(1).Ticks)
                                .Y(dateModel => dateModel.Cost);

                            DataViewerAppMVVM.Series = new SeriesCollection(dayConfig);

                            DataViewerAppMVVM.MainView.PlanCostCartesianChart.Series = DataViewerAppMVVM.Series;

                            DataViewerAppMVVM.MainView.PlanCostCartesianChart.Update();

                            foreach (var pvm in ChartSettingsMVVM.AddedParameterViewModels)
                            {
                                DataViewerUIService.RefreshCostChartByDay(
                                    pvm.ParameterName,
                                    pvm.DateParameterName,
                                    ChartSettingsMVVM.StartDateViewModel.Date,
                                    ChartSettingsMVVM.EndDateViewModel.Date,
                                    pvm.GraphType);
                            }
                        }
                        catch { }
                        break;

                    case "Quinzenal":
                        try
                        {
                            var dayConfig = Mappers.Xy<DateModel>()
                                .X(dateModel => dateModel.DateTime.Ticks / TimeSpan.FromDays(15).Ticks)
                                .Y(dateModel => dateModel.Cost);

                            DataViewerAppMVVM.Series = new SeriesCollection(dayConfig);

                            DataViewerAppMVVM.MainView.PlanCostCartesianChart.Series = DataViewerAppMVVM.Series;

                            DataViewerAppMVVM.MainView.PlanCostCartesianChart.Update();

                            foreach (var pvm in ChartSettingsMVVM.AddedParameterViewModels)
                            {
                                DataViewerUIService.RefreshCostChartByTwoWeeks(
                                    pvm.ParameterName,
                                    pvm.DateParameterName,
                                    ChartSettingsMVVM.StartDateViewModel.Date,
                                    ChartSettingsMVVM.EndDateViewModel.Date,
                                    pvm.GraphType);
                            }
                        }
                        catch { }
                        break;
                    case "Mensal":
                        try
                        {
                            var dayConfig = Mappers.Xy<DateModel>()
                                .X(dateModel => dateModel.DateTime.Ticks / TimeSpan.FromDays(30).Ticks)
                                .Y(dateModel => dateModel.Cost);

                            DataViewerAppMVVM.Series = new SeriesCollection(dayConfig);

                            DataViewerAppMVVM.MainView.PlanCostCartesianChart.Series = DataViewerAppMVVM.Series;

                            DataViewerAppMVVM.MainView.PlanCostCartesianChart.Update();

                            foreach (var pvm in ChartSettingsMVVM.AddedParameterViewModels)
                            {
                                DataViewerUIService.RefreshCostChartByMonth(
                                    pvm.ParameterName,
                                    pvm.DateParameterName,
                                    ChartSettingsMVVM.StartDateViewModel.Date,
                                    ChartSettingsMVVM.EndDateViewModel.Date,
                                    pvm.GraphType);
                            }
                        }
                        catch { }
                        break;
                    case "Trimestral":
                        try
                        {
                            var dayConfig = Mappers.Xy<DateModel>()
                                 .X(dateModel => dateModel.DateTime.Ticks / TimeSpan.FromDays(90).Ticks)
                                 .Y(dateModel => dateModel.Cost);

                            DataViewerAppMVVM.Series = new SeriesCollection(dayConfig);

                            DataViewerAppMVVM.MainView.PlanCostCartesianChart.Series = DataViewerAppMVVM.Series;

                            DataViewerAppMVVM.MainView.PlanCostCartesianChart.Update();

                            foreach (var pvm in ChartSettingsMVVM.AddedParameterViewModels)
                            {
                                DataViewerUIService.RefreshCostChartByQuarter(
                                    pvm.ParameterName,
                                    pvm.DateParameterName,
                                    ChartSettingsMVVM.StartDateViewModel.Date,
                                    ChartSettingsMVVM.EndDateViewModel.Date,
                                    pvm.GraphType);
                            }
                        }
                        catch { }
                        break;
                    case "Semestral":
                        try
                        {
                            var dayConfig = Mappers.Xy<DateModel>()
                                .X(dateModel => dateModel.DateTime.Ticks / TimeSpan.FromDays(175).Ticks)
                                .Y(dateModel => dateModel.Cost);

                            DataViewerAppMVVM.Series = new SeriesCollection(dayConfig);

                            DataViewerAppMVVM.MainView.PlanCostCartesianChart.Series = DataViewerAppMVVM.Series;

                            DataViewerAppMVVM.MainView.PlanCostCartesianChart.Update();

                            foreach (var pvm in ChartSettingsMVVM.AddedParameterViewModels)
                            {
                                DataViewerUIService.RefreshCostChartBySemester(
                                    pvm.ParameterName,
                                    pvm.DateParameterName,
                                    ChartSettingsMVVM.StartDateViewModel.Date,
                                    ChartSettingsMVVM.EndDateViewModel.Date,
                                    pvm.GraphType);
                            }
                        }
                        catch { }
                        break;
                    case "Anual":
                        try
                        {
                            var dayConfig = Mappers.Xy<DateModel>()
                                .X(dateModel => dateModel.DateTime.Year)
                                .Y(dateModel => dateModel.Cost);

                            DataViewerAppMVVM.Series = new SeriesCollection(dayConfig);

                            DataViewerAppMVVM.MainView.PlanCostCartesianChart.Series = DataViewerAppMVVM.Series;

                            DataViewerAppMVVM.MainView.PlanCostCartesianChart.Update();

                            foreach (var pvm in ChartSettingsMVVM.AddedParameterViewModels)
                            {
                                DataViewerUIService.RefreshCostChartByYear(
                                    pvm.ParameterName,
                                    pvm.DateParameterName,
                                    ChartSettingsMVVM.StartDateViewModel.Date,
                                    ChartSettingsMVVM.EndDateViewModel.Date,
                                    pvm.GraphType);
                            }
                        }
                        catch { }
                        break;
                }

                RestoreChartZoom();
            }
            catch { }
        }
        public static void PopulateParametersListView()
        {
            //Parameters 
            ChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.TotalCost + " | " + NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.TotalCost,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    GraphType = "LineSeries"
                });
            ChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.TotalCost + " | " + NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.TotalCost,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    GraphType = "LineSeries"
                });
            ChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel 
                { Name = NavisApp.Properties.NavisworksParameters.MaterialCost + " | " + NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.MaterialCost,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    GraphType = "LineSeries" });
            ChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.MaterialCost + " | " + NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.MaterialCost,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    GraphType = "LineSeries"
                });
            ChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.LaborCost + " | " + NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.LaborCost,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    GraphType = "LineSeries"
                });
            ChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.LaborCost + " | " + NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.LaborCost,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    GraphType = "LineSeries"
                });
            ChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.EquipmentCost + " | " + NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.EquipmentCost,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    GraphType = "LineSeries"
                });
            ChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.EquipmentCost + " | " + NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.EquipmentCost,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    GraphType = "LineSeries"
                });
            ChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.SubcontractorCost + " | " + NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.SubcontractorCost,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    GraphType = "LineSeries"
                });
            ChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.SubcontractorCost + " | " + NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.SubcontractorCost,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    GraphType = "LineSeries"
                });
            ChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.Provided_Progress + " | " + NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.Provided_Progress,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    GraphType = "LineSeries"
                });
            ChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.Provided_Progress + " | " + NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.Provided_Progress,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    GraphType = "LineSeries"
                });

            DataViewerAppMVVM.ChartSettingsMVVM.ExcludedParameters_ListView.ItemsSource = ChartSettingsMVVM.ExcludedParameterViewModels;
            DataViewerAppMVVM.ChartSettingsMVVM.AddedParameters_ListView.ItemsSource = ChartSettingsMVVM.AddedParameterViewModels;
        }
        public static void PopulateXAxisGradation()
        {
            //ChartSettingsMVVM
            ChartSettingsMVVM.GradationXAxisViewModels.Add(new GradationXAxisViewModel { Gradation = "Diário" });
            ChartSettingsMVVM.GradationXAxisViewModels.Add(new GradationXAxisViewModel { Gradation = "Quinzenal" });
            ChartSettingsMVVM.GradationXAxisViewModels.Add(new GradationXAxisViewModel { Gradation = "Mensal" });
            ChartSettingsMVVM.GradationXAxisViewModels.Add(new GradationXAxisViewModel { Gradation = "Trimestral" });
            ChartSettingsMVVM.GradationXAxisViewModels.Add(new GradationXAxisViewModel { Gradation = "Semestral" });
            ChartSettingsMVVM.GradationXAxisViewModels.Add(new GradationXAxisViewModel { Gradation = "Anual" });

            //Gradation X Axis types
            ChartSettingsMVVM.GradationXAxisViewModelSelected = ChartSettingsMVVM.GradationXAxisViewModels[0];
            DataViewerAppMVVM.ChartSettingsMVVM.GradacaoXAxis_CB.ItemsSource = ChartSettingsMVVM.GradationXAxisViewModels;
            DataViewerAppMVVM.ChartSettingsMVVM.GradacaoXAxis_CB.DisplayMemberPath = "Gradation";
            DataViewerAppMVVM.ChartSettingsMVVM.GradacaoXAxis_CB.SelectedItem = ChartSettingsMVVM.GradationXAxisViewModelSelected;
        }
        public static void RestoreChartZoom()
        {
            DataViewerAppMVVM.MainView.PlanCostCartesianChart.AxisX[0].MinValue = double.NaN;
            DataViewerAppMVVM.MainView.PlanCostCartesianChart.AxisX[0].MaxValue = double.NaN;
            DataViewerAppMVVM.MainView.PlanCostCartesianChart.AxisY[0].MinValue = 0;
            DataViewerAppMVVM.MainView.PlanCostCartesianChart.AxisY[0].MaxValue = double.NaN;
        }


        public static DateTime DateTimeAfterConversion(DateTime dateTime)
        {
            long dateTimeTicks = dateTime.Ticks;

            var monthTicks = (long)TimeSpan.FromDays(28).Ticks;

            var dateTimePerMonth = (long)dateTimeTicks / monthTicks;

            var novoValor = (long)dateTimePerMonth * monthTicks;

            DateTime myDate = new DateTime(novoValor);

            return myDate;
        }

        public static void AddSeries(string parameterName, string dateParameter, string seriesType, ChartValues<DateModel> chartValues)
        {
            if (seriesType == "LineSeries")
            {
                DataViewerAppMVVM.Series.Add(new LineSeries() { Title = dateParameter + "\n/ " + parameterName, Values = chartValues });
            }
            else if (seriesType == "ColumnSeries")
            {
                DataViewerAppMVVM.Series.Add(new ColumnSeries() { Title = dateParameter + "\n/ " + parameterName, Values = chartValues });
            }
        }       
    }
}
