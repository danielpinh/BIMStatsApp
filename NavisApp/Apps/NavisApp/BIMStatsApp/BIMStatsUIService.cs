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
        public static void SetGaugeChartValues()
        {
            DocumentTimeliner documentTimeliner = Autodesk.Navisworks.Api.Application.MainDocument.GetTimeliner();

            BIMStatsAppMVVM.CostViewModel.TotalCost = NavisUtils.GetTotalCost();
            BIMStatsAppMVVM.CostViewModel.TotalCostCurrency = BIMStatsAppMVVM.CostViewModel.TotalCost.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"));
            BIMStatsAppMVVM.MainView.TotalCost_TextBlock.Text = BIMStatsAppMVVM.CostViewModel.TotalCostCurrency;

            BIMStatsAppMVVM.CostViewModel.PlannedPartialCost = NavisUtils.GetCostByDateTimeRange(documentTimeliner,
                NavisApp.Properties.NavisworksParameters.TotalCost,
                NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                PlannedExecutedChartSettingsMVVM.StartDateViewModel.Date,
                PlannedExecutedChartSettingsMVVM.EndDateViewModel.Date);

            BIMStatsAppMVVM.CostViewModel.ExecutedPartialCost = NavisUtils.GetCostByDateTimeRange(documentTimeliner,
                NavisApp.Properties.NavisworksParameters.TotalCost,
                NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                PlannedExecutedChartSettingsMVVM.StartDateViewModel.Date,
                PlannedExecutedChartSettingsMVVM.EndDateViewModel.Date);

            double balance = BIMStatsAppMVVM.CostViewModel.PlannedPartialCost - BIMStatsAppMVVM.CostViewModel.ExecutedPartialCost;

            if (balance < 0)
            {
                BIMStatsAppMVVM.MainView.DiffCost_TextBlock.Foreground = new SolidColorBrush(Colors.IndianRed);
            }
            else if (balance == 0)
            {
                BIMStatsAppMVVM.MainView.DiffCost_TextBlock.Foreground = new SolidColorBrush(Colors.Black);
            }
            else 
            {
                BIMStatsAppMVVM.MainView.DiffCost_TextBlock.Foreground = new SolidColorBrush(Colors.ForestGreen);
            }

            BIMStatsAppMVVM.MainView.DiffCost_TextBlock.Text = balance.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"));
        }
        public static void RefreshPlannedExecutedChartByYear(string parameterName, string dateParameter, DateTime startDateTime, DateTime endDateTime, string seriesType, Brush colorFill, Brush colorPoint)
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
                    SetPlannedExecutedChartSeries(parameterName, dateParameter, startDateTime, endDateTime, seriesType, documentTimeliner, chartValues, colorFill, colorPoint);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void RefreshPlannedExecutedChartBySemester(string parameterName, string dateParameter, DateTime startDateTime, DateTime endDateTime, string seriesType, Brush colorFill, Brush colorPoint)
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
                    SetPlannedExecutedChartSeries(parameterName, dateParameter, startDateTime, endDateTime, seriesType, documentTimeliner, chartValues, colorFill, colorPoint);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void RefreshPlannedExecutedChartByQuarter(string parameterName, string dateParameter, DateTime startDateTime, DateTime endDateTime, string seriesType, Brush colorFill, Brush colorPoint)
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

                    SetPlannedExecutedChartSeries(parameterName, dateParameter, startDateTime, endDateTime, seriesType, documentTimeliner, chartValues, colorFill, colorPoint);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void RefreshPlannedExecutedChartByTwoWeeks(string parameterName, string dateParameter, DateTime startDateTime, DateTime endDateTime, string seriesType, Brush colorFill, Brush colorPoint)
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
                    SetPlannedExecutedChartSeries(parameterName, dateParameter, startDateTime, endDateTime, seriesType, documentTimeliner, chartValues, colorFill, colorPoint);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("RefreshCostChartByTwoWeeks\n\n" + ex.Message + ex.TargetSite);
            }
        }
        public static void RefreshPlannedExecutedChartByMonth(string parameterName, string dateParameter, DateTime startDateTime, DateTime endDateTime, string seriesType, Brush colorFill, Brush colorPoint)
        {
            try
            {
                // Iterate over any existing Timeliner Tasks
                DocumentTimeliner documentTimeliner = Autodesk.Navisworks.Api.Application.MainDocument.GetTimeliner();

                ChartValues<DateModel> chartValues = NavisUtils.GetTimeLinerParametersByMonth(documentTimeliner, parameterName,
                    dateParameter, startDateTime, endDateTime);

                // Add zero values to date time gaps 
                chartValues = NavisUtils.FixPlannedExecutedChartDateTimeGapsByMonth(chartValues, startDateTime, endDateTime);

                // Reorder ChartValues
                chartValues = ChartUtils.SortChartValuesByDateTime(chartValues);

                if (chartValues.Count > 0)
                {
                    SetPlannedExecutedChartSeries(parameterName, dateParameter, startDateTime, endDateTime, seriesType, documentTimeliner, chartValues, colorFill, colorPoint);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void RefreshSCurveChartByMonth(string parameterName, string dateParameter, DateTime startDateTime, DateTime endDateTime, string seriesType, Brush colorFill, Brush colorPoint)
        {
            try
            {
                // Iterate over any existing Timeliner Tasks
                DocumentTimeliner documentTimeliner = Autodesk.Navisworks.Api.Application.MainDocument.GetTimeliner();

                ChartValues<DateModel> chartValues = NavisUtils.GetAccumulatedTimeLinerParametersByMonth(documentTimeliner, parameterName,
                    dateParameter, startDateTime, endDateTime);

                // Add zero values to date time gaps 
                chartValues = NavisUtils.FixSCurveChartDateTimeGapsByMonth(chartValues, startDateTime, endDateTime);

                // Reorder ChartValues
                chartValues = ChartUtils.SortChartValuesByDateTime(chartValues);

                if (chartValues.Count > 0)
                {
                    SetSCurveChartSeries(parameterName, dateParameter,  seriesType, documentTimeliner, chartValues, colorFill, colorPoint);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void RefreshPlannedExecutedChartByDay(string parameterName, string dateParameter, DateTime startDateTime, DateTime endDateTime, string seriesType, Brush colorFill, Brush colorPoint)
        {
            try
            {
                // Iterate over any existing Timeliner Tasks
                DocumentTimeliner documentTimeliner = Autodesk.Navisworks.Api.Application.MainDocument.GetTimeliner();

                ChartValues<DateModel> chartValues = NavisUtils.GetTimeLinerParametersByDay(documentTimeliner, parameterName,
                    dateParameter, startDateTime, endDateTime);

                if (chartValues.Count > 0)
                {
                    SetPlannedExecutedChartSeries(parameterName, dateParameter, startDateTime, endDateTime, seriesType, documentTimeliner, chartValues, colorFill, colorPoint);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("RefreshCostChartByDay\n\n"+ ex.Message + ex.TargetSite);
            }
        }
        public static void SetPlannedExecutedChartSeries(
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

            BIMStatsAppMVVM.MainView.PlannedExecutedChartXAxis.LabelFormatter = BIMStatsAppMVVM.DateFormatter;

            BIMStatsAppMVVM.MainView.PlannedExecutedChart.BringIntoView();
        }
        public static void SetSCurveChartSeries(
            string parameterName,
            string dateParameter,
            string seriesType,
            DocumentTimeliner documentTimeliner,
            ChartValues<DateModel> chartValues,
            Brush chartFillColor,
            Brush pointForegroundColor)
        {
            AddSeries(parameterName, dateParameter, seriesType, chartValues, chartFillColor, pointForegroundColor);

            BIMStatsAppMVVM.MainView.SCurveCartesianChartXAxis.LabelFormatter = BIMStatsAppMVVM.DateFormatter;

            BIMStatsAppMVVM.MainView.SCurveCartesianChart.BringIntoView();
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
        public static void RefreshSCurveChart()
        {
            var dayConfig = Mappers.Xy<DateModel>()
                               .X(dateModel => dateModel.Sequence)
                               .Y(dateModel => dateModel.Cost);

            BIMStatsAppMVVM.Series = new SeriesCollection(dayConfig);

            BIMStatsAppMVVM.MainView.SCurveCartesianChart.Series = BIMStatsAppMVVM.Series;

            BIMStatsAppMVVM.MainView.SCurveCartesianChart.Update();

            Brush fillColor1 = null;
            Brush pointColor1 = null;
            GetParameterDefaultColor(NavisApp.Properties.NavisworksParameters.TotalCost + " | " + NavisApp.Properties.NavisworksParameters.ActualEndParameter, out fillColor1, out pointColor1);

            Brush fillColor2 = null;
            Brush pointColor2 = null;
            GetParameterDefaultColor(NavisApp.Properties.NavisworksParameters.TotalCost + " | " + NavisApp.Properties.NavisworksParameters.PlannedEndParameter, out fillColor2, out pointColor2);

            BIMStatsUIService.RefreshSCurveChartByMonth(
                NavisApp.Properties.NavisworksParameters.TotalCost,
                NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                PlannedExecutedChartSettingsMVVM.StartDateViewModel.Date,
                PlannedExecutedChartSettingsMVVM.EndDateViewModel.Date,
                "LineSeries",
                fillColor1,
                pointColor1
                );

            BIMStatsUIService.RefreshSCurveChartByMonth(
               NavisApp.Properties.NavisworksParameters.TotalCost,
               NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
               PlannedExecutedChartSettingsMVVM.StartDateViewModel.Date,
               PlannedExecutedChartSettingsMVVM.EndDateViewModel.Date,
               "LineSeries",
               fillColor2,
               pointColor2
               );

            RestoreSCurveChartZoom();
        }
        public static void RefreshPlannedExecutedChart()
        {
            try
            {
                GradationXAxisViewModel selectedGroupMode = BIMStatsAppMVVM.PlannedExecutedChartSettingsMVVM.GradacaoXAxis_CB.SelectedItem as GradationXAxisViewModel;

                switch (selectedGroupMode.Gradation)
                {
                    case "Diário":

                        try
                        {
                            var dayConfig = Mappers.Xy<DateModel>()
                                 .X(dateModel => dateModel.DateTime.Ticks / TimeSpan.FromDays(1).Ticks)
                                 .Y(dateModel => dateModel.Cost);

                            BIMStatsAppMVVM.Series = new SeriesCollection(dayConfig);

                            BIMStatsAppMVVM.MainView.PlannedExecutedChart.Series = BIMStatsAppMVVM.Series;

                            BIMStatsAppMVVM.MainView.PlannedExecutedChart.Update();

                            foreach (var pvm in PlannedExecutedChartSettingsMVVM.AddedParameterViewModels)
                            {
                                SetParameterDefaultColor(pvm);

                                BIMStatsUIService.RefreshPlannedExecutedChartByDay(
                                    pvm.ParameterName,
                                    pvm.DateParameterName,
                                    PlannedExecutedChartSettingsMVVM.StartDateViewModel.Date,
                                    PlannedExecutedChartSettingsMVVM.EndDateViewModel.Date,
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

                            BIMStatsAppMVVM.MainView.PlannedExecutedChart.Series = BIMStatsAppMVVM.Series;

                            BIMStatsAppMVVM.MainView.PlannedExecutedChart.Update();

                            foreach (var pvm in PlannedExecutedChartSettingsMVVM.AddedParameterViewModels)
                            {
                                SetParameterDefaultColor(pvm);

                                BIMStatsUIService.RefreshPlannedExecutedChartByTwoWeeks(
                                    pvm.ParameterName,
                                    pvm.DateParameterName,
                                    PlannedExecutedChartSettingsMVVM.StartDateViewModel.Date,
                                    PlannedExecutedChartSettingsMVVM.EndDateViewModel.Date,
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

                            BIMStatsAppMVVM.MainView.PlannedExecutedChart.Series = BIMStatsAppMVVM.Series;

                            BIMStatsAppMVVM.MainView.PlannedExecutedChart.Update();

                            foreach (var pvm in PlannedExecutedChartSettingsMVVM.AddedParameterViewModels)
                            {
                                SetParameterDefaultColor(pvm);

                                BIMStatsUIService.RefreshPlannedExecutedChartByMonth(
                                    pvm.ParameterName,
                                    pvm.DateParameterName,
                                    PlannedExecutedChartSettingsMVVM.StartDateViewModel.Date,
                                    PlannedExecutedChartSettingsMVVM.EndDateViewModel.Date,
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

                            BIMStatsAppMVVM.MainView.PlannedExecutedChart.Series = BIMStatsAppMVVM.Series;

                            BIMStatsAppMVVM.MainView.PlannedExecutedChart.Update();

                            foreach (var pvm in PlannedExecutedChartSettingsMVVM.AddedParameterViewModels)
                            {
                                SetParameterDefaultColor(pvm);

                                BIMStatsUIService.RefreshPlannedExecutedChartByQuarter(
                                    pvm.ParameterName,
                                    pvm.DateParameterName,
                                    PlannedExecutedChartSettingsMVVM.StartDateViewModel.Date,
                                    PlannedExecutedChartSettingsMVVM.EndDateViewModel.Date,
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

                            BIMStatsAppMVVM.MainView.PlannedExecutedChart.Series = BIMStatsAppMVVM.Series;

                            BIMStatsAppMVVM.MainView.PlannedExecutedChart.Update();

                            foreach (var pvm in PlannedExecutedChartSettingsMVVM.AddedParameterViewModels)
                            {
                                SetParameterDefaultColor(pvm);

                                BIMStatsUIService.RefreshPlannedExecutedChartBySemester(
                                    pvm.ParameterName,
                                    pvm.DateParameterName,
                                    PlannedExecutedChartSettingsMVVM.StartDateViewModel.Date,
                                    PlannedExecutedChartSettingsMVVM.EndDateViewModel.Date,
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

                            BIMStatsAppMVVM.MainView.PlannedExecutedChart.Series = BIMStatsAppMVVM.Series;

                            BIMStatsAppMVVM.MainView.PlannedExecutedChart.Update();

                            foreach (ParameterViewModel pvm in PlannedExecutedChartSettingsMVVM.AddedParameterViewModels)
                            {
                                SetParameterDefaultColor(pvm);

                                BIMStatsUIService.RefreshPlannedExecutedChartByYear(
                                    pvm.ParameterName,
                                    pvm.DateParameterName,
                                    PlannedExecutedChartSettingsMVVM.StartDateViewModel.Date,
                                    PlannedExecutedChartSettingsMVVM.EndDateViewModel.Date,
                                    pvm.GraphType,
                                    pvm.FillColor,
                                    pvm.PointColor
                                    );
                            }
                        }
                        catch { }
                        break;
                }

                RestorePlannedExecutedChartZoom();
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
            PlannedExecutedChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.TotalCost + " | " + NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.TotalCost,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    GraphType = "LineSeries"
                });
            PlannedExecutedChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.TotalCost + " | " + NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.TotalCost,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    GraphType = "LineSeries"
                });
            PlannedExecutedChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel 
                { Name = NavisApp.Properties.NavisworksParameters.MaterialCost + " | " + NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.MaterialCost,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    GraphType = "LineSeries" });
            PlannedExecutedChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.MaterialCost + " | " + NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.MaterialCost,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    GraphType = "LineSeries"
                });
            PlannedExecutedChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.LaborCost + " | " + NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.LaborCost,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    GraphType = "LineSeries"
                });
            PlannedExecutedChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.LaborCost + " | " + NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.LaborCost,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    GraphType = "LineSeries"
                });
            PlannedExecutedChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.EquipmentCost + " | " + NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.EquipmentCost,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    GraphType = "LineSeries"
                });
            PlannedExecutedChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.EquipmentCost + " | " + NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.EquipmentCost,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    GraphType = "LineSeries"
                });
            PlannedExecutedChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.SubcontractorCost + " | " + NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.SubcontractorCost,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    GraphType = "LineSeries"
                });
            PlannedExecutedChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.SubcontractorCost + " | " + NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.SubcontractorCost,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    GraphType = "LineSeries"
                });
            PlannedExecutedChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.Provided_Progress + " | " + NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.Provided_Progress,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.PlannedEndParameter,
                    GraphType = "LineSeries"
                });
            PlannedExecutedChartSettingsMVVM.ExcludedParameterViewModels
                .Add(new ParameterViewModel
                {
                    Name = NavisApp.Properties.NavisworksParameters.Provided_Progress + " | " + NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    ParameterName = NavisApp.Properties.NavisworksParameters.Provided_Progress,
                    DateParameterName = NavisApp.Properties.NavisworksParameters.ActualEndParameter,
                    GraphType = "LineSeries"
                });

            BIMStatsAppMVVM.PlannedExecutedChartSettingsMVVM.ExcludedParameters_ListView.ItemsSource = PlannedExecutedChartSettingsMVVM.ExcludedParameterViewModels;
            BIMStatsAppMVVM.PlannedExecutedChartSettingsMVVM.AddedParameters_ListView.ItemsSource = PlannedExecutedChartSettingsMVVM.AddedParameterViewModels;
        }
        public static void PopulateComboBoxAxisGradation()
        {
            //PlannedExecutedChartSettingsMVVM
            PlannedExecutedChartSettingsMVVM.GradationXAxisViewModels.Add(new GradationXAxisViewModel { Gradation = "Diário" });
            PlannedExecutedChartSettingsMVVM.GradationXAxisViewModels.Add(new GradationXAxisViewModel { Gradation = "Quinzenal" });
            PlannedExecutedChartSettingsMVVM.GradationXAxisViewModels.Add(new GradationXAxisViewModel { Gradation = "Mensal" });
            PlannedExecutedChartSettingsMVVM.GradationXAxisViewModels.Add(new GradationXAxisViewModel { Gradation = "Trimestral" });
            PlannedExecutedChartSettingsMVVM.GradationXAxisViewModels.Add(new GradationXAxisViewModel { Gradation = "Semestral" });
            PlannedExecutedChartSettingsMVVM.GradationXAxisViewModels.Add(new GradationXAxisViewModel { Gradation = "Anual" });

            //Gradation X Axis types
            PlannedExecutedChartSettingsMVVM.GradationXAxisViewModelSelected = PlannedExecutedChartSettingsMVVM.GradationXAxisViewModels[0];
            BIMStatsAppMVVM.PlannedExecutedChartSettingsMVVM.GradacaoXAxis_CB.ItemsSource = PlannedExecutedChartSettingsMVVM.GradationXAxisViewModels;
            BIMStatsAppMVVM.PlannedExecutedChartSettingsMVVM.GradacaoXAxis_CB.DisplayMemberPath = "Gradation";
            BIMStatsAppMVVM.PlannedExecutedChartSettingsMVVM.GradacaoXAxis_CB.SelectedItem = PlannedExecutedChartSettingsMVVM.GradationXAxisViewModelSelected;
        }
        public static void RestorePlannedExecutedChartZoom()
        {
            BIMStatsAppMVVM.MainView.PlannedExecutedChart.AxisX[0].MinValue = double.NaN;
            BIMStatsAppMVVM.MainView.PlannedExecutedChart.AxisX[0].MaxValue = double.NaN;
            BIMStatsAppMVVM.MainView.PlannedExecutedChart.AxisY[0].MinValue = 0;
            BIMStatsAppMVVM.MainView.PlannedExecutedChart.AxisY[0].MaxValue = double.NaN;
        }
        public static void RestoreSCurveChartZoom()
        {
            BIMStatsAppMVVM.MainView.SCurveCartesianChart.AxisX[0].MinValue = double.NaN;
            BIMStatsAppMVVM.MainView.SCurveCartesianChart.AxisX[0].MaxValue = double.NaN;
            BIMStatsAppMVVM.MainView.SCurveCartesianChart.AxisY[0].MinValue = 0;
            BIMStatsAppMVVM.MainView.SCurveCartesianChart.AxisY[0].MaxValue = double.NaN;
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
                    LineSmoothness = 0.5,
                    PointGeometrySize = 8,
                    PointForeground = pointForegroundColor,                    
                });
            }
            else if (seriesType == "ColumnSeries")
            {
                BIMStatsAppMVVM.Series.Add(new ColumnSeries() { Title = dateParameter + "\n/ " + parameterName, Values = chartValues });
            }
        }
        public static void AddParameterToPlannedExecutedChart(string parameterName)
        {
            try
            {
                List<ParameterViewModel> pvms = new List<ParameterViewModel>(PlannedExecutedChartSettingsMVVM.ExcludedParameterViewModels);
                ParameterViewModel pvm = pvms.Find(x => x.Name.Contains(parameterName));
                int index = pvms.IndexOf(pvm);
                PlannedExecutedChartSettingsMVVM.AddedParameterViewModels.Add(PlannedExecutedChartSettingsMVVM.ExcludedParameterViewModels[index]);
                PlannedExecutedChartSettingsMVVM.ExcludedParameterViewModels.Remove(PlannedExecutedChartSettingsMVVM.ExcludedParameterViewModels[index]);
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
        public static string CurrencyFormatterConverter(double value)
        {
            string result = "";

            try
            {

                result = value.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"));

            }
            catch { }

            return result;
        }

        public static string DateFormatterConverter(double value)
        {
            string dateValue = "";

            try
            {
                GradationXAxisViewModel selectedGroupMode = BIMStatsAppMVVM.PlannedExecutedChartSettingsMVVM.GradacaoXAxis_CB.SelectedItem as GradationXAxisViewModel;

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

                RestorePlannedExecutedChartZoom();
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
