using Dapper;
using GTransfer.Interfaces;
using GTransfer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GTransfer.Library
{
    public static class GlobalClass
    {
        public static string DataConnectionString = "SERVER = PRO-PC; DATABASE = DB_POSDBS; UID = sa; PWD = tebahal";
        // public static string DataConnectionString = "SERVER = IMS-D1; DATABASE = Miniso; UID = sa; PWD = tebahal";
        public static UserProfiles CurrentUser = new UserProfiles() {UNAME="Test" };

        public static object CopyPropertyValuesOnlyPresent(object source, object Destination)
        {
            if (source == null)
                return null;
            //if (source.GetType() != Destination.GetType() && source.GetType().BaseType != Destination.GetType())
            //    return null;
            foreach (PropertyInfo pi in source.GetType().GetProperties())
            {
                object value = pi.GetValue(source, null);
                if (value != null && value.GetType().GetProperties().Count() > 0 && value.GetType().Namespace != "System")
                { }
                //CopyPropertyValues(value, Destination.GetType().GetProperty(pi.Name).GetValue(Destination, null));
                else
                    if (pi.CanWrite)
                {
                    Destination.GetType().GetProperty(pi.Name).SetValue(Destination, pi.GetValue(source, null), null);
                }
            }
            return Destination;
        }
        public static void ProcessError(Exception ex, string Caption)
        {
            StreamWriter fs;

            string Log;
            string HResult;
            string ExceptionType;
            string Message;
            string user;
            try
            {
                ex = GetRootException(ex);
                MessageBox.Show(ex.Message, Caption, MessageBoxButton.OK, MessageBoxImage.Error);
                HResult = Marshal.GetHRForException(ex).ToString().PadLeft(20);
                ExceptionType = ex.GetType().Name.PadLeft(40);
                Message = ex.Message.PadLeft(200);
                user = "";//User.UserName.PadLeft(20);
                Log = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + " " + user + "  " + ExceptionType + "  " + HResult + "  " + Message;
                if (!File.Exists(Environment.CurrentDirectory + "\\ErrorLog.Log"))
                {
                    File.Create(Environment.CurrentDirectory + "\\ErrorLog.Log").Close();
                }
                fs = File.AppendText(Environment.CurrentDirectory + "\\ErrorLog.Log");
                fs.WriteLine(Log);
                fs.Close();
            }
            catch (Exception expt)
            {
                MessageBox.Show(expt.Message, "Error Processor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static Exception GetRootException(Exception ex)
        {
            while (ex.InnerException != null)
                ex = ex.InnerException;
            return ex;
        }


        public static void TreeComboSearch(ComboBox cb, Key e, BaseViewModelWithTree vm, bool ByCode = false, String Code = "")
        {
            if ((e == Key.Down) || (e == Key.Up) || (e == Key.Escape) || (e == Key.Left) || (e == Key.Right))
                return;
            TextBox txtBox = cb.Template.FindName("PART_EditableTextBox", cb) as TextBox;
            if (txtBox != null)
            {
                if (e == Key.Return)
                {

                    if (ByCode == false)
                    {
                        vm.SearchCriteria = txtBox.Text;
                        vm.ByCode = false;
                        vm.SearchInTree(vm.Root);
                    }
                    else
                    {
                        vm.SearchCriteria = Code;
                        vm.ByCode = true;
                        vm.SearchInTree(vm.Root);
                    }
                }
                else
                {

                    string K = e.ToString();
                    string SearchTxt = txtBox.Text;
                    if (SearchTxt == null) return;
                    ObservableCollection<ITreeNode> TNodes = new ObservableCollection<ITreeNode>();
                    var Nodes = vm.TotalNodes.Where<ITreeNode>(nod => (nod.NodeName.ToUpper()).Contains(SearchTxt.ToUpper())).ToList<ITreeNode>();
                    foreach (ITreeNode node in Nodes)
                    {
                        TNodes.Add(node);
                    }
                    cb.ItemsSource = TNodes;

                    cb.SetValue(ComboBox.IsDropDownOpenProperty, true);
                    cb.SelectedIndex = -1;
                    txtBox.Text = SearchTxt;
                    txtBox.SelectionLength = 0;
                    txtBox.CaretIndex = txtBox.Text.Length;
                }
            }
        }

        public static T CopyPropertyValues<T>(T source, T Destination)
        {
            if (source == null)
                return default(T);
            //if (source.GetType() != Destination.GetType() && source.GetType().BaseType != Destination.GetType())
            //    return null;
            foreach (PropertyInfo pi in source.GetType().GetProperties())
            {
                try
                {

                    object value = pi.GetValue(source, null);
                    if (value != null && value.GetType().GetProperties().Count() > 0 && value.GetType().Namespace != "System")
                        CopyPropertyValues(value, Destination.GetType().GetProperty(pi.Name).GetValue(Destination, null));
                    else
                        if (pi.CanWrite)
                        Destination.GetType().GetProperty(pi.Name).SetValue(Destination, pi.GetValue(source, null), null);
                }
                catch (Exception)
                {

                }
            }
            return Destination;
        }

        public static DynamicParameters GetParamters(object obj)
        {
            DynamicParameters dp = new DynamicParameters();
            foreach (PropertyInfo pi in obj.GetType().GetProperties())
            {
                object value = null;
                IgnorePropertyAttribute IgnoreProperty = pi.GetCustomAttribute<IgnorePropertyAttribute>();
                if (IgnoreProperty != null && IgnoreProperty.Value)
                    continue;
                ChangePropertyDataTypeAttribute ChangeProperty = pi.GetCustomAttribute<ChangePropertyDataTypeAttribute>();
                if (ChangeProperty != null)
                {
                    switch (ChangeProperty.Value.Name.ToLower())
                    {
                        case "string":
                            value = pi.GetValue(obj, null).ToString();
                            break;
                    }
                }
                else
                    value = pi.GetValue(obj, null);
                dp.Add(pi.Name, value);
            }
            return dp;
        }

        public static T GetFrameworkElement<T>(string Xaml)
        {

            var grdEncoding = new System.Text.ASCIIEncoding();
            var grdBytes = grdEncoding.GetBytes(Xaml);
            return (T)System.Windows.Markup.XamlReader.Load(new MemoryStream(grdBytes));

        }

        public static T FindVisualChild<T>(DependencyObject obj, string Name) where T : DependencyObject
        {

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T && (child as Control).Name == Name)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child, Name);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
        public static List<T> FindVisualChildren<T>(DependencyObject obj) where T : DependencyObject
        {
            List<T> children = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var o = VisualTreeHelper.GetChild(obj, i);
                if (o != null)
                {
                    if (o is T)
                        children.Add((T)o);

                    children.AddRange(FindVisualChildren<T>(o)); // recursive
                }
            }
            return children;
        }

        public static T FindUpVisualTree<T>(DependencyObject initial) where T : DependencyObject
        {
            DependencyObject current = initial;

            while (current != null && current.GetType() != typeof(T))
            {
                current = VisualTreeHelper.GetParent(current);
            }
            return current as T;
        }
    }
}
