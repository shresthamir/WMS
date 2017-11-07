using GTransfer.Library;
using Microsoft.Win32;
using System.Drawing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using Syncfusion.Windows.PdfViewer;
using Syncfusion.Pdf;

namespace GTransfer.UserInterfaces
{
    /// <summary>
    /// Interaction logic for wExportFormat.xaml
    /// </summary>
    public partial class wExportFormat : Window
    {
        SfDataGrid Report;
        public int ExportFormat { get { return cmbFormat.SelectedIndex; } }

        public wExportFormat(SfDataGrid _report)
        {
            InitializeComponent();
            Report = _report;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Content.ToString() == "Ok")
            {
                switch (ExportFormat)
                {
                    case 0:
                        new ExportToExcelHelper(Report).ExportToExcel();
                        break;
                    case 1:
                        new ExportToPdfHelper().ExportToPdf(Report);
                        break;
                    case 2:
                        new ExportToExcelHelper(Report).ExportToXML();
                        break;
                    case 3:
                        new ExportToExcelHelper(Report).ExportToCSV();
                        break;
                }
            }
            this.Close();
        }
    }

    class ExportToExcelHelper
    {
        SfDataGrid dataGrid;
        public ExportToExcelHelper(SfDataGrid _dataGrid)
        {
            dataGrid = _dataGrid;
        }
        public void ExportToExcel()
        {
            var workbook = GetExcelWorkBook();

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Excel Files(*.xlsx)|*.xlsx"
            };

            if (sfd.ShowDialog() == true)
            {
                workbook.SaveAs(sfd.FileName);

                //Message box confirmation to view the created Pdf file.
                if (MessageBox.Show("Do you want to view the Excel file?", "Excel file has been created",
                                    MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    //Launching the Pdf file using the default Application.
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
        }
        public IWorkbook GetExcelWorkBook()
        {
            if (dataGrid == null)
                return null;
            try
            {
                var options = new ExcelExportingOptions();
                options.ExportStackedHeaders = true;
                options.ExportingEventHandler = ExportingHandler;
                options.CellsExportingEventHandler = CellExportingHandler;
                options.ExcelVersion = ExcelVersion.Excel2007;
                var document = dataGrid.ExportToExcel(dataGrid.View, options);
                var workbook = document.Excel.Workbooks[0];

                var workSheet = workbook.Worksheets[0];

                workSheet.InsertRow(1, 5, ExcelInsertOptions.FormatDefault);

                for (int i = 0; i < 5; i++)
                {
                    workSheet.Rows[i].Merge();
                    var cell = workSheet.Range["A" + (i + 1).ToString()];

                    IStyle style = cell.CellStyle;
                    style.HorizontalAlignment = ExcelHAlign.HAlignCenter;

                    IFont font = style.Font;
                    font.FontName = "Segoe UI";
                    switch (i)
                    {
                        case 0:
                            cell.Value = GlobalClass.company.NAME;
                            font.Size = 16;
                            font.Bold = true;
                            break;
                        case 1:
                            cell.Value = GlobalClass.company.ADDRESS;
                            font.Size = 12;
                            break;
                        case 2:
                            cell.Value = GlobalClass.company.VAT;
                            font.Size = 12;
                            break;
                        case 3:
                            cell.Value = GlobalClass.ReportName;
                            font.Size = 14;
                            font.Bold = true;
                            break;
                        case 4:
                            cell.Value = GlobalClass.ReportParams;
                            font.Size = 12;
                            break;
                    }
                }
                return workbook;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void ExportToCSV()
        {
            var workbook = GetExcelWorkBook();

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV Files(*.csv)|*.csv"
            };

            if (sfd.ShowDialog() == true)
            {
                workbook.SaveAs(sfd.FileName, ",");

                //Message box confirmation to view the created Pdf file.
                if (MessageBox.Show("Do you want to view the Excel file?", "Excel file has been created",
                                    MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    //Launching the Pdf file using the default Application.
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
        }

        public void ExportToXML()
        {
            var workbook = GetExcelWorkBook();

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "XML Files(*.xml)|*.xml"
            };

            if (sfd.ShowDialog() == true)
            {
                workbook.SaveAsXml(sfd.FileName, ExcelXmlSaveType.MSExcel);

                //Message box confirmation to view the created Pdf file.
                if (MessageBox.Show("Do you want to view the Excel file?", "Excel file has been created",
                                    MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    //Launching the Pdf file using the default Application.
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
        }

        public void ExportToHTML()
        {
            var workbook = GetExcelWorkBook();

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "HTML Files(*.html)|*.html"
            };

            if (sfd.ShowDialog() == true)
            {

                workbook.SaveAsHtml(sfd.FileName, Syncfusion.XlsIO.Implementation.HtmlSaveOptions.Default);

                //Message box confirmation to view the created Pdf file.
                if (MessageBox.Show("Do you want to view the Excel file?", "Excel file has been created",
                                    MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    //Launching the Pdf file using the default Application.
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
        }

        private static void CellExportingHandler(object sender, GridCellExcelExportingEventArgs e)
        {
            //e.Range.CellStyle.Font.Size = 12;
            //e.Range.CellStyle.Font.FontName = "Segoe UI";

            //if (e.ColumnName == "UnitPrice" || e.ColumnName == "UnitsInStock")
            //{
            //    double value = 0;
            //    if (double.TryParse(e.CellValue.ToString(), out value))
            //    {
            //        e.Range.Number = value;
            //        e.Handled = true;
            //    }
            //}
        }

        private static void ExportingHandler(object sender, GridExcelExportingEventArgs e)
        {

            if (e.CellType == ExportCellType.HeaderCell)
            {
                e.CellStyle.FontInfo.Bold = true;
            }
            e.CellStyle.FontInfo.Size = 12;
            e.CellStyle.FontInfo.FontName = "Segoe UI";
        }


    }

    class ExportToPdfHelper
    {
        static PdfGridCellStyle cellstyle = new PdfGridCellStyle();
        public ExportToPdfHelper()
        {
            cellstyle = new PdfGridCellStyle();
            cellstyle.StringFormat = new PdfStringFormat() { Alignment = PdfTextAlignment.Right };
            var font = new Font("Segoe UI", 9f, System.Drawing.FontStyle.Regular);
            cellstyle.Font = new PdfTrueTypeFont(font, true);
            cellstyle.Borders.All = new PdfPen(PdfBrushes.DarkGray, 0.2f);
            //CommandManager.RegisterClassCommandBinding(typeof(SfDataGrid), new CommandBinding(ExportToPdf, OnExecuteExportToPdf, OnCanExecuteExportToExcel));
        }

        public void ExportToPdf(SfDataGrid dataGrid)
        {
            if (dataGrid == null) return;
            try
            {
                var options = new PdfExportingOptions();
                options.CellsExportingEventHandler = GridCellPdfExportingEventhandler;
                options.ExportingEventHandler = GridPdfExportingEventhandler;
                options.PageHeaderFooterEventHandler = PdfHeaderFooterEventHandler;
                options.ExportStackedHeaders = true;
                var document = dataGrid.ExportToPdf(options);
                MemoryStream stream = new MemoryStream();
                document.Save(stream);
                PdfViewerControl pdfViewer = new PdfViewerControl();
                pdfViewer.Load(stream);
                Window window = new Window();
                window.Content = pdfViewer;
                window.Show();

                //var document = new PdfDocument();
                //document.PageSettings.Orientation = PdfPageOrientation.Landscape;
                //document.PageSettings.SetMargins(20);                
                //var page = document.Pages.Add();
                //var pdfGrid = dataGrid.ExportToPdfGrid(dataGrid.View, options);

                //var format = new PdfGridLayoutFormat()
                //{
                //    Layout = PdfLayoutType.Paginate,
                //    Break = PdfLayoutBreakType.FitPage                    
                //};

                //pdfGrid.Draw(page, new PointF(), format);


                //SaveFileDialog sfd = new SaveFileDialog
                //{
                //    Filter = "PDF Files(*.pdf)|*.pdf"
                //};

                //if (sfd.ShowDialog() == true)
                //{
                //    using (Stream stream = sfd.OpenFile())
                //    {
                //        document.Save(stream);
                //    }

                //    //Message box confirmation to view the created Pdf file.
                //    if (MessageBox.Show("Do you want to view the Pdf file?", "Pdf file has been created",
                //                        MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                //    {
                //        //Launching the Pdf file using the default Application.
                //        System.Diagnostics.Process.Start(sfd.FileName);
                //    }
                //}
            }
            catch (Exception)
            {

            }
        }



        #region ExportToPdf Event Handlers

        static void GridPdfExportingEventhandler(object sender, GridPdfExportingEventArgs e)
        {
            if (e.CellType == ExportCellType.HeaderCell)
            {
                e.CellStyle.BackgroundBrush = PdfBrushes.LightSteelBlue;
            }
            else if (e.CellType == ExportCellType.GroupCaptionCell)
            {
                e.CellStyle.BackgroundBrush = PdfBrushes.LightGray;
            }
            else if (e.CellType == ExportCellType.GroupSummaryCell)
            {
                e.CellStyle.BackgroundBrush = PdfBrushes.Azure;
            }
            else if (e.CellType == ExportCellType.TableSummaryCell)
            {
                e.CellStyle.BackgroundBrush = PdfBrushes.LightSlateGray;
                e.CellStyle.TextBrush = PdfBrushes.White;
            }
        }

        static void GridCellPdfExportingEventhandler(object sender, GridCellPdfExportingEventArgs e)
        {
            if ((e.ColumnName == "OrderID" || e.ColumnName == "EmployeeID" || e.ColumnName == "OrderDate" || e.ColumnName == "Freight")
                && e.CellType == ExportCellType.RecordCell)
            {
                e.PdfGridCell.Style = cellstyle;
            }
        }

        static void PdfHeaderFooterEventHandler(object sender, PdfHeaderFooterEventArgs e)
        {

            var width = e.PdfPage.GetClientSize().Width;

            PdfPageTemplateElement header = new PdfPageTemplateElement(width, 80);
            FormattedText Ft = new FormattedText(GlobalClass.company.NAME, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 14, System.Windows.Media.Brushes.Black);
            Ft.SetFontWeight(FontWeights.SemiBold);

            header.Graphics.DrawString(GlobalClass.company.NAME, new PdfTrueTypeFont(new Font("Tahoma", 14, System.Drawing.FontStyle.Bold)), PdfBrushes.Black, new PointF((width - (float)Ft.Width) / 2, 0));

            Ft = new FormattedText(GlobalClass.company.ADDRESS, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 11, System.Windows.Media.Brushes.Black);
            header.Graphics.DrawString(GlobalClass.company.ADDRESS, new PdfTrueTypeFont(new Font("Tahoma", 11)), PdfBrushes.Black, new PointF((width - (float)Ft.Width) / 2, 20));

            Ft = new FormattedText(GlobalClass.company.VAT, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 11, System.Windows.Media.Brushes.Black);
            header.Graphics.DrawString(GlobalClass.company.VAT, new PdfTrueTypeFont(new Font("Tahoma", 11)), PdfBrushes.Black, new PointF((width - (float)Ft.Width) / 2, 35));

            Ft = new FormattedText(GlobalClass.ReportName, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 12, System.Windows.Media.Brushes.Black);
            Ft.SetFontWeight(FontWeights.SemiBold);
            header.Graphics.DrawString(GlobalClass.ReportName, new PdfTrueTypeFont(new Font("Tahoma", 12, System.Drawing.FontStyle.Bold)), PdfBrushes.Black, new PointF((width - (float)Ft.Width) / 2, 50));

            Ft = new FormattedText(GlobalClass.ReportParams, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 11, System.Windows.Media.Brushes.Black);
            header.Graphics.DrawString(GlobalClass.ReportParams, new PdfTrueTypeFont(new Font("Tahoma", 11, System.Drawing.FontStyle.Bold)), PdfBrushes.Black, new PointF((width - (float)Ft.Width) / 2, 65));



            //header.Graphics.DrawImage(PdfImage.FromFile(@"C:\555.png"), 155, 5, width / 3f, 34);
            e.PdfDocumentTemplate.Top = header;

            //PdfPageTemplateElement footer = new PdfPageTemplateElement(width, 30);
            //footer.Graphics.DrawImage(PdfImage.FromFile(@"..\..\Resources\Footer.jpg"), 0, 0);
            //e.PdfDocumentTemplate.Bottom = footer;
        }

        #endregion

        private void DrawToImage(FrameworkElement element)
        {

            element.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            element.Arrange(new Rect(element.DesiredSize));

            System.Windows.Media.Imaging.RenderTargetBitmap bitmap = new System.Windows.Media.Imaging.RenderTargetBitmap((int)element.ActualWidth, (int)element.ActualHeight,
                                                               120.0, 120.0, System.Windows.Media.PixelFormats.Pbgra32);
            bitmap.Render(element);

            System.Windows.Media.Imaging.BitmapEncoder encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
            encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bitmap));

            using (Stream s = File.OpenWrite(@"C:\555.png"))
            {
                encoder.Save(s);
            }
        }

    }
}
