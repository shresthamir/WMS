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

namespace GTransfer.Controls
{
    /// <summary>
    /// Interaction logic for ucActionBar.xaml
    /// </summary>
    public partial class ucActionBar : UserControl
    {
        private bool _hasGroup;
        public ucActionBar()
        {
            InitializeComponent();
            this.Loaded += UcActionBar_Loaded;

        }

        private void UcActionBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (_hasGroup == true)
            {
                NewButton.Visibility = Visibility.Collapsed;
                NewGroupButton.Visibility = Visibility.Visible;
            }
            else
            {
                NewButton.Visibility = Visibility.Visible;
                NewGroupButton.Visibility = Visibility.Collapsed;
            }
        }

        public bool HasGroup
        {
            get { return _hasGroup; }
            set
            {
                if (value == true)
                {
                    NewButton.Visibility = Visibility.Collapsed;
                    NewGroupButton.Visibility = Visibility.Visible;
                }
                else
                {
                    NewButton.Visibility = Visibility.Visible;
                    NewGroupButton.Visibility = Visibility.Collapsed;
                }
                _hasGroup = value;
            }
        }
    }
}
