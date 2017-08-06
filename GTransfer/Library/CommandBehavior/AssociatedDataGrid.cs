using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;

namespace GTransfer.CommandBehavior
{
    public static class AssociatedDataGrid
    {
        private static DataGrid Maindg;
        public static DataGrid GetBottom(DependencyObject obj)
        {
            return (DataGrid)obj.GetValue(BottomProperty);
        }

        public static void SetBottom(DependencyObject obj, DataGrid value)
        {
            obj.SetValue(BottomProperty, value);
        }

        // Using a DependencyProperty as the backing store for BottomDataGrid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomProperty =
            DependencyProperty.RegisterAttached("Bottom", typeof(DataGrid), typeof(AssociatedDataGrid), new UIPropertyMetadata(null, AssociatedDataGridPropertyChanged));



        public static DataGrid GetRight(DependencyObject obj)
        {
            return (DataGrid)obj.GetValue(RightProperty);
        }

        public static void SetRight(DependencyObject obj, DataGrid value)
        {
            obj.SetValue(RightProperty, value);
        }

        // Using a DependencyProperty as the backing store for RightDataGrid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightProperty =
            DependencyProperty.RegisterAttached("Right", typeof(DataGrid), typeof(AssociatedDataGrid), new UIPropertyMetadata(null, AssociatedDataGridPropertyChanged));



        public static DataGrid GetLeft(DependencyObject obj)
        {
            return (DataGrid)obj.GetValue(LeftProperty);
        }

        public static void SetLeft(DependencyObject obj, DataGrid value)
        {
            obj.SetValue(LeftProperty, value);
        }

        // Using a DependencyProperty as the backing store for LeftDataGrid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.RegisterAttached("Left", typeof(DataGrid), typeof(AssociatedDataGrid), new UIPropertyMetadata(null, AssociatedDataGridPropertyChanged));



        public static DataGrid GetTop(DependencyObject obj)
        {
            return (DataGrid)obj.GetValue(TopProperty);
        }

        public static void SetTop(DependencyObject obj, DataGrid value)
        {
            obj.SetValue(TopProperty, value);
        }

        // Using a DependencyProperty as the backing store for TopDataGrid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopProperty =
            DependencyProperty.RegisterAttached("Top", typeof(DataGrid), typeof(AssociatedDataGrid), new UIPropertyMetadata(null, AssociatedDataGridPropertyChanged));



        public static int GetColumnSpan(DependencyObject obj)
        {
            return (int)obj.GetValue(ColumnSpanProperty);
        }

        public static void SetColumnSpan(DependencyObject obj, int value)
        {
            obj.SetValue(ColumnSpanProperty, value);
        }

        // Using a DependencyProperty as the backing store for ColumnSpan.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnSpanProperty =
            DependencyProperty.RegisterAttached("ColumnSpan", typeof(int), typeof(AssociatedDataGrid), new UIPropertyMetadata(0));



        private static Nullable<Double> GetInternalWidthOnColumn(DependencyObject obj)
        {
            return (double)obj.GetValue(InternalWidthOnColumnProperty);
        }

        private static void SetInternalWidthOnColumn(DependencyObject obj, Nullable<Double> value)
        {
            obj.SetValue(InternalWidthOnColumnProperty, value);
        }

        // Using a DependencyProperty as the backing store for InternalWidthOnColumn.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty InternalWidthOnColumnProperty =
            DependencyProperty.RegisterAttached("InternalWidthOnColumn", typeof(Nullable<Double>), typeof(AssociatedDataGrid), new UIPropertyMetadata(null, InternalWidthOnColumnPropertyChanged));


        private static void InternalWidthOnColumnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ((DataGridColumn)sender).Width = ((Nullable<double>)e.NewValue).Value;
            }
        }

        private static void SynchronizeHorizontalDataGrid(DataGrid source, DataGrid associated)
        {
            associated.HeadersVisibility = source.HeadersVisibility & (DataGridHeadersVisibility.Column);
            associated.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        private static void SynchronizeVerticalDataGrid(DataGrid source, DataGrid associated)
        {
            associated.HeadersVisibility = source.HeadersVisibility & (DataGridHeadersVisibility.Row);
            Maindg.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;

            int sourceColIndex = 0;

            bool bAllowColumnDisplayIndexSynchronization = true;
            for (int i = 0; i < associated.Columns.Count; i++)
            {
                if (GetColumnSpan(associated.Columns[i]) > 1)
                {
                    bAllowColumnDisplayIndexSynchronization = false;
                    break;
                }
            }


            for (int associatedColIndex = 0; associatedColIndex < associated.Columns.Count; associatedColIndex++)
            {
                var colAssociated = associated.Columns[associatedColIndex];
                int columnSpan = GetColumnSpan(colAssociated);
                //var colAssociated = source.Columns[associatedColIndex];
                //int columnSpan = GetColumnSpan(colAssociated);
                if (sourceColIndex >= source.Columns.Count)
                    break;

                if (columnSpan <= 1)
                {
                    var colSource = source.Columns[sourceColIndex];
                    Binding binding = new Binding();
                    binding.Mode = BindingMode.TwoWay;
                    binding.Source = colSource;
                    binding.Path = new PropertyPath(DataGridColumn.WidthProperty);
                    BindingOperations.SetBinding(colAssociated, DataGridColumn.WidthProperty, binding);

                    //column visibility
                    binding = new Binding();
                    binding.Mode = BindingMode.TwoWay;
                    binding.Source = colSource;
                    binding.Path = new PropertyPath(DataGridColumn.VisibilityProperty);
                    BindingOperations.SetBinding(colAssociated, DataGridColumn.VisibilityProperty, binding);
                    if (bAllowColumnDisplayIndexSynchronization)
                    {
                        //binding = new Binding();
                        //binding.Mode = BindingMode.TwoWay;
                        //binding.Source = colSource;
                        //binding.Path = new PropertyPath(DataGridColumn.DisplayIndexProperty);
                        //BindingOperations.SetBinding(colAssociated, DataGridColumn.DisplayIndexProperty, binding);
                    }

                    sourceColIndex++;

                }
                else
                {
                    MultiBinding multiBinding = new MultiBinding();

                    multiBinding.Converter = WidthConverter;
                    for (int i = 0; i < columnSpan; i++)
                    {
                        // 1 binding pour forcer le raffraichissement
                        // + 1 binding pour avoir la colonne source
                        var colSource = source.Columns[sourceColIndex];
                        Binding binding = new Binding();
                        binding.Source = colSource;
                        binding.Path = new PropertyPath(DataGridColumn.WidthProperty);
                        multiBinding.Bindings.Add(binding);
                        binding = new Binding();
                        binding.Source = colSource;
                        multiBinding.Bindings.Add(binding);
                        sourceColIndex++;
                    }
                    // Rq : use another internal Property
                    //      because original Width property is scratch by Framework AFTER binding is set !
                    BindingOperations.SetBinding(colAssociated, InternalWidthOnColumnProperty, multiBinding);

                }
            }
        }

        private static IMultiValueConverter WidthConverter = new WidthConverterClass();
        private class WidthConverterClass : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                double result = 0;
                foreach (var value in values)
                {
                    DataGridColumn column = value as DataGridColumn;
                    if (column != null)
                    {
                        result = result + column.ActualWidth;
                    }
                }
                return result;
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private static void AssociatedDataGridPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Gestion des synchonisations des colonnes
            DataGrid dgSource = (DataGrid)sender;
            Maindg = dgSource;

            DataGrid dgBottom = GetBottom(dgSource);
            DataGrid dgTop = GetTop(dgSource);
            DataGrid dgLeft = GetLeft(dgSource);
            DataGrid dgRight = GetRight(dgSource);
            //CreateAssociatedColumns(dgSource, dgBottom);
            if ((dgBottom == null) &&
                (dgTop == null) &&
                (dgLeft == null) &&
                (dgRight == null))
                dgSource.RemoveHandler(ScrollViewer.ScrollChangedEvent, new RoutedEventHandler(xDataGrid_ScrollChanged));
            else
                dgSource.AddHandler(ScrollViewer.ScrollChangedEvent, new RoutedEventHandler(xDataGrid_ScrollChanged));

            if (dgBottom != null)
            {
                dgBottom.Loaded+=dgBottom_Loaded;
                SynchronizeVerticalDataGrid(dgSource, dgBottom);
                dgBottom.AddHandler(ScrollViewer.ScrollChangedEvent, new RoutedEventHandler(yDataGrid_ScrollChanged));
            }


            if (dgTop != null)
            {
                SynchronizeVerticalDataGrid(dgSource, dgTop);
            }
            if (dgRight != null)
            {
                SynchronizeHorizontalDataGrid(dgSource, dgRight);
            }
            if (dgLeft != null)
            {
                SynchronizeHorizontalDataGrid(dgSource, dgLeft);
            }
        }

        static void dgBottom_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid DestDg = (DataGrid)sender;
            DataGrid SourceDg = Maindg ;
            
                for (int col = 0; col < DestDg.Columns.Count ; col++)
                {
                        int colspan = GetColumnSpan(DestDg.Columns[col]);
                        if (colspan > 0)
                        {
                            double width = 0;
                            for (int i = 0; i < colspan; i++)
                            {
                                width = width + DestDg.Columns[col].ActualWidth;
                                col = col + 1;
                            }
                        }
                        else
                        {
                            DestDg.Columns[col].Width = SourceDg.Columns[col].ActualWidth;
                        }
                    }
                
            
        }
        private static  void CreateAssociatedColumns(DataGrid SourceDg,DataGrid DestDg)
        {
            if(DestDg.Columns.Count < SourceDg.Columns.Count )
            {
                for(int col = 0 ;col < SourceDg.Columns.Count-1;col++ )
                {
                    if (DestDg.Columns.Count >= col)
                    {
                        DataGridTextColumn dgcolumn = new DataGridTextColumn();
                        //dgcolumn.Width = SourceDg.Columns[col].Width;
                        DestDg.Columns.Add(dgcolumn);
                    }
                    else
                    {
                        int colspan = GetColumnSpan(DestDg.Columns[col]);
                        if(colspan > 0)
                        {
                            double width = 0;
                            for (int i = 0; i < colspan;i++ )
                            {
                                width = width + DestDg.Columns[col].ActualWidth;
                                col = col + 1;
                            }
                        }
                        else
                        {
                            DestDg.Columns[col].Width = SourceDg.Columns[col].Width;
                        }
                    }
                }
            }
        }
        private const string ScrollViewerNameInTemplate = "DG_ScrollViewer";
        private static void yDataGrid_ScrollChanged(object sender, RoutedEventArgs eBase)
        {
            ScrollChangedEventArgs e = (ScrollChangedEventArgs)eBase;
            // ScrollView source à l'origine du Scroll
            ScrollViewer sourceScrollViewer = (ScrollViewer)e.OriginalSource;

            SynchronizeScrollHorizontalOffset(Maindg, sourceScrollViewer);
            //SynchronizeScrollHorizontalOffset(AssociatedDataGrid.GetTop((DependencyObject)sender), sourceScrollViewer);

            //SynchronizeScrollVerticalOffset(AssociatedDataGrid.GetRight((DependencyObject)sender), sourceScrollViewer);
            //SynchronizeScrollVerticalOffset(AssociatedDataGrid.GetLeft((DependencyObject)sender), sourceScrollViewer);
        }

        private static void xDataGrid_ScrollChanged(object sender, RoutedEventArgs eBase)
        {
            ScrollChangedEventArgs e = (ScrollChangedEventArgs)eBase;
            // ScrollView source à l'origine du Scroll
            ScrollViewer sourceScrollViewer = (ScrollViewer)e.OriginalSource;

            SynchronizeScrollHorizontalOffset(AssociatedDataGrid.GetBottom((DependencyObject)sender), sourceScrollViewer);
            SynchronizeScrollHorizontalOffset(AssociatedDataGrid.GetTop((DependencyObject)sender), sourceScrollViewer);

            SynchronizeScrollVerticalOffset(AssociatedDataGrid.GetRight((DependencyObject)sender), sourceScrollViewer);
            SynchronizeScrollVerticalOffset(AssociatedDataGrid.GetLeft((DependencyObject)sender), sourceScrollViewer);
        }

        private static void SynchronizeScrollHorizontalOffset(DataGrid associatedDataGrid, ScrollViewer sourceScrollViewer)
        {
            if (associatedDataGrid != null)
            {
                ScrollViewer associatedScrollViewer = (ScrollViewer)associatedDataGrid.Template.FindName(ScrollViewerNameInTemplate, associatedDataGrid);
                associatedScrollViewer.ScrollToHorizontalOffset(sourceScrollViewer.HorizontalOffset);
            }
        }
        private static void SynchronizeScrollVerticalOffset(DataGrid associatedDataGrid, ScrollViewer sourceScrollViewer)
        {
            if (associatedDataGrid != null)
            {
                ScrollViewer associatedScrollViewer = (ScrollViewer)associatedDataGrid.Template.FindName(ScrollViewerNameInTemplate, associatedDataGrid);
                associatedScrollViewer.ScrollToVerticalOffset(sourceScrollViewer.VerticalOffset);
            }
        }
    }
}
