using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavisApp.Utils
{
    public class ChartUtils
    {
        public static ChartValues<DateModel> SortChartValuesByDateTime(ChartValues<DateModel> chartValues)
        {
            ChartValues<DateModel> cvs = new ChartValues<DateModel>();

            var sortedChartValues = chartValues.OrderBy(x => x.DateTime.Year)
                .ThenBy(x=>x.DateTime.Month)
                .ThenBy(x=>x.DateTime.Day);

            int counter = 0;
            foreach (var dm in sortedChartValues)
            {
                dm.Sequence = counter++;
                cvs.Add(dm);
                counter++;
            }
            return cvs;
        }     
    }
}
