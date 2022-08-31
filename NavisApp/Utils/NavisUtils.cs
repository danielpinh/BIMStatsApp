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

namespace NavisApp.Utils
{
    public class NavisUtils
    {
        public static DockPanePlugin GetTimeLinerDockPane(bool MakeVisible)
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

        public static void GoToSimulationMode(DateTime Day)
        {
            DockPanePlugin TimeLinerDockPane = NavisUtils.GetTimeLinerDockPane(true);
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
        public static DateTime GetSimulationCurrentDate()
        {
            DateTime currentDateTime = new DateTime();

            DockPanePlugin TimeLinerDockPane = NavisUtils.GetTimeLinerDockPane(true);
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
        public static double GetCostByDateTimeRange(DocumentTimeliner documentTimeliner, string parameterName, string dateParameter, DateTime startDate, DateTime endDate)
        {
            double cost = 0;

            foreach (TimelinerTask task in documentTimeliner.Tasks)
            {
                DateModel dModel = GetTimeLinerParameterValuesByParameter(task, parameterName, dateParameter);

                int dateTimeCompareStart = DateTime.Compare(dModel.DateTime, startDate);
                int dateTimeCompareEnd = DateTime.Compare(dModel.DateTime, endDate);

                if (dateTimeCompareStart >= 0 && dateTimeCompareEnd <= 0)
                {
                    cost += dModel.Cost;
                }         
            }
            return cost;
        }
        public static ChartValues<DateModel> GetTimeLinerParametersByYear(DocumentTimeliner documentTimeliner,
            string parameterName, 
            string dateParameter,
            DateTime startDate,
            DateTime endDate)
        {
            BIMStatsAppMVVM.ChartValues = new ChartValues<DateModel>();

            var dateModels = new List<DateModel>();

            GetDatesByRange(documentTimeliner, parameterName, dateParameter, startDate, endDate, dateModels);

            int counter = 0;

            //Group by year
            var dateModelGroupByYear = dateModels.GroupBy(x => x.DateTime.Year);

            foreach (var groupByYear in dateModelGroupByYear)
            {
                int yearKey = groupByYear.Key;

                List<DateModel> dModels = groupByYear.ToList();

                double totalYearCost = 0;

                List<ModelItem> modelItems = new List<ModelItem>();

                foreach (var item in groupByYear)
                {
                    totalYearCost += item.Cost;
                    modelItems.AddRange(item.ModelItems);
                }

                DateModel dateModel = new DateModel();
                dateModel.DateTime = new DateTime(yearKey, 06, 28,1,0,0);
                dateModel.Cost = totalYearCost;
                dateModel.ModelItems = modelItems;
                dateModel.Sequence = counter++;

                BIMStatsAppMVVM.ChartValues.Add(dateModel);
            }

            return BIMStatsAppMVVM.ChartValues;
        }
        public static ChartValues<DateModel> GetTimeLinerParametersBySemester(DocumentTimeliner documentTimeliner,
            string parameterName, 
            string dateParameter, 
            DateTime startDate, 
            DateTime endDate)
        {
            BIMStatsAppMVVM.ChartValues = new ChartValues<DateModel>();

            var dateModels = new List<DateModel>();

            GetDatesByRange(documentTimeliner, parameterName, dateParameter, startDate, endDate, dateModels);

            int counter = 0;

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

                List<ModelItem> firstSemesterModelItems = new List<ModelItem>();
                List<ModelItem> secondSemesterModelItems = new List<ModelItem>();

                foreach (var group in dateModelGroupByMonth)
                {
                    int key = group.Key;

                    if (key > 6)
                    {
                        foreach (var item in group)
                        {
                            secondSemesterTotalCost += item.Cost;
                            secondSemesterModelItems.AddRange(item.ModelItems);
                        }
                    }
                    else
                    {                       
                        foreach (var item in group)
                        {
                            firstSemesterTotalCost += item.Cost;
                            firstSemesterModelItems.AddRange(item.ModelItems);
                        }
                    }
                }

                if (firstSemesterTotalCost > 0)
                {
                    DateModel dateModel = new DateModel();
                    dateModel.DateTime = new DateTime(yearKey, 03, 28,0,0,0);
                    dateModel.Cost = firstSemesterTotalCost;
                    dateModel.ModelItems = firstSemesterModelItems;
                    dateModel.Sequence = counter++;
                    BIMStatsAppMVVM.ChartValues.Add(dateModel);
                }
                if (secondSemesterTotalCost > 0)
                {
                    DateModel dateModel = new DateModel();
                    dateModel.DateTime = new DateTime(yearKey, 09, 28,0,0,0);
                    dateModel.Cost = secondSemesterTotalCost;
                    dateModel.ModelItems = secondSemesterModelItems;
                    dateModel.Sequence = counter++;
                    BIMStatsAppMVVM.ChartValues.Add(dateModel);
                }
            }

            return BIMStatsAppMVVM.ChartValues;
        }
        public static ChartValues<DateModel> GetTimeLinerParametersByQuarter(DocumentTimeliner documentTimeliner,
            string parameterName,
            string dateParameter,
            DateTime startDate,
            DateTime endDate)
        {
            BIMStatsAppMVVM.ChartValues = new ChartValues<DateModel>();

            var dateModels = new List<DateModel>();

            GetDatesByRange(documentTimeliner, parameterName, dateParameter, startDate, endDate, dateModels);

            int counter = 0;

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

                List<ModelItem> firstQuarterModelItems = new List<ModelItem>();
                List<ModelItem> secondQuarterModelItems = new List<ModelItem>();
                List<ModelItem> thirdQuarterModelItems = new List<ModelItem>();
                List<ModelItem> fourthQuarterModelItems = new List<ModelItem>();

                //Group by quarter
                foreach (var group in dateModelGroupByMonth)
                {
                    int monthKey = group.Key;

                    if (monthKey <= 3)
                    {
                        foreach (var item in group)
                        {
                            firstQuarterTotalCost += item.Cost;
                            firstQuarterModelItems.AddRange(item.ModelItems);
                        }
                    }
                    else if (monthKey <= 6)
                    {
                        foreach (var item in group)
                        {
                            secondQuarterTotalCost += item.Cost;
                            secondQuarterModelItems.AddRange(item.ModelItems);
                        }
                    }
                    else if (monthKey <= 9)
                    {
                        foreach (var item in group)
                        {
                            thirdQuarterTotalCost += item.Cost;
                            thirdQuarterModelItems.AddRange(item.ModelItems);
                        }
                    }
                    else
                    {
                        foreach (var item in group)
                        {
                            fourthQuarterTotalCost += item.Cost;
                            fourthQuarterModelItems.AddRange(item.ModelItems);
                        }
                    }
                }

                //Coletando o mês mais avançado para evitar valores nulos no gráfico! 
                int higherMonth = 0;
                if (endDate.Year > yearKey)
                {
                    higherMonth = 12;
                }
                else 
                {
                    higherMonth = endDate.Month;
                }

                if (higherMonth >= 1 || higherMonth >= 2 || higherMonth >= 3)
                {
                    DateModel dateModel = new DateModel();
                    dateModel.DateTime = new DateTime(yearKey, 01, 20);
                    dateModel.Cost = firstQuarterTotalCost;
                    dateModel.Sequence = counter++;
                    dateModel.ModelItems = firstQuarterModelItems;
                    BIMStatsAppMVVM.ChartValues.Add(dateModel);
                }
                if (higherMonth >= 4 || higherMonth >= 5 || higherMonth >= 6)
                {
                    DateModel dateModel = new DateModel();
                    dateModel.DateTime = new DateTime(yearKey, 04, 20);
                    dateModel.Cost = secondQuarterTotalCost;
                    dateModel.Sequence = counter++;
                    dateModel.ModelItems = secondQuarterModelItems;
                    BIMStatsAppMVVM.ChartValues.Add(dateModel);
                }
                if (higherMonth >= 7 || higherMonth >= 8 || higherMonth >= 9)
                {
                    DateModel dateModel = new DateModel();
                    dateModel.DateTime = new DateTime(yearKey, 07, 20);
                    dateModel.Cost = thirdQuarterTotalCost;
                    dateModel.Sequence = counter++;
                    dateModel.ModelItems = thirdQuarterModelItems;
                    BIMStatsAppMVVM.ChartValues.Add(dateModel);
                }
                if (higherMonth >= 10 || higherMonth >= 11 || higherMonth >= 12)
                {
                    DateModel dateModel = new DateModel();
                    dateModel.DateTime = new DateTime(yearKey, 10, 20);
                    dateModel.Cost = fourthQuarterTotalCost;
                    dateModel.Sequence = counter++;
                    dateModel.ModelItems = fourthQuarterModelItems;
                    BIMStatsAppMVVM.ChartValues.Add(dateModel);
                }
            }

            return BIMStatsAppMVVM.ChartValues;
        }
        public static ChartValues<DateModel> GetAccumulatedTimeLinerParametersByMonth(DocumentTimeliner documentTimeliner,
            string parameterName,
            string dateParameter,
            DateTime startDate,
            DateTime endDate)
        {
            BIMStatsAppMVVM.ChartValues = new ChartValues<DateModel>();

            var dateModels = new List<DateModel>();

            GetDatesByRange(documentTimeliner, parameterName, dateParameter, startDate, endDate, dateModels);

            int counter = 0;

            //Group by year
            var dateModelGroupByYear = dateModels.GroupBy(x => x.DateTime.Year);

            double totalMonthCost = 0;

            foreach (var groupByYear in dateModelGroupByYear)
            {
                int yearKey = groupByYear.Key;

                List<DateModel> dModels = groupByYear.ToList();

                //Group by Month
                var dateModelGroupByMonth = dModels.GroupBy(x => x.DateTime.Month);

                foreach (var group in dateModelGroupByMonth)
                {
                    List<ModelItem> modelItems = new List<ModelItem>();

                    int key = group.Key;

                    foreach (var item in group)
                    {
                        totalMonthCost += item.Cost;
                        modelItems.AddRange(item.ModelItems);
                    }

                    DateModel dateModel = new DateModel();
                    dateModel.DateTime = new DateTime(yearKey, key, 28, 0, 0, 0);
                    dateModel.Sequence = counter++;
                    dateModel.Cost = totalMonthCost;
                    dateModel.ModelItems = modelItems;

                    BIMStatsAppMVVM.ChartValues.Add(dateModel);
                }
            }

            return BIMStatsAppMVVM.ChartValues;
        }
        public static ChartValues<DateModel> GetTimeLinerParametersByMonth(DocumentTimeliner documentTimeliner,
            string parameterName, 
            string dateParameter,
            DateTime startDate,
            DateTime endDate)
        {
            BIMStatsAppMVVM.ChartValues = new ChartValues<DateModel>();

            var dateModels = new List<DateModel>();

            GetDatesByRange(documentTimeliner, parameterName, dateParameter, startDate, endDate, dateModels);

            int counter = 0;

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
                    List<ModelItem> modelItems = new List<ModelItem>();

                    int key = group.Key;

                    double totalMonthCost = 0;

                    foreach (var item in group)
                    {
                        totalMonthCost += item.Cost;
                        modelItems.AddRange(item.ModelItems);
                    }

                    DateModel dateModel = new DateModel();
                    dateModel.DateTime = new DateTime(yearKey, key, 28,0,0,0);
                    dateModel.Sequence = counter++;
                    dateModel.Cost = totalMonthCost;
                    dateModel.ModelItems = modelItems;

                    BIMStatsAppMVVM.ChartValues.Add(dateModel);
                }
            }

            return BIMStatsAppMVVM.ChartValues;
        }
        public static ChartValues<DateModel> GetTimeLinerParametersByTwoWeeks(DocumentTimeliner documentTimeliner,
            string parameterName, 
            string dateParameter,
            DateTime startDate,
            DateTime endDate)
        {
            BIMStatsAppMVVM.ChartValues = new ChartValues<DateModel>();

            var dateModels = new List<DateModel>();

            GetDatesByRange(documentTimeliner, parameterName, dateParameter, startDate, endDate, dateModels);

            int counter = 0;

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

                    List<ModelItem> firstModelItems = new List<ModelItem>();
                    List<ModelItem> secondModelItems = new List<ModelItem>();

                    //Group by Two Weeks
                    foreach (var dateModel in group)
                    {
                        DateTime refDate = new DateTime(yearKey, monthKey, 15,0,0,0);
                        DateTime currentDate = dateModel.DateTime;

                        int dateTimeCompare = DateTime.Compare(currentDate, refDate);

                        if (dateTimeCompare <= 0)
                        {
                            totalFirstTwoWeeks += dateModel.Cost;
                            firstModelItems.AddRange(dateModel.ModelItems);
                        }
                        else
                        {
                            totalSecondTwoWees += dateModel.Cost;
                            secondModelItems.AddRange(dateModel.ModelItems);
                        }
                    }

                    firstDateModel.Cost = totalFirstTwoWeeks;
                    secondDateModel.Cost = totalSecondTwoWees;

                    firstDateModel.Sequence = counter++;
                    secondDateModel.Sequence = counter++;

                    firstDateModel.ModelItems = firstModelItems;
                    secondDateModel.ModelItems = secondModelItems;

                    firstDateModel.DateTime = new DateTime(yearKey, monthKey, 5,0,0,0);
                    secondDateModel.DateTime = new DateTime(yearKey, monthKey, 20,0,0,0);

                    BIMStatsAppMVVM.ChartValues.Add(firstDateModel);
                    BIMStatsAppMVVM.ChartValues.Add(secondDateModel);                    
                }
            }
            return BIMStatsAppMVVM.ChartValues;
        }
        public static ChartValues<DateModel> GetTimeLinerParametersByWeek(DocumentTimeliner documentTimeliner,
            string parameterName,
            string dateParameter,
            DateTime startDate,
            DateTime endDate)
        {
            ChartValues<DateModel> chartValues = new ChartValues<DateModel>();

            //var dateModels = new List<DateModel>();

            //GetDatesByRange(documentTimeliner, parameterName, dateParameter, startDate, endDate, dateModels);

            ////Group by year
            //var dateModelGroupByYear = dateModels.GroupBy(x => x.DateTime.Year);

            //foreach (var groupByYear in dateModelGroupByYear)
            //{
            //    int yearKey = groupByYear.Key;
            //    List<DateModel> dModels = groupByYear.ToList();

            //    //Group by Month
            //    var dateModelGroupByMonth = dModels.GroupBy(x => x.DateTime.Month);

            //    foreach (var group in dateModelGroupByMonth)
            //    {
            //        int monthKey = group.Key;

            //        double totalFirstTwoWeeks = 0;
            //        double totalSecondTwoWees = 0;
            //        DateModel firstDateModel = new DateModel();
            //        DateModel secondDateModel = new DateModel();

            //        List<ModelItem> firstModelItems = new List<ModelItem>();
            //        List<ModelItem> secondModelItems = new List<ModelItem>();

            //        //Group by Week
            //        foreach (var dateModel in group)
            //        {
            //            DateTime currentDate = dateModel.DateTime;

            //            int dateTimeCompare = DateTime.Compare(currentDate, refDate);

            //            if (dateTimeCompare <= 0)
            //            {
            //                totalFirstTwoWeeks += dateModel.Cost;
            //                firstModelItems.AddRange(dateModel.ModelItems);
            //            }
            //            else
            //            {
            //                totalSecondTwoWees += dateModel.Cost;
            //                secondModelItems.AddRange(dateModel.ModelItems);
            //            }
            //        }

            //        firstDateModel.Cost = totalFirstTwoWeeks;
            //        secondDateModel.Cost = totalSecondTwoWees;

            //        firstDateModel.ModelItems = firstModelItems;
            //        secondDateModel.ModelItems = secondModelItems;

            //        firstDateModel.DateTime = new DateTime(yearKey, monthKey, 01);
            //        secondDateModel.DateTime = new DateTime(yearKey, monthKey, 16);

            //        chartValues.Add(firstDateModel);
            //        chartValues.Add(secondDateModel);
            //    }
            //}
            return chartValues;
        }
        public static ChartValues<DateModel> GetTimeLinerParametersByDay(DocumentTimeliner documentTimeliner,
            string parameterName,
            string dateParameter,
            DateTime startDate,
            DateTime endDate)
        {
            BIMStatsAppMVVM.ChartValues = new ChartValues<DateModel>();

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
                        BIMStatsAppMVVM.ChartValues.Add(dModel);
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
                            BIMStatsAppMVVM.ChartValues.Add(dModel);
                        }
                    }
                    catch { }
                }
            }

            return BIMStatsAppMVVM.ChartValues;
        }
        public static DateModel GetTimeLinerParameterValuesByParameter(TimelinerTask task,
            string parameterName,
            string dateParameter)
        {
            DateModel dModel = new DateModel();

            if (dateParameter == NavisApp.Properties.NavisworksParameters.ActualStartParameter)
            {
                dModel.DateTime = (DateTime)task.ActualStartDate;
            }
            else if (dateParameter == NavisApp.Properties.NavisworksParameters.ActualEndParameter)
            {
                dModel.DateTime = (DateTime)task.ActualEndDate;
            }
            else if (dateParameter == NavisApp.Properties.NavisworksParameters.PlannedStartParameter)
            {
                dModel.DateTime = (DateTime)task.PlannedStartDate;
            }
            else if (dateParameter == NavisApp.Properties.NavisworksParameters.PlannedEndParameter)
            {
                dModel.DateTime = (DateTime)task.PlannedEndDate;
            }

            dModel.Cost = GetCostByParameterName(parameterName, task);
            dModel.DisplayName = task.DisplayName;
            dModel.ModelItems = GetExplicitSelection(task);

            return dModel;
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
        public static void GetDatesByRange(DocumentTimeliner documentTimeliner,
            string parameterName, 
            string dateParameter,
            DateTime startDate, 
            DateTime endDate,
            List<DateModel> dateModels)
        {
            //Get parameter values
            foreach (TimelinerTask task in documentTimeliner.Tasks)
            {
                try
                {
                    DateModel taskDateModel = new DateModel();  
                    taskDateModel = GetTimeLinerParameterValuesByParameter(task, parameterName, dateParameter);

                    int dateTimeCompareStart = DateTime.Compare(taskDateModel.DateTime, startDate);
                    int dateTimeCompareEnd = DateTime.Compare(taskDateModel.DateTime, endDate);

                    if (dateTimeCompareStart >= 0 && dateTimeCompareEnd <= 0)
                    {
                        dateModels.Add(taskDateModel);
                    }
                }
                catch { }

                foreach (TimelinerTask subTask in task.Children)
                {
                    try
                    {
                        DateModel subtaskDateModel = new DateModel();

                        subtaskDateModel = GetTimeLinerParameterValuesByParameter(subTask, parameterName, dateParameter);

                        int dateTimeCompareStart = DateTime.Compare(subtaskDateModel.DateTime, startDate);
                        int dateTimeCompareEnd = DateTime.Compare(subtaskDateModel.DateTime, endDate);

                        if (dateTimeCompareStart >= 0 && dateTimeCompareEnd <= 0)
                        {
                            dateModels.Add(subtaskDateModel);
                        }
                    }
                    catch { }
                }
            }
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
        /// <summary>
        /// Return explicit collection of ModelItems by given <paramref name="timelinerTask"/>.
        /// </summary>
        /// <param name="timelinerTask"></param>
        /// <returns></returns>
        public static List<ModelItem> GetExplicitSelection(TimelinerTask timelinerTask)
        {
            List<ModelItem> modelItems = new List<ModelItem>();

            try
            {
                TimelinerSelection tls = timelinerTask.Selection;

                foreach (var item in tls.ExplicitSelection)
                {
                    modelItems.Add(item);
                }
            }
            catch { }

            return modelItems;
        }
        /// <summary>
        /// Select <paramref name="itemsToSelect"/> and hide unselected model items in the current document.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="selectedItems"></param>
        public static void HideUnselectedItems(Document doc, List<ModelItem> itemsToSelect)
        {
            //Reset all hidden items in the model
            Autodesk.Navisworks.Api.Application.ActiveDocument.Models.ResetAllHidden();

            //Set current selection in active document
            doc.CurrentSelection.CopyFrom(itemsToSelect);

            //Create hidden collection
            ModelItemCollection hidden = new ModelItemCollection();

            //create a store for the visible items
            ModelItemCollection visible = new ModelItemCollection();

            //Add all the items that are visible to the visible collection
            foreach (ModelItem item in Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems)
            {
                if (item.AncestorsAndSelf != null)

                    visible.AddRange(item.AncestorsAndSelf);

                if (item.Descendants != null)

                    visible.AddRange(item.Descendants);
            }

            //mark as invisible all the siblings of the visible items
            foreach (ModelItem toShow in visible)
            {
                if (toShow.Parent != null)
                {
                    hidden.AddRange(toShow.Parent.Children);
                }
            }

            //remove the visible items from the collection
            foreach (ModelItem toShow in visible)
            {
                hidden.Remove(toShow);
            }

            //hide the remaining items
            Autodesk.Navisworks.Api.Application.ActiveDocument.Models.SetHidden(hidden, true);
        }

        public static ChartValues<DateModel> FixPlannedExecutedChartDateTimeGapsByMonth(ChartValues<DateModel> chartValues, DateTime startDateTime, DateTime endDateTime)
        {
            List<DateTime> dts = DateTimeUtils.GroupByYearAndMonth(startDateTime, endDateTime);

            foreach (var dt in dts)
            {
                bool monthContains = false;

                foreach (var dm in chartValues)
                {
                    if (dt.Month == dm.DateTime.Month &&
                        dt.Year == dm.DateTime.Year)
                    {
                        monthContains = true;
                        break;
                    }
                }

                if (!monthContains)
                {
                    DateModel dateModel = new DateModel();
                    dateModel.DateTime = new DateTime(dt.Year, dt.Month, 28);
                    dateModel.Cost = 0;
                    chartValues.Add(dateModel);
                }
            }

            return chartValues;
        }
        public static ChartValues<DateModel> FixSCurveChartDateTimeGapsByMonth(ChartValues<DateModel> chartValues, DateTime startDateTime, DateTime endDateTime)
        {
            List<DateTime> dts = DateTimeUtils.GroupByYearAndMonth(startDateTime, endDateTime);

            foreach (var dt in dts)
            {
                bool monthContains = false;

                foreach (var dm in chartValues)
                {
                    if (dt.Month == dm.DateTime.Month &&
                        dt.Year == dm.DateTime.Year)
                    {
                        monthContains = true;
                        break;
                    }
                }

                if (!monthContains)
                {
                    DateModel dateModel = new DateModel();
                    dateModel.DateTime = new DateTime(dt.Year, dt.Month, 28);
                    dateModel.Cost = chartValues.Last().Cost;
                    chartValues.Add(dateModel);
                }
            }

            return chartValues;
        }

        public static ChartValues<DateModel> FixDateTimeGapsByYear(ChartValues<DateModel> chartValues, DateTime startDateTime, DateTime endDateTime)
        {
            List<DateTime> dts = DateTimeUtils.GroupByYear(startDateTime, endDateTime);

            foreach (var dt in dts)
            {
                bool yearContains = false;

                foreach (var dm in chartValues)
                {
                    if (dt.Year == dm.DateTime.Year)
                    {
                        yearContains = true;
                        break;
                    }
                }

                if (!yearContains)
                {
                    DateModel dateModel = new DateModel();
                    dateModel.DateTime = new DateTime(dt.Year, 06, 28);
                    dateModel.Cost = 0;
                    chartValues.Add(dateModel);
                }
            }

            return chartValues;
        }
        public static ChartValues<DateModel> FixDateTimeGapsBySemester(ChartValues<DateModel> chartValues, DateTime startDateTime, DateTime endDateTime)
        {
            List<DateTime> dts = DateTimeUtils.GroupBySemester(startDateTime, endDateTime);

            foreach (var dt in dts)
            {
                bool semesterContains = false;

                foreach (var dm in chartValues)
                {
                    if (dt.Month == dm.DateTime.Month &&
                        dt.Year == dm.DateTime.Year)
                    {
                        semesterContains = true;
                        break;
                    }
                }

                if (!semesterContains)
                {
                    DateModel dateModel = new DateModel();
                    dateModel.DateTime = new DateTime(dt.Year, dt.Month, 28);
                    dateModel.Cost = 0;
                    chartValues.Add(dateModel);
                }
            }

            return chartValues;
        }
        public static ChartValues<DateModel> FixDateTimeGapsByQuarter(ChartValues<DateModel> chartValues, DateTime startDateTime, DateTime endDateTime)
        {
            List<DateTime> dts = DateTimeUtils.GroupByQuarter(startDateTime, endDateTime);

            foreach (var dt in dts)
            {
                bool quarterContains = false;

                foreach (var dm in chartValues)
                {
                    if (dt.Month == dm.DateTime.Month &&
                        dt.Year == dm.DateTime.Year)
                    {
                        quarterContains = true;
                        break;
                    }
                }

                if (!quarterContains)
                {
                    DateModel dateModel = new DateModel();
                    dateModel.DateTime = new DateTime(dt.Year, dt.Month, 28);
                    dateModel.Cost = 0;
                    chartValues.Add(dateModel);
                }
            }

            return chartValues;
        }
        public static ChartValues<DateModel> FixDateTimeGapsByTwoWeeks(ChartValues<DateModel> chartValues, DateTime startDateTime, DateTime endDateTime)
        {
            List<DateTime> dts = DateTimeUtils.GroupByTwoWeeks(startDateTime, endDateTime);

            foreach (var dt in dts)
            {
                bool twoWeeksContains = false;

                foreach (var dm in chartValues)
                {
                    if (dt.Month == dm.DateTime.Month &&
                        dt.Year == dm.DateTime.Year &&
                        dt.Day == dm.DateTime.Day)
                    {
                        twoWeeksContains = true;
                        break;
                    }
                }

                if (!twoWeeksContains)
                {
                    DateModel dateModel = new DateModel();
                    dateModel.DateTime = new DateTime(dt.Year, dt.Month, dt.Day);
                    dateModel.Cost = 0;
                    chartValues.Add(dateModel);
                }
            }

            return chartValues;
        }
    }
}
