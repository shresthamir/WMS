﻿using System;
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
            la.Content = new UserInterfaces.GRN();
            la.IsActive = true;
            la.Title = "GRN";
            LayDocPane.Children.Add(la);
        }
    }
}
