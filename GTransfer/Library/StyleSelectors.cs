#region Copyright Syncfusion Inc. 2001 - 2015
// Copyright Syncfusion Inc. 2001 - 2015. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;

namespace GTransfer.Library
{
    public class GroupSummaryStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var summaryRecordEntry = item as SummaryRecordEntry;
            if (summaryRecordEntry.SummaryRow.ShowSummaryInRow)
                return App.Current.Resources["groupSummaryCell"] as Style;
            return App.Current.Resources["normalgroupSummaryCell"] as Style;
        }
    }

    public class TableSummaryStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var summaryRecordEntry = item as SummaryRecordEntry;
            if (summaryRecordEntry.SummaryRow.ShowSummaryInRow)
                return App.Current.Resources["tableSummaryCell"] as Style;
            return App.Current.Resources["normaltableSummaryCell"] as Style;
        }
    }
}