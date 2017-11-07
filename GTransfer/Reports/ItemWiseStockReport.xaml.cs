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

namespace GTransfer.Reports
{
    /// <summary>
    /// Interaction logic for LocationWiseItemReport.xaml
    /// </summary>
    public partial class ItemWiseStockReport : UserControl
    {
        public ItemWiseStockReport()
        {
            InitializeComponent();
            this.DataContext = new Reports.vmItemWiseStockReport(Report);
        }
    }
}
