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
        private ObservableCollection<CategoryVsDevice> _CategoryVsDeviceList;
        private ObservableCollection<dynamic> _DeviceList;
        private SfDataGrid Report;

        public ObservableCollection<dynamic> DeviceList
        {
            get
            {
                using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    return new ObservableCollection<dynamic>(con.Query("SELECT DeviceId,DeviceName FROM tblDevices"));
                }
            }
            set { value = _DeviceList; OnPropertyChanged("DeviceList"); }
        }

        
        public RelayCommand LoadpickingListCommand { get { return new RelayCommand(ExecuteLoadpickingListCommand); } }
        public RelayCommand SavePickingCommand { get { return new RelayCommand(ExecuteSavePickingCommand); } }
        public RelayCommand CancelCommand { get { return new RelayCommand(ExecuteCancelCommand); } }
        public RelayCommand GeneratePickingListCommand { get { return new RelayCommand(ExecuteGeneratePickingListCommand); } }        
        public RelayCommand AssignCommand { get { return new RelayCommand(ExecuteAssignCommand); } }



        public ObservableCollection<CategoryVsDevice> CategoryVsDeviceList { get { return _CategoryVsDeviceList; } set { _CategoryVsDeviceList = value; OnPropertyChanged("CategoryVsDeviceList"); } }
        public bool IsloadMode { get { return _IsloadMode; } set { _IsloadMode = value; OnPropertyChanged("IsloadMode"); } }
        public int ReqId { get { return _ReqId; } set { _ReqId = value; PickingList = null; OnPropertyChanged("ReqId"); } }

        public ObservableCollection<PickingList> PickingList { get { return _PickingList; } set { _PickingList = value; OnPropertyChanged("PickingList"); } }

        public pickingListViewModel(SfDataGrid _Report)
        {
            Report = _Report;
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
                    if (con.ExecuteScalar<int>("SELECT COUNT(*) FROM tblPickingList WHERE ReqId = " + ReqId) != 0)
                    {
                        MessageBox.Show("Picking List already generated");
                        return;
                    }
                    var result = con.Query<PickingList>(@"SELECT RD.Bcode, RD.MCODE, MI.DESCA, MI.MENUCODE, RD.UNIT, RD.ApprovedQty ReqQty, L.LocationCode, ISNULL(LB.Balance,0) Balance,L.LocationId,ISNULL(MG.DESCA, 'N/A') MCAT, LB.Warehouse  FROM TBL_REQUISITION_DETAILS RD 
                                                          JOIN MENUITEM MI ON RD.MCODE = MI.MCODE
                                                          JOIN MENUITEM MG ON MI.MGROUP = MG.MCODE
                                                          LEFT JOIN vwLocationStockBalance LB ON LB.MCODE = RD.MCODE AND LB.UNIT = RD.UNIT
                                                          LEFT JOIN TBL_LOCATIONS L ON LB.LocationId = L.LocationId
                                                          WHERE RD.ReqId = " + ReqId + @" AND LB.WAREHOUSE = '" + GlobalClass.Warehouse + @"'
                                                          ORDER BY LocationCode, MCODE, UNIT");
                    if (result == null || result.Count() <= 0)
                    {
                        MessageBox.Show("Sorry No Picking List Found For Given Id");
                    }
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
                        CategoryVsDeviceList = new ObservableCollection<CategoryVsDevice>();
                        foreach (var item in PickingList.Select(i => i.MCAT).Distinct().ToList())
                        {
                            CategoryVsDeviceList.Add(new CategoryVsDevice() { MCAT = item });
                        }
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
            try
            {
                using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    var pList = con.Query<PickingList>(@"SELECT TP.ReqId, TP.Mcode, TP.Unit, TP.LocationId, TP.Quantity, TP.Bcode, MI.MENUCODE, MI.DESCA, 
TL.LocationCode, TP.DeviceId, TD.DeviceName, MG.DESCA MCAT FROM tblPickingList TP 
LEFT JOIN MenuItem MI ON TP.Mcode = MI.MCODE 
LEFT JOIN MenuItem MG ON MI.MGROUP = MG.MCODE
LEFT JOIN TBL_LOCATIONS TL ON TL.LocationId = TP.LocationId 
LEFT JOIN tblDevices TD ON TP.DeviceId = TD.DeviceId 
WHERE  ReqId=" + ReqId + " and [Status] = 0 ORDER BY TL.LocationCode");
                    PickingList = new ObservableCollection<Models.PickingList>(pList);
                    IsloadMode = true;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void ExecuteAssignCommand(object obj)
        {
            if (PickingList == null || CategoryVsDeviceList == null) return;
            foreach (var i in PickingList)
            {
                foreach (var j in CategoryVsDeviceList)
                {
                    if (i.MCAT == j.MCAT)
                    {
                        if (j.Device != null)
                        {
                            i.DeviceId = j.Device.DeviceId;
                            i.DeviceName = j.Device.DeviceName;
                        }
                    }
                }
            }
        }

        private void ExecuteSavePickingCommand(object obj)
        {
            try
            {
                if (PickingList == null || PickingList.Count <= 0) { return; }
                var pCheck = PickingList.Where(p => p.DeviceId == null || p.DeviceId == "").ToList();
                if (pCheck == null || pCheck.Count() > 0) { MessageBox.Show("Please Assign a Device for items"); return; }
                using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    con.Open();
                    var saveList = new ObservableCollection<PickingList>();
                    foreach (var i in PickingList)
                    {
                        if (!string.IsNullOrEmpty(i.LocationId))
                        {
                            saveList.Add(i);
                        }
                    }
                    if (saveList.Count <= 0)
                    {
                        MessageBox.Show("Nothing to Save.Please provide valid List");
                        return;
                    }
                    con.Execute("INSERT INTO tblPickingList (ReqId,Mcode,Unit,LocationId,Quantity,Status,Bcode,DeviceId, Warehouse) values(@ReqId,@Mcode,@Unit,@LocationId,@Quantity,@Status,@Bcode,@DeviceId, @Warehouse)", saveList);
                    if (MessageBox.Show("Picking List Saved Successfully.Do U Want To Print", "Print Data", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        ExecutePrint(null);
                    }
                    privateUndo();
                }
            }
            catch (Exception ex)
            {
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

        #region PrintExport
        public override void ExecuteExport(object obj)
        {
            GlobalClass.ReportName = "Item Picking List";

            GlobalClass.ReportParams = "";// string.Format("From Date : {0} To {1}", FDate.ToString("MM/dd/yyyy"), TDate.ToString("MM/dd/yyyy"));

            wExportFormat ef = new wExportFormat(Report);
            ef.ShowDialog();
        }
        protected override bool CanExecutePrint(object obj)

        {
            return PickingList != null && PickingList.Count > 0;
        }

        protected override bool CanExecuteExport(object obj)
        {
            return PickingList != null && PickingList.Count > 0;
        }
        protected override bool CanExecutePreview(object obj)
        {
            return PickingList != null && PickingList.Count > 0;
        }


        public override void ExecutePreview(object obj)
        {

            GlobalClass.ReportName = "Item Picking List";
            GlobalClass.ReportParams = "";// string.Format("From Date : {0} To {1}", FDate.ToString("MM/dd/yyyy"), TDate.ToString("MM/dd/yyyy"));

            Report.PrintSettings.PrintPageMargin = new Thickness(30);
            Report.PrintSettings.AllowColumnWidthFitToPrintPage = false;
            Report.PrintSettings.PrintPageOrientation = PrintOrientation.Portrait;
            Report.Columns["DeviceName"].IsHidden = true;
            Report.ShowPrintPreview();
        }

        public override void ExecutePrint(object obj)
        {

            GlobalClass.ReportName = "Item Picking List";
            GlobalClass.ReportParams = "";// string.Format("From Date : {0} To {1}", FDate.ToString("MM/dd/yyyy"), TDate.ToString("MM/dd/yyyy"));

            Report.PrintSettings.PrintPageMargin = new Thickness(30);
            Report.PrintSettings.AllowColumnWidthFitToPrintPage = false;
            Report.PrintSettings.PrintPageOrientation = PrintOrientation.Portrait;
            Report.Columns["DeviceName"].IsHidden = true;
            Report.Print();
        }
        #endregion

    }

    
    public class CategoryVsDevice : BaseModel
    {
        private string _MCAT;
        private dynamic _Device;
        public dynamic Device { get { return _Device; } set { _Device = value; OnPropertyChanged("Device"); } }
        public string MCAT { get { return _MCAT; } set { _MCAT = value; OnPropertyChanged("MCAT"); } }
    }
}
