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
    }
}
