using GTransfer.Library;
using GTransfer.Models;
using GTransfer.UserInterfaces;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GTransfer.ViewModels
{
    class pickingListViewModel:BaseViewModel
    {
        private PickingList _PLObj;
        public PickingList PLObj { get {return _PLObj; }set { _PLObj = value;OnPropertyChanged("PLObj"); } }

        public RelayCommand PrintCommand { get { return new RelayCommand(ExecutePrintCommand); } }

        public pickingListViewModel() { }

        private void ExecutePrintCommand(object obj)
        {
            SfDataGrid grid = obj as SfDataGrid;
            new PreviewWindow()
            {
                PrintPreviewArea =
                {
                    PrintManagerBase = ((PrintManagerBase)new CustomPrintManager(this.ReportName, grid))
                }
            }.ShowDialog();
        }
    }
    public class CustomPrintManager : GridPrintManager
    {
        SfDataGrid sfgrid = new SfDataGrid();

        public CustomPrintManager(string ReportName, SfDataGrid grid)
            : base(grid)
        {
            sfgrid = grid;

            grid.PrintSettings.PrintPageHeaderHeight = Double.NaN;
            grid.PrintSettings.PrintPageHeaderTemplate = Application.Current.Resources["PrintHeaderTemplate"] as DataTemplate;
            grid.PrintSettings.PrintPageFooterHeight = Double.NaN;
            grid.PrintSettings.PrintPageFooterTemplate = Application.Current.Resources["PrintFooterTemplate"] as DataTemplate;



        }

        protected override double GetColumnWidth(string mappingName)
        {

            //if (ReportFields.ReportObj != null && ReportFields.ReportObj.ReportFormatCollection != null)
            //{
            //    var width = ReportFields.ReportObj.ReportFormatCollection.FirstOrDefault(x => x.MappingName == mappingName).Size;
            //    if (width > 0) { return width; }
            //    else
            //    {
            //        return sfgrid.Columns.FirstOrDefault(x => x.MappingName == mappingName).Width;
            //    }
            //}
            //else
                return sfgrid.Columns.FirstOrDefault(x => x.MappingName == mappingName).Width;

        }

        protected override FormattedText GetFormattedText(PrintManagerBase.RowInfo rowInfo, PrintManagerBase.CellInfo cellInfo, string cellValue)
        {

            FormattedText formattedText = base.GetFormattedText(rowInfo, cellInfo, cellValue);
            //if (rowInfo.Record != null)
            //{
            //    formattedText.SetFontWeight(PrintPreviewFontSetting.FontWeight);
            //}
            //else
            //{
                formattedText.SetFontWeight(FontWeights.Bold);
           // }
            formattedText.SetFontStyle(FontStyles.Normal);
            formattedText.SetFontSize(12);
            formattedText.SetFontFamily((FontFamily)new FontFamilyConverter().ConvertFromString("Segoe UI"));
            return formattedText;
        }
    }
}
