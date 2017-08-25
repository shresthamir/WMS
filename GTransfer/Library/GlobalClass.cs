using Dapper;
using GTransfer.Interfaces;
using GTransfer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
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
        //public static string DataConnectionString = "SERVER = PRO-PC; DATABASE = DB_POSDBS; UID = sa; PWD = tebahal";
        public static string DataConnectionString;
        public static UserProfiles CurrentUser = new UserProfiles() { UNAME = "admin" };
        public static string PhiscalId = "073/74";
        public static string DIVISION = "MMX";
        
        static GlobalClass()
        {
            SqlConnectionStringBuilder sbr = new SqlConnectionStringBuilder();
            if(File.Exists(Path.Combine(Environment.SystemDirectory, "dbpath.dll")))
            {
                string[] connProps = File.ReadAllLines(Path.Combine(Environment.SystemDirectory, "dbpath.dll"));
                for(int i = 0; i<connProps.Count(); i++)
                {
                    switch(i)
                    {
                        case 0:
                            sbr.UserID = connProps[i];
                            break;
                        case 1:
                            sbr.Password = connProps[i];
                            break;
                        case 2:
                            sbr.InitialCatalog = connProps[i];
                            break;
                        case 3:
                            sbr.DataSource = connProps[i];
                            break;
                        case 5:
                            DIVISION = connProps[i];
                            break;
                    }
                }
                DataConnectionString = sbr.ConnectionString;
            }
        }


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

        public static int GetBillSequences(IDbCommand Cmd, string VNAME, string VoucherType, string VoucherName, String Series, ref string Vno, ref String Chalanno, ref string Vnum)
        {
            string Div = DIVISION;
            int Curno;           
            try
            {
                Cmd.CommandText = "SELECT isnull(CURNO,0) CURNO FROM RMD_SEQUENCES WHERE VNAME = '" + VNAME + "' AND DIVISION LIKE  '" + Div + "'";
                var cur = Cmd.ExecuteScalar();
                if (cur != null)
                {
                    Curno = Convert.ToInt32(cur);
                    Vnum = Curno.ToString();
                    Vno = Series + Convert.ToString(Curno);
                    Chalanno = Vno;
                }
                else
                {
                    Cmd.CommandText = "INSERT INTO RMD_SEQUENCES (VNAME,CURNO,DIVISION,VoucherType,VOUCHERNAME) VALUES ( '" + VNAME + "',1,'" + Div + "','" + VoucherType + "','" + VoucherName + "')";
                    Cmd.ExecuteNonQuery();
                    Curno = 1;
                    Vnum = "1";
                    Vno = Series + Convert.ToString(Curno);
                    Chalanno = Vno;
                }
                return Curno;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return 0;

        }

        public static int GetBillSequences(string VNAME, string VoucherType, string VoucherName, String Series, ref string Vno, ref String Chalanno, ref string Vnum)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DataConnectionString))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    GetBillSequences(cmd, VNAME, VoucherType, VoucherName, Series, ref Vno, ref Chalanno, ref Vnum);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return 0;

        }

        /// <summary>
        /// returns BS Date string of given AD Date
        /// </summary>
        /// <param name="Adate">AD Date to be converted</param>
        /// <returns></returns>
        public static string GetBSDate(DateTime Adate)
        {
            try
            {               
                using (SqlConnection Con = new SqlConnection(GlobalClass.DataConnectionString))
                using (SqlCommand Cmd = Con.CreateCommand())
                {
                    Con.Open();
                    Cmd.CommandText = "select dbo.dateToMiti('" + Adate.ToString("dd/MMM/yyyy") + "','/')";
                    var bsdate = Cmd.ExecuteScalar();
                    if (bsdate != null)
                        return (string)bsdate;
                    else
                        return "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public enum VoucherTypeEnum
    {
        Default = 0, Sales = 1, SalesReturn = 2, Purchase = 3, PurchaseReturn = 4, StockIssue = 5, StockReceive = 6, BranchTransferIn = 7,
        BranchTransferOut = 8, StockSettlement = 9, Stockadjustment = 10, Receipe = 11, Journal = 12, Delivery = 13, TaxInvoice = 14,
        CreditNote = 15, DebitNote = 16, PaymentVoucher = 17, ReceiveVoucher = 18, PurchaseOrder = 19, SalesOrder = 20, DeliveryReturn = 21, AccountOpeningBalance = 22, PartyOpeningBalance = 23, OpeningStockBalance = 24, SubLedgerOpeningBalance = 25
    }
}
