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
using static KanColleNote.Core.Prophet.BattleHPManager;

namespace KanColleNote.UI
{

    [ValueConversion(typeof(int), typeof(Brush))]
    public class DamageColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
            {
                return new SolidColorBrush(Colors.Green);
            }
            Int32.TryParse(values[0].ToString(), out int nowHP);
            Int32.TryParse(values[1].ToString(), out int maxHP);
            var state = BattleHPManager.GetDamageState(nowHP, maxHP);
            switch (state)
            {
                case DamageState.DIE:
                    return new SolidColorBrush(Colors.White); //"击沉"; //或脱离？
                case DamageState.MAXDAMAGE:
                    return new SolidColorBrush(Colors.Red); //"大破";
                case DamageState.MIDDAMAGE:
                    return new SolidColorBrush(Colors.Orange); //"中破";
                case DamageState.MINDAMAGE:
                    return new SolidColorBrush(Colors.Yellow); //"小破";
                case DamageState.MILDLY:
                    return new SolidColorBrush(Colors.Green); //擦伤
                case DamageState.FULL:
                    return new SolidColorBrush(Colors.Green); //无伤
            }
            return new SolidColorBrush(Colors.Green); //无伤
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        public int Cutoff { get; set; }
    }

    [ValueConversion(typeof(int), typeof(Brush))]
    public class DamageFillConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
            {
                return new SolidColorBrush(Colors.Green);
            }
            Int32.TryParse(values[0].ToString(), out int nowHP);
            Int32.TryParse(values[1].ToString(), out int maxHP);
            var state = BattleHPManager.GetDamageState(nowHP, maxHP);
            if (state >= BattleHPManager.DamageState.MINDAMAGE)
            {
                return new SolidColorBrush(Color.FromArgb(0xBF, 0, 0, 0));
            }
            else
            {
                return new SolidColorBrush(Colors.White); //无伤
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        public int Cutoff { get; set; }
    }

    [ValueConversion(typeof(int), typeof(Brush))]
    public class DamageStrokeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
            {
                return new SolidColorBrush(Colors.Green);
            }
            Int32.TryParse(values[0].ToString(), out int nowHP);
            Int32.TryParse(values[1].ToString(), out int maxHP);
            var state = BattleHPManager.GetDamageState(nowHP, maxHP);
            if (state >= BattleHPManager.DamageState.MINDAMAGE)
            {
                return new SolidColorBrush(Colors.White);
            }
            else
            {
                return new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
            }
        }

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
    }



}
