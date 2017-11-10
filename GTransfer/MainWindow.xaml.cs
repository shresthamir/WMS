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
using Xceed.Wpf.AvalonDock.Layout;

namespace GTransfer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void LocationEntry_Click(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Content = new UserInterfaces.LocationEntry();
            la.IsActive = true;
            la.Title = "Location";
            LayDocPane.Children.Add(la);
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        { }

        private void ItemLocationMapping_Click(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Content = new UserInterfaces.ItemLocationMapping();
            la.IsActive = true;
            la.Title = "Item-Location Mapping";
            LayDocPane.Children.Add(la);
        }

        private void RequisitionEntry_Click(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Content = new UserInterfaces.requisitionEntry();
            la.IsActive = true;
            la.Title = "Requisition Entry";
            LayDocPane.Children.Add(la);
        }

        private void GRN_Click(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Content = new UserInterfaces.ShipmentVariance();
            la.IsActive = true;
            la.Title = "Shipment Receive Variance";
            LayDocPane.Children.Add(la);
        }

        private void SRL_Click(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Content = new UserInterfaces.ShipmentReceiveLog();
            la.IsActive = true;
            la.Title = "Shipment Receive Log";
            LayDocPane.Children.Add(la);
        }

        private void PickingListEntry_Click(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Content = new UserInterfaces.pickingList();
            la.IsActive = true;
            la.Title = "Picking List";
            LayDocPane.Children.Add(la);
        }

        private void LoationTransferEntry_Click(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Content = new UserInterfaces.LocationTransfer();
            la.IsActive = true;
            la.Title = "Location Transfer";
            LayDocPane.Children.Add(la);
        }

        private void BT_Click(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Content = new UserInterfaces.StockIssueVariance();
            la.IsActive = true;
            la.Title = "Stock Issue Variance";
            LayDocPane.Children.Add(la);
        }

        private void locationWiseItem_Click(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Content = new Reports.ItemWiseStockReport();
            la.IsActive = true;
            la.Title = "LocationWise Item Report";
            LayDocPane.Children.Add(la);
        }

        private void ItemWiseLocation_Click(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Content = new Reports.LocationWiseStockReport();
            la.IsActive = true;
            la.Title = "ItemWise Location Report";
            LayDocPane.Children.Add(la);
        }

        private void goodsReceivedDetail_Click(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Content = new Reports.GoodsReceivedReport();
            la.IsActive = true;
            la.Title = "Goods Received Detail";
            LayDocPane.Children.Add(la);
        }

        private void packingList_Click(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Content = new Reports.PackagingList();
            la.IsActive = true;
            la.Title = "Packing List";
            LayDocPane.Children.Add(la);
        }

        private void stockMovementReport_Click(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Content = new Reports.StockMovementReport();
            la.IsActive = true;
            la.Title = "Stock Movement Report";
            LayDocPane.Children.Add(la);
        }

        private void goodsReceivedSummary_Click(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Content = new Reports.GoodsReceivedSummaryReport();
            la.IsActive = true;
            la.Title = "Goods Received Summary";
            LayDocPane.Children.Add(la);
        }
        private void StockSettlementEntry_Click(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Content = new UserInterfaces.StockSettlementEntry();
            la.IsActive = true;
            la.Title = "Stock Settlement Entry";
            LayDocPane.Children.Add(la);
        }
    }
}
