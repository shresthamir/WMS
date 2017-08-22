using GTransfer.Models;
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
    /// Interaction logic for GRN.xaml
    /// </summary>
    public partial class ShipmentReceiveLog : UserControl
    {
        public ShipmentReceiveLog()
        {
            InitializeComponent();
            this.DataContext = new ShipmentLogViewModel();
        }

       
    }


   
}
