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
using System.Windows.Media.Effects;
using NavisApp.ViewModels;
using NavisApp.Utils;

namespace NavisApp
{
    public class BIMStatsUIService
    {
        public static void RefreshCostChartByYear(string parameterName, string dateParameter, DateTime startDateTime, DateTime endDateTime, string seriesType, Brush colorFill, Brush colorPoint)
        {
            try
            {
                // Iterate over any existing Timeliner Tasks
                DocumentTimeliner documentTimeliner = Autodesk.Navisworks.Api.Application.MainDocument.GetTimeliner();

                ChartValues<DateModel> chartValues = NavisUtils.GetTimeLinerParametersByYear(documentTimeliner, parameterName,
                    dateParameter, startDateTime, endDateTime);

                // Add zero values to date time possible existing gaps 
                chartValues = NavisUtils.FixDateTimeGapsByYear(chartValues, startDateTime, endDateTime);

                // Reorder ChartValues
                chartValues = ChartUtils.SortChartValuesByDateTime(chartValues);

                if (chartValues.Count > 0)
                {
                    SetChartSeries(parameterName, dateParameter, startDateTime, endDateTime, seriesType, documentTimeliner, chartValues, colorFill, colorPoint);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void RefreshCostChartBySemester(string parameterName, string dateParameter, DateTime startDateTime, DateTime endDateTime, string seriesType, Brush colorFill, Brush colorPoint)
        {
            try
            {
                // Iterate over any existing Timeliner Tasks
                DocumentTimeliner documentTimeliner = Autodesk.Navisworks.Api.Application.MainDocument.GetTimeliner();

                ChartValues<DateModel> chartValues = NavisUtils.GetTimeLinerParametersBySemester(documentTimeliner, parameterName,
                    dateParameter, startDateTime, endDateTime);

                // Add zero values to date time possible existing gaps 
                chartValues = NavisUtils.FixDateTimeGapsBySemester(chartValues, startDateTime, endDateTime);

                // Reorder ChartValues
                chartValues = ChartUtils.SortChartValuesByDateTime(chartValues);

                if (chartValues.Count > 0)
                {
                    SetChartSeries(parameterName, dateParameter, startDateTime, endDateTime, seriesType, documentTimeliner, chartValues, colorFill, colorPoint);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void RefreshCostChartByQuarter(string parameterName, string dateParameter, DateTime startDateTime, DateTime endDateTime, string seriesType, Brush colorFill, Brush colorPoint)
        {
            try
            {
                // Iterate over any existing Timeliner Tasks
                DocumentTimeliner documentTimeliner = Autodesk.Navisworks.Api.Application.MainDocument.GetTimeliner();

                ChartValues<DateModel> chartValues = NavisUtils.GetTimeLinerParametersByQuarter(documentTimeliner, parameterName,
                    dateParameter, startDateTime, endDateTime);

                // Add zero values to date time possible existing gaps 
                chartValues = NavisUtils.FixDateTimeGapsByQuarter(chartValues, startDateTime, endDateTime);

                // Reorder ChartValues
                chartValues = ChartUtils.SortChartValuesByDateTime(chartValues);

                if (chartValues.Count > 0)
                {

                    SetChartSeries(parameterName, dateParameter, startDateTime, endDateTime, seriesType, documentTimeliner, chartValues, colorFill, colorPoint);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void RefreshCostChartByTwoWeeks(string parameterName, string dateParameter, DateTime startDateTime, DateTime endDateTime, string seriesType, Brush colorFill, Brush colorPoint)
        {
            try
            {
                DocumentTimeliner documentTimeliner = Autodesk.Navisworks.Api.Application.MainDocument.GetTimeliner();

                ChartValues<DateModel> chartValues = NavisUtils.GetTimeLinerParametersByTwoWeeks(documentTimeliner, parameterName,
                    dateParameter, startDateTime, endDateTime);

                // Add zero values to date time possible existing gaps 
                chartValues = NavisUtils.FixDateTimeGapsByTwoWeeks(chartValues, startDateTime, endDateTime);

                // Reorder ChartValues
                chartValues = ChartUtils.SortChartValuesByDateTime(chartValues);

                if (chartValues.Count > 0)
                {
                    SetChartSeries(parameterName, dateParameter, startDateTime, endDateTime, seriesType, documentTimeliner, chartValues, colorFill, colorPoint);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("RefreshCostChartByTwoWeeks\n\n" + ex.Message + ex.TargetSite);
            }
        }
        public static void RefreshCostChartByMonth(string parameterName, string dateParameter, DateTime startDateTime, DateTime endDateTime, string seriesType, Brush colorFill, Brush colorPoint)
        {
            try
            {
                // Iterate over any existing Timeliner Tasks
                DocumentTimeliner documentTimeliner = Autodesk.Navisworks.Api.Application.MainDocument.GetTimeliner();

                ChartValues<DateModel> chartValues = NavisUtils.GetTimeLinerParametersByMonth(documentTimeliner, parameterName,
                    dateParameter, startDateTime, endDateTime);

                // Add zero values to date time gaps 
                chartValues = NavisUtils.FixDateTimeGapsByMonth(chartValues, startDateTime, endDateTime);

                // Reorder ChartValues
                chartValues = ChartUtils.SortChartValuesByDateTime(chartValues);

                if (chartValues.Count > 0)
                {
                    SetChartSeries(parameterName, dateParameter, startDateTime, endDateTime, seriesType, documentTimeliner, chartValues, colorFill, colorPoint);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void RefreshCostChartByDay(string parameterName, string dateParameter, DateTime startDateTime, DateTime endDateTime, string seriesType, Brush colorFill, Brush colorPoint)
        {
            try
            {
                // Iterate over any existing Timeliner Tasks
                DocumentTimeliner documentTimeliner = Autodesk.Navisworks.Api.Application.MainDocument.GetTimeliner();

                ChartValues<DateModel> chartValues = NavisUtils.GetTimeLinerParametersByDay(documentTimeliner, parameterName,
                    dateParameter, startDateTime, endDateTime);

                if (chartValues.Count > 0)
                {
                    SetChartSeries(parameterName, dateParameter, startDateTime, endDateTime, seriesType, documentTimeliner, chartValues, colorFill, colorPoint);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("RefreshCostChartByDay\n\n"+ ex.Message + ex.TargetSite);
            }
        }
        public static void SetChartSeries(
            string parameterName,
            string dateParameter,
            DateTime startDateTime,
            DateTime endDateTime,
            string seriesType,
            DocumentTimeliner documentTimeliner,
            ChartValues<DateModel> chartValues,
            Brush chartFillColor,
            Brush pointForegroundColor)
        {
            AddSeries(parameterName, dateParameter, seriesType, chartValues, chartFillColor, pointForegroundColor);

            BIMStatsAppMVVM.MainView.MyXAxis.LabelFormatter = BIMStatsAppMVVM.Formatter;

            BIMStatsAppMVVM.CostViewModel.PartialCost = NavisUtils.GetPlannedPartialCost(documentTimeliner, startDateTime, endDateTime);

            BIMStatsAppMVVM.MainView.PlanCostCartesianChart.BringIntoView();
        }


        public static void RemoveSeriesByName(string parameterName)
        {
            try
            {
                ISeriesView itemToDelete = null;

                if (BIMStatsAppMVVM.Series.Count > 0)
                {
                    foreach (var item in BIMStatsAppMVVM.Series)
                    {
                        if (item.Title == parameterName || item.Title.Contains(parameterName))
                        {
                            itemToDelete = item;
                            break;
                        }
                    }
                }

                BIMStatsAppMVVM.Series.Remove(itemToDelete);
            }
            catch { }
        }
        public static void RemoveSerieTitlesByName(string parameterName)
        {
            string itemToDelete = "";

            if (BIMStatsAppMVVM.Series.Count > 0)
            {
                foreach (var item in BIMStatsAppMVVM.Series)
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
                BIMStatsAppMVVM.SerieTitles.Remove(itemToDelete);
            }
            catch { }
        }
        public static void RefreshCostChart()
        {
            try
            {
                GradationXAxisViewModel selectedGroupMode = BIMStatsAppMVVM.ChartSettingsMVVM.GradacaoXAxis_CB.SelectedItem as GradationXAxisViewModel;

                switch (selectedGroupMode.Gradation)
                {
                    case "Diário":

                        try
                        {
                            var dayConfig = Mappers.Xy<DateModel>()
                                 .X(dateModel => dateModel.DateTime.Ticks / TimeSpan.FromDays(1).Ticks)
                                 .Y(dateModel => dateModel.Cost);

                            BIMStatsAppMVVM.Series = new SeriesCollection(dayConfig);

                            BIMStatsAppMVVM.MainView.PlanCostCartesianChart.Series = BIMStatsAppMVVM.Series;

                            BIMStatsAppMVVM.MainView.PlanCostCartesianChart.Update();

                            foreach (var pvm in ChartSettingsMVVM.AddedParameterViewModels)
                            {
                                SetParameterDefaultColor(pvm);

                                BIMStatsUIService.RefreshCostChartByDay(
                                    pvm.ParameterName,
                                    pvm.DateParameterName,
                                    ChartSettingsMVVM.StartDateViewModel.Date,
                                    ChartSettingsMVVM.EndDateViewModel.Date,
                                    pvm.GraphType,
                                    pvm.FillColor,
                                    pvm.PointColor
                                    );
                            }
                        }
                        catch { }
                        break;

                    case "Quinzenal":
                        try
                        {
                            var dayConfig = Mappers.Xy<DateModel>()
                                .X(dateModel => dateModel.Sequence)
                                .Y(dateModel => dateModel.Cost);

                            BIMStatsAppMVVM.Series = new SeriesCollection(dayConfig);

                            BIMStatsAppMVVM.MainView.PlanCostCartesianChart.Series = BIMStatsAppMVVM.Series;

                            BIMStatsAppMVVM.MainView.PlanCostCartesianChart.Update();

                            foreach (var pvm in ChartSettingsMVVM.AddedParameterViewModels)
                            {
                                SetParameterDefaultColor(pvm);

                                BIMStatsUIService.RefreshCostChartByTwoWeeks(
                                    pvm.ParameterName,
                                    pvm.DateParameterName,
                                    ChartSettingsMVVM.StartDateViewModel.Date,
                                    ChartSettingsMVVM.EndDateViewModel.Date,
                                    pvm.GraphType,
                                    pvm.FillColor,
                                    pvm.PointColor
                                    );
                            }
                        }
                        catch { }
                        break;
                    case "Mensal":
                        try
                        {
                            var dayConfig = Mappers.Xy<DateModel>()
                                .X(dateModel => dateModel.Sequence)
                                .Y(dateModel => dateModel.Cost);

                            BIMStatsAppMVVM.Series = new SeriesCollection(dayConfig);

                            BIMStatsAppMVVM.MainView.PlanCostCartesianChart.Series = BIMStatsAppMVVM.Series;

                            BIMStatsAppMVVM.MainView.PlanCostCartesianChart.Update();

                            foreach (var pvm in ChartSettingsMVVM.AddedParameterViewModels)
                            {
                                SetParameterDefaultColor(pvm);

                                BIMStatsUIService.RefreshCostChartByMonth(
                                    pvm.ParameterName,
                                    pvm.DateParameterName,
                                    ChartSettingsMVVM.StartDateViewModel.Date,
                                    ChartSettingsMVVM.EndDateViewModel.Date,
                                    pvm.GraphType,
                                    pvm.FillColor,
                                    pvm.PointColor
                                    );
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        break;
                    case "Trimestral":
                        try
                        {
                            var dayConfig = Mappers.Xy<DateModel>()
                                .X(dateModel => dateModel.Sequence)
                                .Y(dateModel => dateModel.Cost);

                            BIMStatsAppMVVM.Series = new SeriesCollection(dayConfig);

                            BIMStatsAppMVVM.MainView.PlanCostCartesianChart.Series = BIMStatsAppMVVM.Series;

                            BIMStatsAppMVVM.MainView.PlanCostCartesianChart.Update();

                            foreach (var pvm in ChartSettingsMVVM.AddedParameterViewModels)
                            {
                                SetParameterDefaultColor(pvm);

                                BIMStatsUIService.RefreshCostChartByQuarter(
                                    pvm.ParameterName,
                                    pvm.DateParameterName,
                                    ChartSettingsMVVM.StartDateViewModel.Date,
                                    ChartSettingsMVVM.EndDateViewModel.Date,
                                    pvm.GraphType,
                                    pvm.FillColor,
                                    pvm.PointColor
                                    );
                            }
                        }
                        catch { }
                        break;
                    case "Semestral":
                        try
                        {
                            var dayConfig = Mappers.Xy<DateModel>()
                                .X(dateModel => dateModel.Sequence)
                                .Y(dateModel => dateModel.Cost);

                            BIMStatsAppMVVM.Series = new SeriesCollection(dayConfig);

                            BIMStatsAppMVVM.MainView.PlanCostCartesianChart.Series = BIMStatsAppMVVM.Series;

                            BIMStatsAppMVVM.MainView.PlanCostCartesianChart.Update();

                            foreach (var pvm in ChartSettingsMVVM.AddedParameterViewModels)
                            {
                                SetParameterDefaultColor(pvm);

                                BIMStatsUIService.RefreshCostChartBySemester(
                                    pvm.ParameterName,
                                    pvm.DateParameterName,
                                    ChartSettingsMVVM.StartDateViewModel.Date,
                                    ChartSettingsMVVM.EndDateViewModel.Date,
                                    pvm.GraphType,
                                    pvm.FillColor,
                                    pvm.PointColor
                                    );
                            }
                        }
                        catch { }
                        break;
                    case "Anual":
                        try
                        {
                            var dayConfig = Mappers.Xy<DateModel>()
                                .X(dateModel => dateModel.Sequence)
                                .Y(dateModel => dateModel.Cost);

                            BIMStatsAppMVVM.Series = new SeriesCollection(dayConfig);

                            BIMStatsAppMVVM.MainView.PlanCostCartesianChart.Series = BIMStatsAppMVVM.Series;

                            BIMStatsAppMVVM.MainView.PlanCostCartesianChart.Update();

                            foreach (ParameterViewModel pvm in ChartSettingsMVVM.AddedParameterViewModels)
                            {
                                SetParameterDefaultColor(pvm);

                                BIMStatsUIService.RefreshCostChartByYear(
                                    pvm.ParameterName,
                                    pvm.DateParameterName,
                                    ChartSettingsMVVM.StartDateViewModel.Date,
                                    ChartSettingsMVVM.EndDateViewModel.Date,
                                    pvm.GraphType,
                                    pvm.FillColor,
                                    pvm.PointColor
                                    );
                            }
                        }
                        catch { }
                        break;
                }

                RestoreChartZoom();
            }
            catch { }
        }

        public static void SetParameterDefaultColor(ParameterViewModel pvm)
        {
            Brush fillColor = null;
            Brush pointColor = null;
            GetParameterDefaultColor(pvm.Name, out fillColor, out pointColor);
            pvm.FillColor = fillColor;
            pvm.PointColor = pointColor;
        }

        public static void PopulateListViewParameters()
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

            BIMStatsAppMVVM.ChartSettingsMVVM.ExcludedParameters_ListView.ItemsSource = ChartSettingsMVVM.ExcludedParameterViewModels;
            BIMStatsAppMVVM.ChartSettingsMVVM.AddedParameters_ListView.ItemsSource = ChartSettingsMVVM.AddedParameterViewModels;
        }
        public static void PopulateComboBoxAxisGradation()
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
            BIMStatsAppMVVM.ChartSettingsMVVM.GradacaoXAxis_CB.ItemsSource = ChartSettingsMVVM.GradationXAxisViewModels;
            BIMStatsAppMVVM.ChartSettingsMVVM.GradacaoXAxis_CB.DisplayMemberPath = "Gradation";
            BIMStatsAppMVVM.ChartSettingsMVVM.GradacaoXAxis_CB.SelectedItem = ChartSettingsMVVM.GradationXAxisViewModelSelected;
        }
        public static void RestoreChartZoom()
        {
            BIMStatsAppMVVM.MainView.PlanCostCartesianChart.AxisX[0].MinValue = double.NaN;
            BIMStatsAppMVVM.MainView.PlanCostCartesianChart.AxisX[0].MaxValue = double.NaN;
            BIMStatsAppMVVM.MainView.PlanCostCartesianChart.AxisY[0].MinValue = 0;
            BIMStatsAppMVVM.MainView.PlanCostCartesianChart.AxisY[0].MaxValue = double.NaN;
        }
        public static void AddSeries(
            string parameterName, 
            string dateParameter, 
            string seriesType,
            ChartValues<DateModel> chartValues,
            Brush chartFillColor, 
            Brush pointForegroundColor)
        {
            if (seriesType == "LineSeries")
            {
                BIMStatsAppMVVM.Series.Add(new LineSeries() {
                    Title = dateParameter + "\n/ " + parameterName,
                    Values = chartValues,
                    Fill = chartFillColor,
                    Foreground = chartFillColor,
                    Stroke = pointForegroundColor,
                    StrokeThickness = 2,
                    LineSmoothness = 1,
                    PointGeometrySize = 8,
                    PointForeground = pointForegroundColor,                    
                });
            }
            else if (seriesType == "ColumnSeries")
            {
                BIMStatsAppMVVM.Series.Add(new ColumnSeries() { Title = dateParameter + "\n/ " + parameterName, Values = chartValues });
            }
        }
        public static void AddParameterToCostChart(string parameterName)
        {
            try
            {
                List<ParameterViewModel> pvms = new List<ParameterViewModel>(ChartSettingsMVVM.ExcludedParameterViewModels);
                ParameterViewModel pvm = pvms.Find(x => x.Name.Contains(parameterName));
                int index = pvms.IndexOf(pvm);
                ChartSettingsMVVM.AddedParameterViewModels.Add(ChartSettingsMVVM.ExcludedParameterViewModels[index]);
                ChartSettingsMVVM.ExcludedParameterViewModels.Remove(ChartSettingsMVVM.ExcludedParameterViewModels[index]);
            }
            catch { }
        }
        public static string QuarterFormatter(DateTime currentDate)
        {
            string resultValue = "invalid";

            int year = currentDate.Year;
            int month = currentDate.Month;

            if (month == 1)
            {
                resultValue = "1º Tri-" + year.ToString();
            }
            else if (month == 4)
            {
                resultValue = "2º Tri-" + year.ToString();
            }
            else if (month == 7)
            {
                resultValue = "3º Tri-" + year.ToString();
            }
            else if (month == 10)
            {
                resultValue = "4º Tri-" + year.ToString();
            }

            return resultValue;
        }
        public static string SemesterFormatter(DateTime currentDate)
        {
            string resultValue = "invalid";

            int year = currentDate.Year;
            int month = currentDate.Month;

            if (month <= 6)
            {
                resultValue = "1º Semestre-" + year.ToString();
            }
            else
            {
                resultValue = "2º Semestre-" + year.ToString();
            }

            return resultValue;
        }
        public static string MonthFormatter(DateTime currentDate)
        {
            int year = currentDate.Year;
            int month = currentDate.Month;
            string resultValue = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) + "-" + year.ToString();
            return resultValue;
        }
        public static string TwoWeeksFormatter(DateTime currentDate)
        {
            int year = currentDate.Year;
            int month = currentDate.Month;
            int day = currentDate.Day;
            string resultValue = "invalid";

            try
            {
                if (day <= 15)
                {
                    resultValue = "1-" + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month) + "-" + year.ToString().Substring(year.ToString().Length - 2);
                }
                else
                {
                    resultValue = "2-" + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month) + "-" + year.ToString().Substring(year.ToString().Length - 2);
                }
            }
            catch { }

            return resultValue;
        }

        public static string FormatterConverter(double value)
        {
            string dateValue = "";

            try
            {
                GradationXAxisViewModel selectedGroupMode = BIMStatsAppMVVM.ChartSettingsMVVM.GradacaoXAxis_CB.SelectedItem as GradationXAxisViewModel;

                switch (selectedGroupMode.Gradation)
                {
                    case "Diário":
                        dateValue = new System.DateTime((long)(value * TimeSpan.FromDays(1).Ticks)).ToString("dd/MM/yy");
                        break;
                    case "Quinzenal":
                        dateValue = TwoWeeksFormatter(BIMStatsAppMVVM.ChartValues.First(x => x.Sequence == value).DateTime);
                        break;
                    case "Mensal":
                        dateValue = MonthFormatter(BIMStatsAppMVVM.ChartValues.First(x => x.Sequence == value).DateTime);
                        break;
                    case "Trimestral":
                        dateValue = QuarterFormatter(BIMStatsAppMVVM.ChartValues.First(x => x.Sequence == value).DateTime);
                        break;
                    case "Semestral":
                        dateValue = SemesterFormatter(BIMStatsAppMVVM.ChartValues.First(x => x.Sequence == value).DateTime);
                        break;
                    case "Anual":
                        dateValue = BIMStatsAppMVVM.ChartValues.First(x => x.Sequence == value).DateTime.ToString("yyyy");
                        break;
                }

                RestoreChartZoom();
            }
            catch { }


            return dateValue;
        }
        public static void GetParameterDefaultColor(string parameterName, out Brush fillColor, out Brush pointColor)
        {
            fillColor = new SolidColorBrush(Colors.LightSlateGray);
            pointColor = new SolidColorBrush(Colors.LightSlateGray);

            switch (parameterName)
            {
                case "Custo total | Fim planejado":
                    fillColor = new SolidColorBrush(Colors.IndianRed);
                    fillColor.Opacity = 0.3;
                    pointColor = new SolidColorBrush(Colors.IndianRed);
                    pointColor.Opacity = 1;
                    break;
                case "Custo total | Fim executado":
                    fillColor = new SolidColorBrush(Colors.DimGray);
                    fillColor.Opacity = 0.3;
                    pointColor = new SolidColorBrush(Colors.DimGray);
                    pointColor.Opacity = 1;
                    break;
                case "Material | Fim planejado":
                    fillColor = new SolidColorBrush(Colors.DarkOliveGreen);
                    fillColor.Opacity = 0.3;
                    pointColor = new SolidColorBrush(Colors.DarkOliveGreen);
                    pointColor.Opacity = 1;
                    break;
                case "Material | Fim executado":
                    fillColor = new SolidColorBrush(Colors.Chocolate);
                    fillColor.Opacity = 0.3;
                    pointColor = new SolidColorBrush(Colors.Chocolate);
                    pointColor.Opacity = 1;
                    break;
                case "Mão-de-obra | Fim planejado":
                    fillColor = new SolidColorBrush(Colors.MediumBlue);
                    fillColor.Opacity = 0.3;
                    pointColor = new SolidColorBrush(Colors.MediumBlue);
                    pointColor.Opacity = 1;
                    break;
                case "Mão-de-obra | Fim executado":
                    fillColor = new SolidColorBrush(Colors.Black);
                    fillColor.Opacity = 0.3;
                    pointColor = new SolidColorBrush(Colors.Black);
                    pointColor.Opacity = 1;
                    break;
                case "Equipamentos | Fim planejado":
                    fillColor = new SolidColorBrush(Colors.DeepSkyBlue);
                    fillColor.Opacity = 0.3;
                    pointColor = new SolidColorBrush(Colors.DeepSkyBlue);
                    pointColor.Opacity = 1;
                    break;
                case "Equipamentos | Fim executado":
                    fillColor = new SolidColorBrush(Colors.Salmon);
                    fillColor.Opacity = 0.3;
                    pointColor = new SolidColorBrush(Colors.Salmon);
                    pointColor.Opacity = 1;
                    break;
                case "Terceirizada | Fim planejado":
                    fillColor = new SolidColorBrush(Colors.Indigo);
                    fillColor.Opacity = 0.3;
                    pointColor = new SolidColorBrush(Colors.Indigo);
                    pointColor.Opacity = 1;
                    break;
                case "Terceirizada | Fim executado":
                    fillColor = new SolidColorBrush(Colors.Orange);
                    fillColor.Opacity = 0.3;
                    pointColor = new SolidColorBrush(Colors.Orange);
                    pointColor.Opacity = 1;
                    break;
                case "Progresso | Fim planejado":
                    fillColor = new SolidColorBrush(Colors.Fuchsia);
                    fillColor.Opacity = 0.3;
                    pointColor = new SolidColorBrush(Colors.Fuchsia);
                    pointColor.Opacity = 1;
                    break;
                case "Progresso | Fim executado":
                    fillColor = new SolidColorBrush(Colors.Firebrick);
                    fillColor.Opacity = 0.3;
                    pointColor = new SolidColorBrush(Colors.Firebrick);
                    pointColor.Opacity = 1;
                    break;
            }
        }
    }
}
