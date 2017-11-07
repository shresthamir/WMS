using GTransfer.Library;
using GTransfer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Syncfusion.UI.Xaml.Grid;
using System.Windows;
using GTransfer.UserInterfaces;

namespace GTransfer.Reports
{
    class GoodsReceivedDetailVM : BaseViewModel
    {
        private SfDataGrid Report;
        private DateTime _FDate;
        private DateTime _TDate;
        private string _ShipmentNo;
        private ObservableCollection<GoodsReceivedDetailModel> _ReportDataList;
        private bool _LocationWise;

        public ObservableCollection<GoodsReceivedDetailModel> ReportDataList { get { return _ReportDataList; } set { _ReportDataList = value; OnPropertyChanged("ReportDataList"); } }

        public DateTime FDate { get { return _FDate; } set { _FDate = value; OnPropertyChanged("FDate"); } }
        public DateTime TDate { get { return _TDate; } set { _TDate = value; OnPropertyChanged("TDate"); } }
        public string ShipmentNo { get { return _ShipmentNo; } set { _ShipmentNo = value; OnPropertyChanged("ShipmentNo"); } }
        public bool LocationWise { get { return _LocationWise; } set { _LocationWise = value; OnPropertyChanged("LocationWise"); } }


        public GoodsReceivedDetailVM(SfDataGrid _Report)
        {
            this.Report = _Report;
            FDate = TDate = DateTime.Today;
        }

        public override void LoadMethod(object obj)
        {
            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                var result = con.Query<GoodsReceivedDetailModel>(@"SELECT CAST(RIGHT(M.VCHRNO,LEN(M.VCHRNO)-2) AS INT) VNUM, M.REFORDBILL, M.VCHRNO, CONVERT(VARCHAR, M.TRNDATE, 101) Date, I.MENUCODE, I.DESCA, PD.UNIT, PD.WAREHOUSE" + (LocationWise ? ", L.LocationCode" : string.Empty) + @", SUM(PD.InQty) InQty FROM RMD_TRNMAIN M 
JOIN RMD_TRNPROD_DETAIL PD ON M.VCHRNO = PD.VCHRNO AND M.DIVISION = PD.DIVISION AND M.PhiscalID = PD.PhiscalID
JOIN MENUITEM I ON PD.MCODE = I.MCODE
JOIN TBL_LOCATIONS L ON PD.LocationId = L.LocationId
WHERE LEFT(M.VCHRNO,2) IN ('PI') AND M.TRNDATE BETWEEN @FDate AND @TDate " + (string.IsNullOrEmpty(ShipmentNo) ? string.Empty : "AND M.REFORDBILL = @ShipmentNo")
+ @" GROUP BY M.VCHRNO, REFORDBILL, TRNDATE, MENUCODE, DESCA, UNIT, PD.WAREHOUSE" + (LocationWise ? ", L.LocationCode" : string.Empty)
+ @" ORDER BY VNUM, WAREHOUSE" + (LocationWise ? ", L.LocationCode" : string.Empty), this);
                if (result != null)
                {
                    ReportDataList = new ObservableCollection<GoodsReceivedDetailModel>(result);
                }
            }
        }

        #region PrintExport
        public override void ExecuteExport(object obj)
        {
            GlobalClass.ReportName = "Goods Received Detail";

            GlobalClass.ReportParams = string.Format("From Date : {0} To {1}", FDate.ToString("MM/dd/yyyy"), TDate.ToString("MM/dd/yyyy"));

            wExportFormat ef = new wExportFormat(Report);
            ef.ShowDialog();
        }
        protected override bool CanExecutePrint(object obj)

        {
            return ReportDataList != null && ReportDataList.Count > 0;
        }

        protected override bool CanExecuteExport(object obj)
        {
            return ReportDataList != null && ReportDataList.Count > 0;
        }
        protected override bool CanExecutePreview(object obj)
        {
            return ReportDataList != null && ReportDataList.Count > 0;
        }


        public override void ExecutePreview(object obj)
        {

            GlobalClass.ReportName = "Goods Received Detail";
            GlobalClass.ReportParams = string.Format("From Date : {0} To {1}", FDate.ToString("MM/dd/yyyy"), TDate.ToString("MM/dd/yyyy"));

            Report.PrintSettings.PrintPageMargin = new Thickness(30);
            Report.PrintSettings.AllowColumnWidthFitToPrintPage = false;
            Report.PrintSettings.PrintPageOrientation = PrintOrientation.Landscape;
            Report.ShowPrintPreview();
        }

        public override void ExecutePrint(object obj)
        {

            GlobalClass.ReportName = "Goods Received Detail";
            GlobalClass.ReportParams = string.Format("From Date : {0} To {1}", FDate.ToString("MM/dd/yyyy"), TDate.ToString("MM/dd/yyyy"));

            Report.PrintSettings.PrintPageMargin = new Thickness(30);
            Report.PrintSettings.AllowColumnWidthFitToPrintPage = false;
            Report.PrintSettings.PrintPageOrientation = PrintOrientation.Landscape;
            Report.Print();
        }
        #endregion
    }

    class GoodsReceivedDetailModel
    {
        public string REFORDBILL { get; set; }
        public string VCHRNO { get; set; }
        public string Date { get; set; }
        public string MENUCODE { get; set; }
        public string DESCA { get; set; }
        public string UNIT { get; set; }
        public string WAREHOUSE { get; set; }
        public string LocationCode { get; set; }
        public decimal InQty { get; set; }
    }
}
