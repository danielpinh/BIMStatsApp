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
    public class DateTimeUtils
    {
        public static double GetDateTimeTicksByTimeSpan(DateTime dateTime, int timeSpam)
        {            
            long dateTimeTicks = dateTime.Ticks;

            long monthTicks = ((long)TimeSpan.FromDays(timeSpam).Ticks) / 10000;

            double dateTimePerMonth = (long)dateTimeTicks / monthTicks;

            var dateTimeFinal = (long)dateTimePerMonth * monthTicks;

            return dateTimeFinal;
        }
        public static DateTime DateTimeAfterConversion(DateTime dateTime)
        {
            long dateTimeTicks = dateTime.Ticks;

            long monthTicks = ((long)TimeSpan.FromDays(30).Ticks) / 10000;

            var dateTimePerMonth = (long)dateTimeTicks / monthTicks;

            var novoValor = (long)dateTimePerMonth * monthTicks;

            DateTime myDate = new DateTime(novoValor);

            return myDate;
        }

      








        public static DateTime GetEarlierDate(string parameterName)
        {
            DateTime earlierDateTime = DateTime.Now;

            try
            {
                // Iterate over any existing Timeliner Tasks
                DocumentTimeliner documentTimeliner = Autodesk.Navisworks.Api.Application.MainDocument.GetTimeliner();
                List<DateTime> dateTimes = NavisUtils.GetTimeLinerDates(documentTimeliner, parameterName);

                foreach (var dt in dateTimes)
                {
                    if (DateTime.Compare(dt, earlierDateTime) < 0)
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

        /// <summary>
        /// Iterate over each day by given DateTime range between <paramref name="from"/> and <paramref name="thru"/> dates.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="thru"></param>
        /// <returns>ForLoop by each day between given DateTime range.</returns>
        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        /// <summary>
        /// Group range of DateTime between <paramref name="from"/> and <paramref name="thru"/> dates by year and by months.        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="thru"></param>
        /// <returns>DateTime list of the 28th day in each month and year group.</returns>
        public static List<DateTime> GroupByYearAndMonth(DateTime from, DateTime thru)
        {
            List<DateTime> dts = new List<DateTime>();
            List<DateTime> resultDts = new List<DateTime>();

            foreach (var dt in EachDay(from, thru))
            {
                dts.Add(dt);
            }

            //Grouping by year
            var yearGroups = dts.GroupBy(x => x.Year);

            foreach (var yearGroup in yearGroups)
            {
                int yearKey = yearGroup.Key;

                List<DateTime> dModels = yearGroup.ToList();

                //Group by Month
                var monthGroups = dModels.GroupBy(x => x.Month);

                foreach (var monthGroup in monthGroups)
                {
                    int key = monthGroup.Key;
                    
                    DateTime newDt = new DateTime(yearKey, key, 28, 0, 0, 0);

                    resultDts.Add(newDt);
                }
            }

            return resultDts;
        }
        /// <summary>
        /// Group range of DateTime between <paramref name="from"/> and <paramref name="thru"/> dates by year.  
        ///  
        /// </summary>
        /// <param name="from"></param>
        /// <param name="thru"></param>
        /// <returns>DateTime list of the 28th day of 6th month in each year group.</returns>
        public static List<DateTime> GroupByYear(DateTime from, DateTime thru)
        {
            List<DateTime> dts = new List<DateTime>();
            List<DateTime> resultDts = new List<DateTime>();

            foreach (var dt in EachDay(from, thru))
            {
                dts.Add(dt);
            }

            //Grouping by year
            var yearGroups = dts.GroupBy(x => x.Year);

            foreach (var yearGroup in yearGroups)
            {
                int yearKey = yearGroup.Key;

                DateTime newDt = new DateTime(yearKey, 06, 28, 1, 0, 0);

                resultDts.Add(newDt);
            }

            return resultDts;
        }
        
        public static List<DateTime> GroupBySemester(DateTime from, DateTime thru)
        {
            List<DateTime> dts = new List<DateTime>();
            List<DateTime> resultDts = new List<DateTime>();

            foreach (var dt in EachDay(from, thru))
            {
                dts.Add(dt);
            }

            //Grouping by year
            var yearGroups = dts.GroupBy(x => x.Year);

            foreach (var yearGroup in yearGroups)
            {
                int yearKey = yearGroup.Key;

                List<DateTime> dModels = yearGroup.ToList();

                //Group by Month
                var monthGroups = dModels.GroupBy(x => x.Month);

                bool firstSemester = false;
                bool secondSemester = false;

                foreach (var group in monthGroups)
                {
                    int key = group.Key;

                    if (key >= 6)
                    {
                        secondSemester = true;
                    }
                    else
                    {
                        firstSemester = true;
                    }
                }

                if (firstSemester)
                {
                    DateTime newDt = new DateTime(yearKey, 03, 28, 0, 0, 0);
                    resultDts.Add(newDt);
                }
                if (secondSemester)
                {
                    DateTime newDt = new DateTime(yearKey, 09, 28, 0, 0, 0);
                    resultDts.Add(newDt);
                }
            }   

            return resultDts;
        }
        public static List<DateTime> GroupByQuarter(DateTime from, DateTime thru)
        {
            List<DateTime> dts = new List<DateTime>();
            List<DateTime> resultDts = new List<DateTime>();

            foreach (var dt in EachDay(from, thru))
            {
                dts.Add(dt);
            }

            //Grouping by year
            var yearGroups = dts.GroupBy(x => x.Year);

            foreach (var yearGroup in yearGroups)
            {
                int yearKey = yearGroup.Key;

                List<DateTime> dModels = yearGroup.ToList();

                //Group by Month
                var monthGroups = dModels.GroupBy(x => x.Month);

                bool firstQuarter = false;
                bool secondQuarter = false;
                bool thirdQuarter = false;
                bool fourthQuarter = false;

                foreach (var group in monthGroups)
                {
                    int monthKey = group.Key;

                    if (monthKey <= 3)
                    {
                        foreach (var item in group)
                        {
                            firstQuarter = true;
                        }
                    }
                    else if (monthKey <= 6)
                    {
                        foreach (var item in group)
                        {
                            secondQuarter = true;
                        }
                    }
                    else if (monthKey <= 9)
                    {
                        foreach (var item in group)
                        {
                            thirdQuarter = true;
                        }
                    }
                    else
                    {
                        foreach (var item in group)
                        {
                            fourthQuarter = true;
                        }
                    }
                }

                if (firstQuarter)
                {
                    DateTime newDt = new DateTime(yearKey, 01, 20);
                    resultDts.Add(newDt);
                }
                if (secondQuarter)
                {
                    DateTime newDt = new DateTime(yearKey, 04, 20);
                    resultDts.Add(newDt);
                }
                if (thirdQuarter)
                {
                    DateTime newDt = new DateTime(yearKey, 07, 20);
                    resultDts.Add(newDt);
                }
                if (fourthQuarter)
                {
                    DateTime newDt = new DateTime(yearKey, 10, 20);
                    resultDts.Add(newDt);
                }
            }

            return resultDts;
        }
        public static List<DateTime> GroupByTwoWeeks(DateTime from, DateTime thru)
        {
            List<DateTime> dts = new List<DateTime>();
            List<DateTime> resultDts = new List<DateTime>();

            foreach (var dt in EachDay(from, thru))
            {
                dts.Add(dt);
            }

            //Grouping by year
            var yearGroups = dts.GroupBy(x => x.Year);

            foreach (var yearGroup in yearGroups)
            {
                int yearKey = yearGroup.Key;

                List<DateTime> dModels = yearGroup.ToList();

                //Group by Month
                var monthGroups = dModels.GroupBy(x => x.Month);

                bool firsTwoWeeks = false;
                bool secondTwoWeeks = false;

                foreach (var monthGroup in monthGroups)
                {
                    int monthKey = monthGroup.Key;

                    foreach (var dt in monthGroup)
                    {
                        DateTime refDate = new DateTime(yearKey, monthKey, 15, 0, 0, 0);

                        int dateTimeCompare = DateTime.Compare(dt, refDate);

                        if (dateTimeCompare <= 0)
                        {
                            firsTwoWeeks = true;
                        }
                        else
                        {
                            secondTwoWeeks = true;
                        }
                    }

                    if (firsTwoWeeks)
                    {
                        DateTime newDt = new DateTime(yearKey, monthKey, 5, 0, 0, 0);
                        resultDts.Add(newDt);
                    }
                    if (secondTwoWeeks)
                    {
                        DateTime newDt = new DateTime(yearKey, monthKey, 20, 0, 0, 0);
                        resultDts.Add(newDt);
                    }
                }
            }

            return resultDts;
        }


    }
}
