using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using GTransfer.Library;

namespace GTransfer.Converters
{

    [ValueConversion(typeof(DateTime), typeof(string))]    
    public class DateToMitiConverter : FrameworkElement, IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {

                if (((DateTime)value) < new DateTime(1944, 4, 13))
                    return "";
                using (SqlConnection conn = new SqlConnection(GlobalClass.DataConnectionString))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = "SELECT DBO.DateToMiti('" + ((DateTime)value).ToString("MM/dd/yyyy") + "','/')";
                    return cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                return "Date Conversion Error " + ex.Message  ;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "";
        }
    }

    [ValueConversion(typeof(object), typeof(string))]
    internal class longConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == DBNull.Value)
                return string.Empty;
            else if (value.GetType() == typeof(decimal) || value.GetType() == typeof(double))
            {
                if(GParse.ToDecimal(value) == 0)
                    return string.Empty;
                else 
                    return GParse.ToDecimal(value).ToString("#0.00");
            }
            else if (value.GetType() == typeof(long) && (long)value == 0)
                return string.Empty;
            else
                return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            return GParse.ToLong(value);
        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public  class ReverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }

    }

    [ValueConversion(typeof(string), typeof(Visibility))]
    internal class ReqFieldVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.IsNullOrEmpty(value.ToString()) ? Visibility.Visible : Visibility.Hidden;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //return Visibility.Visible;
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class HasGroupToVisiblityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if((bool)values[0]==true  && (bool)values[1]==true )
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class reverseHasGroupToVisiblityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)values[0] == true && (bool)values[1] == true)
            {
                return Visibility.Collapsed ;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(int),typeof(Visibility))]
    public class IntToVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //return Visibility.Visible;
            return GParse.ToInteger(value) == 0 ? Visibility.Collapsed : Visibility.Visible;
            //return (bool)value ? Visibility.Visible : Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }
    }
    public class ByteToVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //return Visibility.Visible;
            return GParse.ToByte(value) == 0 ? Visibility.Collapsed : Visibility.Visible;
            //return (bool)value ? Visibility.Visible : Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }
    }
    [ValueConversion(typeof(Visibility ), typeof(bool ))]
    public class VisiblityToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //return Visibility.Visible;
            return (Visibility)value == Visibility.Collapsed  ? false  : true ;
            //return (bool)value ? Visibility.Visible : Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool )value == false ;
        }
    }

    [ValueConversion(typeof(string ), typeof(Visibility))]
    public class ProductTypeToVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //return Visibility.Visible;
            return (string)value =="A" ? Visibility.Visible : Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible ? "A":"G";
        }
    }


    [ValueConversion(typeof(int ), typeof(bool))]
    public class IntToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //return Visibility.Visible;
            return GParse.ToInteger( value) == 0 ? false : true ;
            //return (bool)value ? Visibility.Visible : Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value == false;
        }
    }
    [ValueConversion(typeof(TimeSpan), typeof(DateTime?))]
    public class TimeSpanToDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //return Visibility.Visible;
            return new DateTime(1900, 1, 1) + (TimeSpan)value;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((DateTime)value).TimeOfDay;
        }
    }

    [ValueConversion(typeof(object), typeof(TextAlignment))]
    public class GetAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object prop;
            try
            {
                System.Windows.Controls.DataGridCell cell = value as System.Windows.Controls.DataGridCell;
                System.Windows.Controls.DataGridBoundColumn column = cell.Column as System.Windows.Controls.DataGridBoundColumn;
                string PropertyPath = (column.Binding as System.Windows.Data.Binding).Path.Path;
                if (PropertyPath.Contains('.'))
                {
                    prop = cell.DataContext.GetType().GetProperty(PropertyPath.Split('.')[0]).PropertyType;
                    PropertyPath = PropertyPath.Split('.')[1];
                }
                else
                    prop = cell.DataContext.GetType();
                Type type = (prop as Type).GetProperty(PropertyPath).PropertyType;
                if (type == typeof(decimal) || type == typeof(double) || type == typeof(int))
                    return TextAlignment.Right;
                return TextAlignment.Left;
            }
            catch (Exception ex)
            {
                return TextAlignment.Left;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(object), typeof(Type))]
    public class GetTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value == null) ? null : value.GetType();
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(object), typeof(Visibility))]
    public class TaskPaneUIConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value.ToString() == parameter.ToString()) ? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    

    [ValueConversion(typeof(object ),typeof(string ))]
    public class DoubleToString :IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (GParse.ToDouble(value)==0 )
            {
                return string.Empty;
            }
            else
            {
                return value.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value.Equals(string.Empty))
            {
                return 0;
            }
            else
            {
                return GParse.ToDouble(value);
            }
        }
    }

   

    [ValueConversion(typeof(string), typeof(string))]
    public class DateFormatConverter:IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;            
            return DateTime.Parse(value.ToString(), System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).ToString("MM/dd/yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            return DateTime.Parse(value.ToString(), System.Globalization.DateTimeFormatInfo.InvariantInfo).ToShortDateString();
        }
    }

    [ValueConversion(typeof(bool), typeof(bool ))]
    public class RadioButtonTrueFalseConveter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter,System.Globalization.CultureInfo culture)
        {
            if (value == null) return false;
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter,System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
                return parameter;
            else
                return null;
        }

    }
    [ValueConversion(typeof(bool),typeof(byte))]
    public class RadioButtonToByteConveter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if ((bool)parameter == true)
            {
                if ((byte)value == 0)
                    return true ;
                else
                    return false  ;
            }
            else
            {
                if ((byte)value == 0)
                    return false;
                else
                    return true ;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if ((bool)parameter == true)
            {
                if ((bool)value == true)
                    return 0;
                else
                    return 1;
            }
            else
            {
                if ((bool)value == true)
                    return 1;
                else
                    return 0;
            }
            //return value.Equals(true) ? parameter : Binding.DoNothing;
        }

    }
    [ValueConversion(typeof(string),typeof(decimal  ))]
    public class StringToDecimalConverter:IValueConverter 
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
            
                if ( value.ToString()  != "0")
                {
                    return value ;
                }
                else
                {
                    return "";
                }
            
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value=="")
            { return 0 ;}
            else
            {
                return value ;
            }
        }
    }

    public class ByteToBooleanConverter:IValueConverter 
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if((byte)value == 1)
            {
                return true ;
            }
            else
            {
                return false;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if((Boolean)value ==true )
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
    public class IntToPercentConveter:IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return GParse.ToDecimal (value) * 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return GParse.ToInteger (value) * 0.01;
        }
    }
    public class BooleanOrConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            foreach (object value in values)
            {
                if ((bool)value == true)
                {
                    return true;
                }
            }
            return false;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    public class SumMultipleWidthConverter:IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double Total=0;
            foreach(var val in values )
            {
                Total += GParse.ToDouble(val);
            }
            return Total;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    [ValueConversion(typeof(object), typeof(object))]
    internal class HideZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == DBNull.Value)
                return string.Empty;
            if (GParse.ToDecimal(value) == 0)
                return string.Empty;
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            return GParse.ToDecimal(value);
        }
    }
}
