using GTransfer.Interfaces;
using GTransfer.Models;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using GTransfer.Library;

namespace GTransfer.CommandBehavior
{
    class GridTreeBehavior : Behavior<GridTreeControl>
    {
        public short ComboBoxIndex { get; set; }
        public string ComboProperty { get; set; }
        public short CoveredRange { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.ModelLoaded += new EventHandler(AssciatedObject_ModelLoaded);
        }

        private void AssciatedObject_ModelLoaded(object sender, EventArgs e)
        {
            this.AssociatedObject.InternalGrid.QueryCoveredRange += new GridQueryCoveredRangeEventHandler(QueryCoveredRange);
            this.AssociatedObject.InternalGrid.QueryCellInfo += new GridQueryCellInfoEventHandler(QyeryCellInfo);
            //this.AssociatedObject.InternalGrid.CommitCellInfo += new GridCommitCellInfoEventHandler(CommitCell);
            this.AssociatedObject.InternalGrid.CurrentCellValidating += new CurrentCellValidatingEventHandler(CellValidating);

            this.AssociatedObject.InternalGrid.Model.Options.ActivateCurrentCellBehavior = GridCellActivateAction.DblClickOnCell;
            this.AssociatedObject.InternalGrid.Model.Options.ShowErrorIconOnEditing = true;
            GridTooltipService.SetShowTooltips(this.AssociatedObject.InternalGrid, true);
            GridCommentService.SetShowComment(this.AssociatedObject.InternalGrid, true);

            

            //this.AssociatedObject.InternalGrid.Model.SaveCellText += new GridCellTextEventHandler(Savetext);
        }

        private void CellValidating(object sender, CurrentCellValidatingEventArgs e)
        {   
            var curcell = this.AssociatedObject.InternalGrid.CurrentCell;
            try
            {
                if (e.Style.ColumnIndex == ComboBoxIndex)
                {

                    if (e.NewValue != e.OldValue)
                    {
                        if (string.IsNullOrEmpty(e.NewValue.ToString()))
                        {
                            return;
                        }
                        var curnod = this.AssociatedObject.InternalGrid.GetNodeAtRowIndex(e.Style.RowIndex);

                        if (curnod != null)
                        {
                            if (curnod.HasChildNodes == true)
                            {
                                ChangeProperty(curnod, e.NewValue.ToString(), e);
                            }
                        }
                        this.AssociatedObject.InternalGrid.InvalidateCells();
                    }
                }

            }
            catch (Exception ex)
            {
                if (ex == e.Style.Exception)
                { MessageBox.Show(ex.Message); }
            }
        }

        private void ChangeProperty(GridTreeNode ParentNod, string value, CurrentCellValidatingEventArgs e)
        {

            foreach (GridTreeNode nod in ParentNod.ChildNodes)
            {
                object mnu = nod.Item;
                mnu.GetType().GetProperty(ComboProperty).SetValue(mnu, value);
                this.AssociatedObject.InternalGrid.InvalidateCell(e.Style.CellRowColumnIndex);
                if (nod.HasChildNodes == true)
                {
                    ChangeProperty(nod, value, e);
                }
            }

        }
        private void CommitCell(object sender, GridCommitCellInfoEventArgs e)
        {

            var CurCell = this.AssociatedObject.InternalGrid.CurrentCell;
            if (CurCell.ColumnIndex == 2)
            {
                CurCell.CancelEdit();
            }
        }

        private void QyeryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {

            var CurrentNod = this.AssociatedObject.InternalGrid.GetNodeAtRowIndex(e.Cell.RowIndex);
            var curCell = this.AssociatedObject.InternalGrid.CurrentCell;
            if (CurrentNod != null)
            {
                if (CurrentNod.Item is ITreeNode)
                {
                    if ((CurrentNod.Item as ITreeNode).IsGroup)
                    {
                        e.Style.Font = new GridFontInfo { FontWeight = System.Windows.FontWeights.SemiBold };
                        if (e.Cell.ColumnIndex == 1)
                        {
                            e.Style.Foreground = Brushes.Black;
                            e.Style.ShowTooltip = true;
                        }
                    }
                }
                if(CurrentNod.Item is MenuRight)
                {
                    if (e.Cell.ColumnIndex > 2 && this.AssociatedObject.Columns.Count >= e.Cell.ColumnIndex)
                    {
                        FormMenu fm = (CurrentNod.Item as MenuRight).AppMenu;
                        byte val = (byte)fm.GetType().GetProperty(this.AssociatedObject.Columns[e.Cell.ColumnIndex-1].MappingName).GetValue(fm);
                        if (val == 0)
                        {
                            e.Style.CellItemTemplate = e.Style.CellEditTemplate =  this.AssociatedObject.FindResource("EmptyTemplate") as DataTemplate;
                        }
                    }
                }
            }
        }

        private void QueryCoveredRange(object sender, GridQueryCoveredRangeEventArgs e)
        {
            var CurrentNod = this.AssociatedObject.InternalGrid.GetNodeAtRowIndex(e.CellRowColumnIndex.RowIndex);
            if (CurrentNod != null)
            {
                if (CurrentNod.Item is Product)
                {
                    Product CurrentMenuitem = CurrentNod.Item as Product;
                    if (CurrentMenuitem.TYPE == "G")
                    {
                        if (e.CellRowColumnIndex.ColumnIndex > 1 && e.CellRowColumnIndex.ColumnIndex < this.AssociatedObject.InternalGrid.Columns.Count - CoveredRange)
                        {
                            e.Range = new CoveredCellInfo(e.CellRowColumnIndex.RowIndex, 1, e.CellRowColumnIndex.RowIndex, this.AssociatedObject.InternalGrid.Columns.Count - CoveredRange);
                            e.Handled = true;
                        }
                    }
                }
                else if (CurrentNod.Item is ITreeNode)
                {
                    if((CurrentNod.Item as ITreeNode).IsGroup)
                    {
                        if (e.CellRowColumnIndex.ColumnIndex > 1 && e.CellRowColumnIndex.ColumnIndex < this.AssociatedObject.InternalGrid.Columns.Count - CoveredRange)
                        {
                            e.Range = new CoveredCellInfo(e.CellRowColumnIndex.RowIndex, 1, e.CellRowColumnIndex.RowIndex, this.AssociatedObject.InternalGrid.Columns.Count - CoveredRange);
                            e.Handled = true;
                        }
                    }
                }
            }
        }
    }

    //public class RowValidationBehaviour : Behavior<Syncfusion.UI.Xaml.Grid.SfDataGrid>
    //{
    //    protected override void OnAttached()
    //    {
    //        this.AssociatedObject.RowValidating += OnRowValidating;
    //    }

    //    void OnRowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs args)
    //    {
    //        //if (args.RowData != null && string.IsNullOrEmpty((args.RowData as OrderInfo).CustomerID))
    //        //{
    //        //    args.ErrorMessages.Add("CustomerID", "Customer ID field should not be null or empty");
    //        //    args.IsValid = false;
    //        //}

    //        if (sender is Syncfusion.UI.Xaml.Grid.SfDataGrid)
    //        {
    //            IEnumerable<MartixCollection> m = (IEnumerable<MartixCollection>)(sender as Syncfusion.UI.Xaml.Grid.SfDataGrid).ItemsSource;
    //            if (args.RowData != null && m.Any(x => x.DESCRIPTION == (args.RowData as MartixCollection).DESCRIPTION))
    //            {
    //                //args.ErrorMessages.Add("CustomerID", "Customer ID field should not be null or empty");
    //                // args.IsValid = false;
    //                (args.RowData as MartixCollection).IsinRow = true;


    //            }
    //        }
    //    }

    //    protected override void OnDetaching()
    //    {
    //        this.AssociatedObject.RowValidating -= OnRowValidating;
    //    }
    //}
    
}
