using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Dapper;
using System.Collections.ObjectModel;
using GTransfer.Library;
using GTransfer.Models;

namespace GTransfer.UserInterfaces
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
          
            

            try
            {
                
                InitializeComponent();
                Loaded += (sender, e) => MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                cmbFYear.DisplayMemberPath = "PhiscalID";
                using (SqlConnection conn = new SqlConnection(GlobalClass.DataConnectionString))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmbFYear.ItemsSource = new List<FiscalYear>(conn.Query<FiscalYear>("SELECT * FROM PhiscalYear ORDER BY BeginDate Desc"));
                    cmbFYear.SelectedIndex = 0;                    
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                GlobalClass.ProcessError(ex, "Login");
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var CurFiscalYear = cmbFYear.SelectedItem as FiscalYear;
                using (SqlConnection conn = new SqlConnection(GlobalClass.DataConnectionString))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();

                    var adInfos = conn.Query<UserAttribs>(
                        @"SELECT  SC.COLUMN_NAME, SC.DATA_TYPE, ISNULL( SEP.VALUE, SC.COLUMN_NAME) DESCRIPTION FROM 
                            SYS.TABLES ST JOIN INFORMATION_SCHEMA.COLUMNS  SC ON ST.name = SC.TABLE_NAME
                            JOIN SYS.COLUMNS C ON SC.COLUMN_NAME = C.NAME AND C.OBJECT_ID = ST.OBJECT_ID
                            LEFT JOIN SYS.EXTENDED_PROPERTIES SEP ON ST.OBJECT_ID = SEP.MAJOR_ID
                            AND C.COLUMN_ID = SEP.MINOR_ID WHERE ST.NAME = 'USERPROFILES' 
                            AND COLUMN_NAME NOT IN ('UNAME', 'PASSWORD', 'DESCRIPTION', 'DISLIMIT', 'ACCOUNT', 'ROLE', 'PARENT', 'TYPE')
                            ORDER BY [DATA_TYPE], [COLUMN_NAME]");


                    cmd.CommandText = @"SELECT * FROM USERPROFILES WHERE UNAME = @uid";
                    cmd.Parameters.AddWithValue("@uid", txtUserName.Text);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        dr.Read();
                        if (dr["Password"].ToString() != Encrypt(txtPassword.Password, "AmitLalJoshi"))
                        {
                            MessageBox.Show("Invalid Username or Password", "Login", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                        CurFiscalYear = new FiscalYear();
                        GlobalClass.CurrentUser = new UserProfiles
                        {
                            UNAME = dr["UNAME"].ToString(),
                            ROLE = dr["ROLE"].ToString(),
                            DESCRIPTION = dr["DESCRIPTION"].ToString(),
                            DISLIMIT = (decimal)dr["DISLIMIT"],
                            ACCOUNT = dr["ACCOUNT"].ToString(),
                            PARENT = dr["PARENT"].ToString(),
                            TYPE = dr["TYPE"].ToString()
                        };


                        GlobalClass.CurrentUser.UserRights = new ObservableCollection<UserAttribs>(adInfos);
                        foreach (UserAttribs ua in GlobalClass.CurrentUser.UserRights)
                        {
                            ua.VALUE = dr[ua.COLUMN_NAME];
                        }
                        dr.Close();


                     //   GlobalClass.CurrentUser.MenuRights = new ObservableCollection<MenuRight>(conn.Query<MenuRight>(string.Format("SELECT UNAME, MID, [OPEN], NEW, EDIT, [DELETE], [PRINT], EXPORT FROM tblUserRights WHERE UNAME = '{0}'", GlobalClass.CurrentUser.UNAME)));

                        //if (!CheckSession())
                        //    return;
                        GlobalClass.CurFiscalYear = CurFiscalYear;
                        cmd.CommandText = "SELECT * FROM COMPANY";
                        using (dr = cmd.ExecuteReader())
                        {
                            dr.Read();
                            GlobalClass.company = new Company
                            {
                                INITIAL = dr["INITIAL"].ToString(),
                                NAME = dr["NAME"].ToString(),
                                ADDRESS = dr["ADDRESS"].ToString(),
                                TELA = dr["TELA"].ToString(),
                                TELB = dr["TELB"].ToString(),
                                VAT = dr["VAT"].ToString()
                            };
                            dr.Close();
                        }

                        var m = new MainWindow();
                        m.Title = "WMS -" + GlobalClass.company.NAME;
                        m.Show();
                        this.Close();
                      // ActivityLog.CreateLogTables();

                    }
                    else
                    {
                        MessageBox.Show("Invalid Username or Password", "Login", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }

                }
            }
            catch (Exception ex)
            {
                GlobalClass.ProcessError(ex, "Login Error");
            }
            finally
            {
                GC.Collect();
            }
        }

        //private void btnLogin_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        var CurFiscalYear = cmbFYear.SelectedItem as FiscalYear;
        //        //if (!CurFiscalYear.ISACTIVE)
        //        //{
        //        //    if (MessageBox.Show("You are logging into Previous Fiscal year. Do you want to continue?", "Login", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
        //        //        return;
        //        //}
        //        using (SqlConnection conn = new SqlConnection(GlobalClass.DataConnectionString))
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            conn.Open();

                    
                    
        //            cmd.CommandText = @"SELECT U.UNAME, U.PASSWORD, U.ROLE FROM USERPROFILES U WHERE UNAME =@uid";
        //            cmd.Parameters.AddWithValue("@uid", txtUserName.Text);
        //            SqlDataReader dr = cmd.ExecuteReader();
        //            if (dr.HasRows)
        //            {
        //                dr.Read();
        //                string UserPwd = dr["Password"].ToString();
        //                GlobalClass.CurUser = new UserProfiles
        //                {
        //                    UNAME = dr["UNAME"].ToString(),
        //                    ROLE = dr["ROLE"].ToString()
        //                };
        //                dr.Close();
        //                if (UserPwd == Encrypt(txtPassword.Password, "AmitLalJoshi"))
        //                {

        //                    //if (!CheckSession())
        //                    //    return;
        //                    GlobalClass.CurFiscalYear = CurFiscalYear;
        //                    cmd.CommandText = "SELECT * FROM COMPANY";
        //                    using (dr = cmd.ExecuteReader())
        //                    {
        //                        dr.Read();
        //                        GlobalClass.company = new Company
        //                        {
        //                            INITIAL = dr["INITIAL"].ToString(),
        //                            NAME = dr["NAME"].ToString(),
        //                            ADDRESS = dr["ADDRESS"].ToString(),
        //                            TELA = dr["TELA"].ToString(),
        //                            TELB = dr["TELB"].ToString(),
        //                            VAT = dr["VAT"].ToString()
        //                        };
        //                        dr.Close();
        //                    }
                            
        //                    var m = new MainWindow();
        //                    m.Title = "IMS POS - " + GlobalClass.company.NAME;
        //                    m.Show();
        //                    this.Close();
        //                }
        //                else
        //                {
        //                    MessageBox.Show("Invalid Username or Password", "Login", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        //                }
        //            }
        //            else
        //            {
        //                MessageBox.Show("Invalid Username or Password", "Login", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobalClass.ProcessError(ex, "Login Error");
        //    }
        //    finally
        //    {
        //        GC.Collect();
        //    }
        //}

       

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            App.Current.Shutdown();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectionStart = 0;
            ((TextBox)sender).SelectionLength = ((TextBox)sender).Text.Length;
        }

        private void txtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            ((PasswordBox)sender).SelectAll();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnLogin_Click(this, null);
        }

        private void cmbDB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        string Encrypt(string Text, string Key)
        {
            int i;
            string TEXTCHAR;
            string KEYCHAR;
            string encoded = string.Empty;
            for (i = 0; i < Text.Length; i++)
            {
                TEXTCHAR = Text.Substring(i, 1);
                var keysI = (i % Key.Length) + 1;
                KEYCHAR = Key.Substring(keysI, Key.Length - keysI);

                var encrypted = Microsoft.VisualBasic.Strings.Asc(TEXTCHAR) ^ Microsoft.VisualBasic.Strings.Asc(KEYCHAR);
                encoded += Microsoft.VisualBasic.Strings.Chr(encrypted);
            }
            return encoded;
        }
    }
}
