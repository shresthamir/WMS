﻿using GTransfer.Library;
using GTransfer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Windows;
using Syncfusion.UI.Xaml.Grid;
using GTransfer.UserInterfaces;

namespace GTransfer.Reports
{
    class GoodsReceivedSummaryVM : BaseViewModel
    {
        private SfDataGrid Report;
        private DateTime _FDate;
        private DateTime _TDate;
        private IEnumerable<GoodsReceivedSummaryModel> _ReportList;
        private ObservableCollection<GoodsReceivedSummaryModel> _ReportDataList;
        private string _ShipmentNo;
        private int displayMode = 1;

        public ObservableCollection<GoodsReceivedSummaryModel> ReportDataList { get { return _ReportDataList; } set { _ReportDataList = value; OnPropertyChanged("ReportDataList"); } }

        public string ShipmentNo { get { return _ShipmentNo; } set { _ShipmentNo = value; OnPropertyChanged("ShipmentNo"); } }
        public DateTime FDate { get { return _FDate; } set { _FDate = value; OnPropertyChanged("FDate"); } }
        public DateTime TDate { get { return _TDate; } set { _TDate = value; OnPropertyChanged("TDate"); } }
        public bool ShowAll { get { return displayMode == 1; } set { if (value) SetDisplayMode(1); } }
        public bool ShowVariant { get { return displayMode == 2; } set { if (value) SetDisplayMode(2); } }
        public bool ShowNonVariant { get { return displayMode == 3; } set { if (value) SetDisplayMode(3); } }


        void SetDisplayMode(int mode)
        {
            displayMode = mode;
            if (_ReportList != null)
            {
                switch (displayMode)
                {
                    case 1:
                        ReportDataList = new ObservableCollection<GoodsReceivedSummaryModel>(_ReportList);
                        break;
                    case 2:
                        ReportDataList = new ObservableCollection<GoodsReceivedSummaryModel>(_ReportList.Where(x => x.Variance != 0));
                        break;
                    case 3:
                        ReportDataList = new ObservableCollection<GoodsReceivedSummaryModel>(_ReportList.Where(x => x.Variance == 0));
                        break;
                }
            }
            OnPropertyChanged("ShowAll");
            OnPropertyChanged("ShowVariant");
            OnPropertyChanged("ShowNonVariant");
        }

        public GoodsReceivedSummaryVM(SfDataGrid _Report)
        {
            this.Report = _Report;
            FDate = TDate = DateTime.Today;
        }

        public override void LoadMethod(object obj)
        {
            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                _ReportList = con.Query<GoodsReceivedSummaryModel>(@"SELECT B.OrderNo, B.MCODE, MI.MENUCODE, MI.DESCA, B.UNIT, TP.Warehouse, ISNULL(OP.QUANTITY, 0) OrderQty, ISNULL(SUM(TP.REALQTY_IN), 0) ReceivedQty, ISNULL(SUM(TP.REALQTY_IN), 0) - ISNULL(OP.QUANTITY, 0) Variance
FROM
(
    SELECT DISTINCT * FROM
    (
        SELECT VCHRNO OrderNo, MCODE, UNIT FROM RMD_ORDERPROD" + (string.IsNullOrEmpty(ShipmentNo)?string.Empty:" WHERE VCHRNO = @ShipmentNo" ) + @"
        UNION ALL
        SELECT REFORDBILL, PD.MCODE, PD.UNIT FROM RMD_TRNMAIN M         
        JOIN RMD_TRNPROD PD ON M.VCHRNO = PD.VCHRNO AND M.DIVISION = PD.DIVISION 
        JOIN RMD_ORDERPROD OP ON M.REFORDBILL = OP.VCHRNO AND M.DIVISION = op.DIVISION
        WHERE " + (string.IsNullOrEmpty(ShipmentNo) ? string.Empty : " M.VCHRNO = @ShipmentNo AND") + @" M.TRNDATE BETWEEN @FDate AND @TDate
    ) A
) B 
JOIN MENUITEM MI ON MI.MCODE = B.MCODE
LEFT JOIN RMD_ORDERPROD OP ON OP.VCHRNO = B.OrderNo AND OP.MCODE = B.MCODE AND OP.UNIT = B.UNIT
LEFT JOIN (RMD_TRNMAIN TM  JOIN RMD_TRNPROD TP ON TM.VCHRNO = TP.VCHRNO AND TM.DIVISION = TP.DIVISION)
ON B.OrderNo = TM.REFORDBILL AND TP.MCODE = B.MCODE AND TP.UNIT = B.UNIT
GROUP BY B.OrderNo, B.MCODE, B.UNIT, TP.Warehouse, OP.QUANTITY, MI.MENUCODE, MI.DESCA
", this);
                if (_ReportList != null)
                {
                    SetDisplayMode(displayMode);
                }
            }
        }

        #region PrintExport
        public override void ExecuteExport(object obj)
        {
            GlobalClass.ReportName = "Goods Received Summary";

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

            GlobalClass.ReportName = "Goods Received Summary";
            GlobalClass.ReportParams = string.Format("From Date : {0} To {1}", FDate.ToString("MM/dd/yyyy"), TDate.ToString("MM/dd/yyyy"));

            Report.PrintSettings.PrintPageMargin = new Thickness(30);
            Report.PrintSettings.AllowColumnWidthFitToPrintPage = false;
            Report.PrintSettings.PrintPageOrientation = PrintOrientation.Landscape;
            Report.ShowPrintPreview();
        }

        public override void ExecutePrint(object obj)
        {

            GlobalClass.ReportName = "Goods Received Summary";
            GlobalClass.ReportParams = string.Format("From Date : {0} To {1}", FDate.ToString("MM/dd/yyyy"), TDate.ToString("MM/dd/yyyy"));

            Report.PrintSettings.PrintPageMargin = new Thickness(30);
            Report.PrintSettings.AllowColumnWidthFitToPrintPage = false;
            Report.PrintSettings.PrintPageOrientation = PrintOrientation.Landscape;
            Report.Print();
        }
        #endregion
    }

    class GoodsReceivedSummaryModel
    {
        public string OrderNo { get; set; }
        public string MCODE { get; set; }
        public string Date { get; set; }
        public string MENUCODE { get; set; }
        public string DESCA { get; set; }
        public string UNIT { get; set; }
        public string Warehouse { get; set; }
        public string LocationCode { get; set; }
        public decimal OrderQty { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal Variance { get; set; }
    }
}
