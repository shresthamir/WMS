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

namespace GTransfer.Reports
{
    /// <summary>
    /// Interaction logic for GoodsReceivedReport.xaml
    /// </summary>
    public partial class PackagingList : UserControl
    {
        public PackagingList()
        {
            InitializeComponent();
            this.DataContext = new PackagingListVM(Report);
        }
    }
}
