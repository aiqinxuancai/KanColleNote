using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace KanColleNote.UI
{

    [ValueConversion(typeof(int), typeof(bool))]
    public class CutoffConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int valueInt;
            Int32.TryParse(value.ToString(), out valueInt);
            int parameterInt;
            Int32.TryParse(value.ToString(), out parameterInt);
            return (valueInt > parameterInt);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public int Cutoff { get; set; }
    }


    /// <summary>
    /// ProphetCell.xaml 的交互逻辑
    /// </summary>
    public partial class ProphetCell : UserControl
    {
        public ProphetCell()
        {
            InitializeComponent();




        }

    }



}
