using KanColleNote.Core.Prophet;
using Newtonsoft.Json.Linq;
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

    [ValueConversion(typeof(int), typeof(Brush))]
    public class CutoffConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    int valueInt;
        //    Int32.TryParse(value.ToString(), out valueInt);
        //    int parameterInt;
        //    Int32.TryParse(value.ToString(), out parameterInt);
        //    return (valueInt > parameterInt);
        //}


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
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

        private void PriceProgressBar_MouseEnter(object sender, MouseEventArgs e)
        {
            //展示其数据
            var a = this.BindingGroup.ToString();
            DataGridRow row = (DataGridRow)BindingGroup.Owner;
            if (row.Item != null)
            {
                if (row.Item.GetType() == typeof(BattleUnit))
                {
                    //JObject root = row.Item

                }
            }
        }

        private void PriceProgressBar_MouseLeave(object sender, MouseEventArgs e)
        {

        }
    }



}
