﻿using Syncfusion.UI.Xaml.Grid.Cells;
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
    /// Interaction logic for pickingList.xaml
    /// </summary>
    public partial class pickingList : UserControl
    {
        public pickingList()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.pickingListViewModel(Report);
        }
    }
}
