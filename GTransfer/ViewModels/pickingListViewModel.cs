using GTransfer.Library;
using GTransfer.Models;
using GTransfer.UserInterfaces;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Dapper;
using System.Collections.ObjectModel;

namespace GTransfer.ViewModels
{
    class pickingListViewModel : BaseViewModel
    {
        private PickingList _PLObj;
        public PickingList PLObj { get { return _PLObj; } set { _PLObj = value; OnPropertyChanged("PLObj"); } }
        private ObservableCollection<PickingList> _PickingList;
        private bool _IsloadMode;
        private int _ReqId;


        public RelayCommand PrintPreviewCommand { get { return new RelayCommand(ExecutePrintCommand); } }
        public RelayCommand LoadpickingListCommand { get { return new RelayCommand(ExecuteLoadpickingListCommand); } }
        public RelayCommand SavePickingCommand { get { return new RelayCommand(ExecuteSavePickingCommand); } }
        public RelayCommand CancelCommand { get { return new RelayCommand(ExecuteCancelCommand); } }
        public RelayCommand GeneratePickingListCommand { get { return new RelayCommand(ExecuteGeneratePickingListCommand); } }
        public RelayCommand PrintCommand { get { return new RelayCommand(Print); } }

       
        public bool IsloadMode { get { return _IsloadMode; }set { _IsloadMode = value;OnPropertyChanged("IsloadMode"); } }
        public int ReqId { get { return _ReqId; } set { _ReqId = value; PickingList = null;  OnPropertyChanged("ReqId"); } }

        public ObservableCollection<PickingList> PickingList { get{return _PickingList; } set{_PickingList = value; OnPropertyChanged("PickingList"); } }

        public pickingListViewModel() {
            IsloadMode = false;
        }

        private void ExecuteGeneratePickingListCommand(object obj)
        {

            try
            {
                if (ReqId <= 0) { return; }
                using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    con.Open();
                    var result = con.Query<PickingList>(@"SELECT RD.Bcode, RD.MCODE, MI.DESCA, MI.MENUCODE, RD.UNIT, RD.ApprovedQty ReqQty, L.LocationCode, ISNULL(LB.Balance,0) Balance,L.LocationId  FROM TBL_REQUISITION_DETAILS RD 
                                                          JOIN MENUITEM MI ON RD.MCODE = MI.MCODE
                                                          LEFT JOIN vwLocationStockBalance LB ON LB.MCODE = RD.MCODE AND LB.UNIT = RD.UNIT
                                                          LEFT JOIN TBL_LOCATIONS L ON LB.LocationId = L.LocationId
                                                          WHERE RD.ReqId = " + ReqId + @"
                                                          ORDER BY LocationCode, MCODE, UNIT");
                    if (result == null || result.Count()<=0) { MessageBox.Show("Sorry No Picking List Found For Given Id"); }
                    else
                    {
                        // PickingList = new ObservableCollection<Models.PickingList>();

                        var tempInValidList = new ObservableCollection<Models.PickingList>();
                        var tempValidList = new ObservableCollection<Models.PickingList>();
                        var Plist = new ObservableCollection<PickingList>(result);
                        foreach (var P in Plist)
                        {
                            P.ReqId = ReqId;
                            if (!string.IsNullOrEmpty(P.LocationCode))
                            {
                                var alreadyAddItems = tempValidList.Where(a => a.Mcode == P.Mcode);
                                decimal bookedQty = 0;
                                if (alreadyAddItems != null)
                                {
                                    bookedQty = alreadyAddItems.Sum(x => x.Quantity);
                                }
                                decimal Q = P.Balance - (P.ReqQty - bookedQty);
                                if (Q <= 0)
                                {
                                    Q = P.Balance;
                                }
                                else if (Q > 0) { Q = P.ReqQty - bookedQty; }
                                P.Quantity = Q;
                                tempValidList.Add(P);
                            }
                            else
                            {
                                tempInValidList.Add(P);
                            }
                        }
                        PickingList = new ObservableCollection<Models.PickingList>(tempValidList.Where(x => x.Quantity > 0));
                        if (PickingList.Count() <= 0) { MessageBox.Show("Sorry error occure.Either item doesn't contain valid location or location stock is empty"); }
                        IsloadMode = false;
                        //foreach (var i in tempInValidList)
                        //{
                        //    PickingList.Add(i);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void ExecuteLoadpickingListCommand(object obj)
        {
            try {
                using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString)) {
                    PickingList = new ObservableCollection<Models.PickingList>(con.Query<PickingList>("select tp.ReqId,tp.Mcode,tp.Unit,tp.LocationId,tp.Quantity,tp.Bcode,mi.MENUCODE,mi.DESCA,tl.LocationCode from tblPickingList tp left join menuitem mi on tp.Mcode=mi.MCODE left join TBL_LOCATIONS tl on tl.LocationId=tp.LocationId where  ReqId=" + ReqId+ " and Status=0 order by tl.LocationCode"));
                    IsloadMode = true;
                }
            } catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ExecuteSavePickingCommand(object obj)
        {
            try
            {
                if (PickingList == null || PickingList.Count <= 0) { return; }
                using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    con.Open();
                    var saveList = new ObservableCollection<PickingList>();
                    foreach (var i in PickingList) {
                        if (!string.IsNullOrEmpty(i.LocationId))
                        {
                            saveList.Add(i);
                        }
                    }
                    if (saveList.Count <= 0) { MessageBox.Show("Nothing to Save.Please provide valid List"); return; }
                    con.Execute("INSERT INTO tblPickingList (ReqId,Mcode,Unit,LocationId,Quantity,Status,Bcode) values(@ReqId,@Mcode,@Unit,@LocationId,@Quantity,@Status,@Bcode)", saveList);
                    if (MessageBox.Show("Picking List Saved Successfully.Do U Want To Print", "Print Data", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
                        ExecutePrintCommand(obj);
                    }
                        privateUndo();
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void privateUndo()
        {
            PickingList = null;
            ReqId = 0;
            IsloadMode = false;
        }
        private void ExecuteCancelCommand(object obj)
        {
            privateUndo();
        }

        private void ExecutePrintCommand(object obj)
        {
            SfDataGrid grid = obj as SfDataGrid;
            new PreviewWindow()
            {
                PrintPreviewArea =
                {
                    PrintManagerBase = ((PrintManagerBase)new CustomPrintManager(grid))
                }
            }.ShowDialog();
        }
        private void Print(object obj)
        {
            SfDataGrid grid = obj as SfDataGrid;
            new PreviewWindow()
            {
                PrintPreviewArea =
                {
                    PrintManagerBase = ((PrintManagerBase)new CustomPrintManager(grid))
                }
            }.PrintPreviewArea.PrintCommand.Execute((object)null);
        }
    }

    public class CustomPrintManager : GridPrintManager
    {
        SfDataGrid sfgrid = new SfDataGrid();

        public CustomPrintManager(SfDataGrid grid)
            : base(grid)
        {
            sfgrid = grid;

         //   grid.PrintSettings.PrintPageHeaderHeight = 70;
           // grid.PrintSettings.PrintPageHeaderTemplate = Application.Current.Resources["PrintHeaderTemplate"] as DataTemplate;
            //grid.PrintSettings.PrintPageFooterHeight = Double.NaN;
            //grid.PrintSettings.PrintPageFooterTemplate = Application.Current.Resources["PrintFooterTemplate"] as DataTemplate;



        }

        protected override double GetColumnWidth(string mappingName)
        {
            if (mappingName == "Bcode") { return 100; }
            else if (mappingName == "MENUCODE") { return 100; }
            else if (mappingName == "DESCA") { return 200; }
            else if (mappingName == "Unit") { return 80; }
            else if (mappingName == "Quantity") { return 80; }
            else if (mappingName == "LocationCode") { return 80; }
            else { return 100; }

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
          //  return sfgrid.Columns.FirstOrDefault(x => x.MappingName == mappingName).Width;

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
