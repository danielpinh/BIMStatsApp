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
using System;
using LiveCharts.Definitions.Series;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Linq;
using System.Globalization;


namespace NavisApp
{
    public class Utils
    {
        public static DateTime GetSimulationCurrentDate()
        {
            DateTime currentDateTime = new DateTime();

            DockPanePlugin TimeLinerDockPane = GetTimeLinerDockPane(true);
            if (TimeLinerDockPane == null) return currentDateTime;

            foreach (var prop in TimeLinerDockPane.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if ("VM" == prop.Name)
                {
                    object obj = prop.GetValue(TimeLinerDockPane, null);

                    foreach (var method in obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if ("get_Tabs" == method.Name)
                        {
                            // Get tab list
                            object tabs = method.Invoke(obj, null);
                            IList tabsList = (IList)tabs;

                            // Set simulation tab selected
                            foreach (var tab in tabsList)
                            {
                                string TabName = tab.GetType().GetProperty("Uid").GetValue(tab).ToString();

                                if (TabName == "SimulateTabVM")
                                {
                                    tab.GetType().GetProperty("IsSelected").SetValue(tab, true);

                                    // iterate controls in simulate tab and set date
                                    foreach (var property in tab.GetType().GetProperties())
                                    {
                                        if (property.Name == "CurrentTime")
                                        {
                                            currentDateTime = (DateTime)tab.GetType().GetProperty("CurrentTime").GetValue(tab);
                                            break;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
            return currentDateTime;
        }
        public static void GoToSimulationMode(DateTime Day)
        {
            DockPanePlugin TimeLinerDockPane = GetTimeLinerDockPane(true);
            if (TimeLinerDockPane == null) return;

            foreach (var prop in TimeLinerDockPane.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if ("VM" == prop.Name)
                {
                    object obj = prop.GetValue(TimeLinerDockPane, null);

                    foreach (var method in obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if ("get_Tabs" == method.Name)
                        {
                            // Get tab list
                            object tabs = method.Invoke(obj, null);
                            IList tabsList = (IList)tabs;

                            // Set simulation tab selected
                            foreach (var tab in tabsList)
                            {
                                string TabName = tab.GetType().GetProperty("Uid").GetValue(tab).ToString();

                                if (TabName == "SimulateTabVM")
                                {
                                    tab.GetType().GetProperty("IsSelected").SetValue(tab, true);

                                    // iterate controls in simulate tab and set date
                                    foreach (var property in tab.GetType().GetProperties())
                                    {
                                        if (property.Name == "CurrentTime")
                                        {
                                            tab.GetType().GetProperty("CurrentTime").SetValue(tab, Day);
                                            break;
                                        }
                                    }
                                }
                                else tab.GetType().GetProperty("IsSelected").SetValue(tab, false);
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        }
        private static DockPanePlugin GetTimeLinerDockPane(bool MakeVisible)
        {
            DockPanePlugin TimeLinerDockPane = null;

            foreach (PluginRecord pr in Autodesk.Navisworks.Api.Application.Plugins.PluginRecords)
            {
                if ("TimelinerDockPane" == pr.Name)
                {
                    if (pr is DockPanePluginRecord && pr.IsEnabled)
                    {
                        if (pr.LoadedPlugin == null)
                        {
                            pr.LoadPlugin();
                        }

                        TimeLinerDockPane = pr.LoadedPlugin as DockPanePlugin;
                        if (TimeLinerDockPane != null)
                        {
                            if (!TimeLinerDockPane.Visible)
                            {
                                TimeLinerDockPane.Visible = true;
                            }
                            break;
                        }
                    }
                }
            }

            return TimeLinerDockPane;
        }
        public static double GetPlannedPartialCost(DocumentTimeliner documentTimeliner, DateTime startDate, DateTime endDate)
        {
            double totalCost = 0;

            //Get parameter values
            foreach (TimelinerTask task in documentTimeliner.Tasks)
            {
                foreach (TimelinerTask subTask in task.Children)
                {
                    try
                    {
                        DateModel dModel = GetTimeLinerParameterValuesByParameter(subTask, NavisApp.Properties.NavisworksParameters.TotalCost, NavisApp.Properties.NavisworksParameters.PlannedEndParameter);

                        int dateTimeCompareStart = DateTime.Compare(dModel.DateTime, startDate);
                        int dateTimeCompareEnd = DateTime.Compare(dModel.DateTime, endDate);

                        if (dateTimeCompareStart >= 0 && dateTimeCompareEnd <= 0)
                        {
                            totalCost += dModel.Cost;
                        }
                    }
                    catch { }
                }
            }
            return totalCost;
        }
        public static List<DateTime> GetTimeLinerDates(DocumentTimeliner documentTimeliner, string parameterName)
        {
            List<DateTime> dateTimeList = new List<DateTime>();

            //Get parameter values
            foreach (TimelinerTask task in documentTimeliner.Tasks)
            {
                try
                {
                    dateTimeList.Add(GetTimeLinerDateTime(task, parameterName));
                }
                catch { }


                foreach (TimelinerTask subTask in task.Children)
                {
                    try
                    {
                        dateTimeList.Add(GetTimeLinerDateTime(subTask, parameterName));
                    }
                    catch { }
                }
            }

            return dateTimeList;
        }
        public static ChartValues<DateModel> GetTimeLinerYearsByRangeByParameter(DocumentTimeliner documentTimeliner, string parameterName, string dateParameter, DateTime startDate, DateTime endDate)
        {
            ChartValues<DateModel> chartValues = new ChartValues<DateModel>();

            var dateModels = new List<DateModel>();

            GetDatesByRange(documentTimeliner, parameterName, dateParameter, startDate, endDate, dateModels);

            //Group by year
            var dateModelGroupByYear = dateModels.GroupBy(x => x.DateTime.Year);

            foreach (var groupByYear in dateModelGroupByYear)
            {
                int yearKey = groupByYear.Key;

                List<DateModel> dModels = groupByYear.ToList();

                double totalYearCost = 0;

                foreach (var item in groupByYear)
                {
                    totalYearCost += item.Cost;
                }

                DateModel dateModel = new DateModel();
                dateModel.DateTime = new DateTime(yearKey, 06, 15);
                dateModel.Cost = totalYearCost;

                chartValues.Add(dateModel);
            }

            return chartValues;
        }
        public static ChartValues<DateModel> GetTimeLinerSemestersByRangeByParametr(DocumentTimeliner documentTimeliner, string parameterName, string dateParameter, DateTime startDate, DateTime endDate)
        {
            ChartValues<DateModel> chartValues = new ChartValues<DateModel>();

            var dateModels = new List<DateModel>();

            GetDatesByRange(documentTimeliner, parameterName, dateParameter, startDate, endDate, dateModels);

            //Group by year
            var dateModelGroupByYear = dateModels
                .GroupBy(x => x.DateTime.Year);

            foreach (var groupByYear in dateModelGroupByYear)
            {
                int yearKey = groupByYear.Key;

                List<DateModel> dModels = groupByYear.ToList();

                //Group by Month
                var dateModelGroupByMonth = dModels
                    .GroupBy(x => x.DateTime.Month);

                double firstSemesterTotalCost = 0;
                double secondSemesterTotalCost = 0;

                foreach (var group in dateModelGroupByMonth)
                {
                    int key = group.Key;

                    if (key > 6)
                    {
                        foreach (var item in group)
                        {
                            secondSemesterTotalCost += item.Cost;
                        }
                    }
                    else
                    {
                        foreach (var item in group)
                        {
                            firstSemesterTotalCost += item.Cost;
                        }
                    }
                }

                if (firstSemesterTotalCost > 0)
                {
                    DateModel dateModel = new DateModel();
                    dateModel.DateTime = new DateTime(yearKey, 03, 15);
                    dateModel.Cost = firstSemesterTotalCost;
                    chartValues.Add(dateModel);
                }
                if (secondSemesterTotalCost > 0)
                {
                    DateModel dateModel = new DateModel();
                    dateModel.DateTime = new DateTime(yearKey, 09, 15);
                    dateModel.Cost = secondSemesterTotalCost;
                    chartValues.Add(dateModel);
                }
            }

            return chartValues;
        }
        public static ChartValues<DateModel> GetTimeLinerQuartersByRangeByParameter(DocumentTimeliner documentTimeliner, string parameterName, string dateParameter, DateTime startDate, DateTime endDate)
        {
            ChartValues<DateModel> chartValues = new ChartValues<DateModel>();

            var dateModels = new List<DateModel>();

            GetDatesByRange(documentTimeliner, parameterName, dateParameter, startDate, endDate, dateModels);

            //Group by year
            var dateModelGroupByYear = dateModels
                .GroupBy(x => x.DateTime.Year);

            foreach (var groupByYear in dateModelGroupByYear)
            {
                int yearKey = groupByYear.Key;

                List<DateModel> dModels = groupByYear.ToList();

                //Group by Month
                var dateModelGroupByMonth = dModels
                    .GroupBy(x => x.DateTime.Month);

                double firstQuarterTotalCost = 0;
                double secondQuarterTotalCost = 0;
                double thirdQuarterTotalCost = 0;
                double fourthQuarterTotalCost = 0;

                //Group by quarter
                foreach (var group in dateModelGroupByMonth)
                {
                    int monthKey = group.Key;

                    if (monthKey <= 3)
                    {
                        foreach (var item in group)
                        {
                            firstQuarterTotalCost += item.Cost;
                        }
                    }
                    else if (monthKey <= 6)
                    {
                        foreach (var item in group)
                        {
                            secondQuarterTotalCost += item.Cost;
                        }
                    }
                    else if (monthKey <= 9)
                    {
                        foreach (var item in group)
                        {
                            thirdQuarterTotalCost += item.Cost;
                        }
                    }
                    else
                    {
                        foreach (var item in group)
                        {
                            fourthQuarterTotalCost += item.Cost;
                        }
                    }
                }

                if (firstQuarterTotalCost > 0)
                {
                    DateModel dateModel = new DateModel();
                    dateModel.DateTime = new DateTime(yearKey, 01, 15);
                    dateModel.Cost = firstQuarterTotalCost;
                    chartValues.Add(dateModel);
                }
                if (secondQuarterTotalCost > 0)
                {
                    DateModel dateModel = new DateModel();
                    dateModel.DateTime = new DateTime(yearKey, 04, 15);
                    dateModel.Cost = secondQuarterTotalCost;
                    chartValues.Add(dateModel);
                }
                if (thirdQuarterTotalCost > 0)
                {
                    DateModel dateModel = new DateModel();
                    dateModel.DateTime = new DateTime(yearKey, 07, 15);
                    dateModel.Cost = thirdQuarterTotalCost;
                    chartValues.Add(dateModel);
                }
                if (fourthQuarterTotalCost > 0)
                {
                    DateModel dateModel = new DateModel();
                    dateModel.DateTime = new DateTime(yearKey, 10, 15);
                    dateModel.Cost = fourthQuarterTotalCost;
                    chartValues.Add(dateModel);
                }
            }

            return chartValues;
        }
        public static ChartValues<DateModel> GetTimeLinerMonthsByRangeByParameter(DocumentTimeliner documentTimeliner, string parameterName, string dateParameter, DateTime startDate, DateTime endDate)
        {
            ChartValues<DateModel> chartValues = new ChartValues<DateModel>();

            var dateModels = new List<DateModel>();

            GetDatesByRange(documentTimeliner, parameterName, dateParameter, startDate, endDate, dateModels);

            //Group by year
            var dateModelGroupByYear = dateModels.GroupBy(x => x.DateTime.Year);

            foreach (var groupByYear in dateModelGroupByYear)
            {
                int yearKey = groupByYear.Key;

                List<DateModel> dModels = groupByYear.ToList();

                //Group by Month
                var dateModelGroupByMonth = dModels.GroupBy(x => x.DateTime.Month);

                foreach (var group in dateModelGroupByMonth)
                {
                    int key = group.Key;

                    double totalMonthCost = 0;

                    foreach (var item in group)
                    {
                        totalMonthCost += item.Cost;
                    }

                    DateModel dateModel = new DateModel();
                    dateModel.DateTime = new DateTime(yearKey, key,28);

                    //DateTime dateTimeAfterConversion = DataViewerUIService.DateTimeAfterConversion(dateModel.DateTime);
                    //MessageBox.Show("Data antes: " + dateModel.DateTime + "\n\n" + "Data depois: " + dateTimeAfterConversion);

                    dateModel.Cost = totalMonthCost;

                    chartValues.Add(dateModel);
                }
            }

            return chartValues;
        }
        public static ChartValues<DateModel> GetTimeLinerTwoWeeksByRangeByParameter(DocumentTimeliner documentTimeliner, string parameterName, string dateParameter, DateTime startDate, DateTime endDate)
        {
            ChartValues<DateModel> chartValues = new ChartValues<DateModel>();

            var dateModels = new List<DateModel>();

            GetDatesByRange(documentTimeliner, parameterName, dateParameter, startDate, endDate, dateModels);

            //Group by year
            var dateModelGroupByYear = dateModels.GroupBy(x => x.DateTime.Year);

            foreach (var groupByYear in dateModelGroupByYear)
            {
                int yearKey = groupByYear.Key;
                List<DateModel> dModels = groupByYear.ToList();

                //Group by Month
                var dateModelGroupByMonth = dModels.GroupBy(x => x.DateTime.Month);

                foreach (var group in dateModelGroupByMonth)
                {
                    int monthKey = group.Key;

                    double totalFirstTwoWeeks = 0;
                    double totalSecondTwoWees = 0;
                    DateModel firstDateModel = new DateModel();
                    DateModel secondDateModel = new DateModel();

                    //Group by Two Weeks
                    foreach (var dateModel in group)
                    {
                        DateTime refDate = new DateTime(yearKey, monthKey, 16);
                        DateTime currentDate = dateModel.DateTime;

                        int dateTimeCompare = DateTime.Compare(currentDate, refDate);

                        if (dateTimeCompare <= 0)
                        {
                            totalFirstTwoWeeks += dateModel.Cost;
                        }
                        else
                        {
                            totalSecondTwoWees += dateModel.Cost;
                        }
                    }

                    firstDateModel.Cost = totalFirstTwoWeeks;
                    secondDateModel.Cost = totalSecondTwoWees;

                    firstDateModel.DateTime = new DateTime(yearKey, monthKey, 01);
                    secondDateModel.DateTime = new DateTime(yearKey, monthKey, 16);

                    chartValues.Add(firstDateModel);
                    chartValues.Add(secondDateModel);
                }
            }
            return chartValues;
        }
        public static ChartValues<DateModel> GetTimeLinerDaysByRangeByParameter(DocumentTimeliner documentTimeliner, string parameterName, string dateParameter, DateTime startDate, DateTime endDate)
        {
            ChartValues<DateModel> chartValues = new ChartValues<DateModel>();

            //Get parameter values
            foreach (TimelinerTask task in documentTimeliner.Tasks)
            {
                try
                {
                    DateModel dModel = GetTimeLinerParameterValuesByParameter(task, parameterName, dateParameter);

                    int dateTimeCompareStart = DateTime.Compare(dModel.DateTime, startDate);
                    int dateTimeCompareEnd = DateTime.Compare(dModel.DateTime, endDate);

                    if (dateTimeCompareStart >= 0 && dateTimeCompareEnd <= 0)
                    {
                        chartValues.Add(dModel);
                    }
                }
                catch { }


                foreach (TimelinerTask subTask in task.Children)
                {
                    try
                    {
                        DateModel dModel = GetTimeLinerParameterValuesByParameter(subTask, parameterName, dateParameter);

                        int dateTimeCompareStart = DateTime.Compare(dModel.DateTime, startDate);
                        int dateTimeCompareEnd = DateTime.Compare(dModel.DateTime, endDate);

                        if (dateTimeCompareStart >= 0 && dateTimeCompareEnd <= 0)
                        {
                            chartValues.Add(dModel);
                        }
                    }
                    catch { }
                }
            }

            return chartValues;
        }
        public static void GetDatesByRange(DocumentTimeliner documentTimeliner, string parameterName, string dateParameter, DateTime startDate, DateTime endDate, List<DateModel> dateModels)
        {
            //Get parameter values
            foreach (TimelinerTask task in documentTimeliner.Tasks)
            {
                try
                {
                    DateModel dModel = GetTimeLinerParameterValuesByParameter(task, parameterName, dateParameter);

                    int dateTimeCompareStart = DateTime.Compare(dModel.DateTime, startDate);
                    int dateTimeCompareEnd = DateTime.Compare(dModel.DateTime, endDate);

                    if (dateTimeCompareStart >= 0 && dateTimeCompareEnd <= 0)
                    {
                        dateModels.Add(dModel);
                    }
                }
                catch { }


                foreach (TimelinerTask subTask in task.Children)
                {
                    try
                    {
                        DateModel dModel = GetTimeLinerParameterValuesByParameter(subTask, parameterName, dateParameter);

                        int dateTimeCompareStart = DateTime.Compare(dModel.DateTime, startDate);
                        int dateTimeCompareEnd = DateTime.Compare(dModel.DateTime, endDate);

                        if (dateTimeCompareStart >= 0 && dateTimeCompareEnd <= 0)
                        {
                            dateModels.Add(dModel);
                        }
                    }
                    catch { }
                }
            }
        }
        public static DateModel GetTimeLinerParameterValuesByParameter(TimelinerTask task, string parameterName, string dateParameter)
        {
            DateModel dModel = new DateModel();

            if (dateParameter == NavisApp.Properties.NavisworksParameters.ActualStartParameter)
            {
                dModel.DateTime = (DateTime)task.ActualStartDate;
                dModel.Cost = GetCostByParameterName(parameterName, task);
                dModel.DisplayName = task.DisplayName;
            }
            else if (dateParameter == NavisApp.Properties.NavisworksParameters.ActualEndParameter)
            {
                dModel.DateTime = (DateTime)task.ActualEndDate;
                dModel.Cost = GetCostByParameterName(parameterName, task);
                dModel.DisplayName = task.DisplayName;
            }
            else if (dateParameter == NavisApp.Properties.NavisworksParameters.PlannedStartParameter)
            {
                dModel.DateTime = (DateTime)task.PlannedStartDate;
                dModel.Cost = GetCostByParameterName(parameterName, task);
                dModel.DisplayName = task.DisplayName;
            }
            else if (dateParameter == NavisApp.Properties.NavisworksParameters.PlannedEndParameter)
            {
                dModel.DateTime = (DateTime)task.PlannedEndDate;
                dModel.Cost = GetCostByParameterName(parameterName, task);
                dModel.DisplayName = task.DisplayName;
            }
            return dModel;
        }
        public static double GetCostByParameterName(string parameterName, TimelinerTask task)
        {
            double cost = 0;

            if (parameterName == NavisApp.Properties.NavisworksParameters.TotalCost)
            {
                cost = (double)task.TotalCost;
            }
            else if (parameterName == NavisApp.Properties.NavisworksParameters.MaterialCost)
            {
                cost = (double)task.MaterialCost;
            }
            else if (parameterName == NavisApp.Properties.NavisworksParameters.LaborCost)
            {
                cost = (double)task.LaborCost;
            }
            else if (parameterName == NavisApp.Properties.NavisworksParameters.EquipmentCost)
            {
                cost = (double)task.EquipmentCost;
            }
            else if (parameterName == NavisApp.Properties.NavisworksParameters.SubcontractorCost)
            {
                cost = (double)task.SubcontractorCost;
            }
            else if (parameterName == NavisApp.Properties.NavisworksParameters.Provided_Progress)
            {
                cost = (double)task.ProgressPercent;
            }
            return cost;
        }
        public static DateTime GetTimeLinerDateTime(TimelinerTask task, string parameterName)
        {
            DateTime dateTime = default;

            if (parameterName == NavisApp.Properties.NavisworksParameters.ActualStartParameter)
            {
                dateTime = (DateTime)task.ActualStartDate;
            }
            else if (parameterName == NavisApp.Properties.NavisworksParameters.ActualEndParameter)
            {
                dateTime = (DateTime)task.ActualEndDate;
            }
            else if (parameterName == NavisApp.Properties.NavisworksParameters.PlannedStartParameter)
            {
                dateTime = (DateTime)task.PlannedStartDate;
            }
            else if (parameterName == NavisApp.Properties.NavisworksParameters.PlannedEndParameter)
            {
                dateTime = (DateTime)task.PlannedEndDate;
            }
            return dateTime;
        }
        public static double GetTotalCost()
        {
            double totalCost = 0;

            // Iterate over any existing Timeliner Tasks
            DocumentTimeliner documentTimeliner = Autodesk.Navisworks.Api.Application.MainDocument.GetTimeliner();

            //Get parameter values
            foreach (TimelinerTask task in documentTimeliner.Tasks)
            {
                try
                {
                    totalCost += (double)task.TotalCost;
                }
                catch { }
            }

            return totalCost;
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
            string resultValue = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) +"-"+year.ToString();
            return resultValue;
        }
        public static string TwoWeeksFormatter(DateTime currentDate)
        {
            int year = currentDate.Year;
            int month = currentDate.Month;
            int day = currentDate.Day;
            string resultValue = "invalid";
            if (day < 15)
            {
                resultValue = "1-" + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month) + "-" + year.ToString().Substring(year.ToString().Length - 2);
            }
            else
            {
                resultValue = "2-" + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month) + "-" + year.ToString().Substring(year.ToString().Length - 2);
            }

            return resultValue;
        }
    }
}
