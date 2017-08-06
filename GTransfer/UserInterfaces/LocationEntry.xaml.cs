using GTransfer.Library;
using GTransfer.ViewModels;
using System;
using System.Collections.Generic;
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

namespace GTransfer.UserInterfaces
{
    /// <summary>
    /// Interaction logic for LocationEntry.xaml
    /// </summary>
    /// 
    ///
    public partial class LocationEntry : UserControl
    {
        LocationEntryViewModel LEVM;
        public LocationEntry()
        {
            InitializeComponent();
            this.DataContext = LEVM = new ViewModels.LocationEntryViewModel();

        }
        private void txtWarePhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }
        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;
            if (item != null && !item.IsFocused)
                item.Focus();
        }

        #region methods
        private void CheckIsNumeric(TextCompositionEventArgs e)
        {
            int result;
            if (!(int.TryParse(e.Text, out result) || e.Text == "."))
            {
                e.Handled = true;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectionStart = 0;
            ((TextBox)sender).SelectionLength = ((TextBox)sender).Text.Length;
        }

        #endregion

        private void ComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            GlobalClass.TreeComboSearch(sender as ComboBox, e.Key, LEVM);
            return;
        }
    }
}
