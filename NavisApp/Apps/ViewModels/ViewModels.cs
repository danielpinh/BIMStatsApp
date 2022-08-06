using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace NavisApp.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
    public class IsCheckedViewModel : ViewModelBase
    {
        public bool IsChecked { get; set; }
    }
    public class DateViewModel : ViewModelBase
    {
        public DateTime Date { get; set; }
    }
    public class GradationXAxisViewModel : ViewModelBase
    {
        public string Gradation { get; set; }
    }
    public class CostViewModel : ViewModelBase
    {
        public double PartialCost { get; set; }
        public double TotalCost { get; set; }
    }
    public class ParameterViewModel : ViewModelBase
    {
        public string Name { get; set; }
        public string ParameterName { get; set; }
        public string DateParameterName { get; set; }
        public Brush FillColor { get; set; }
        public Brush PointColor { get; set; }
        public string GraphType { get; set; }
    }
    public class CurrentParameterViewModel : ViewModelBase
    {
        public string ParameterName { get; set; }
    }
    public class EditParameterViewModel : ViewModelBase
    {
        public string GraphType { get; set; }
        public string GraphTypeLabel { get; set; }
    }

}
