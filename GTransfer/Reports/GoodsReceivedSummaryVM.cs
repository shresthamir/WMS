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

namespace GTransfer.Reports
{
    class GoodsReceivedSummaryVM : BaseViewModel
    {
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

        public GoodsReceivedSummaryVM()
        {
            FDate = TDate = DateTime.Today;
        }

        public override void LoadMethod(object obj)
        {
            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                _ReportList = con.Query<GoodsReceivedSummaryModel>(@"SELECT B.OrderNo, B.MCODE, MI.MENUCODE, MI.DESCA, B.UNIT, TP.Warehouse, ISNULL(OP.QUANTITY, 0) OrderQty, ISNULL(SUM(TP.REALQTY_IN), 0) ReceivedQty, ISNULL(OP.QUANTITY, 0) - ISNULL(SUM(TP.REALQTY_IN), 0) Variance
FROM
(
    SELECT DISTINCT * FROM
    (
        SELECT VCHRNO OrderNo, MCODE, UNIT FROM RMD_ORDERPROD-- WHERE VCHRNO = 'OR5'
        UNION ALL
        SELECT REFORDBILL, PD.MCODE, PD.UNIT FROM RMD_TRNMAIN M         
        JOIN RMD_TRNPROD PD ON M.VCHRNO = PD.VCHRNO AND M.DIVISION = PD.DIVISION 
        JOIN RMD_ORDERPROD OP ON M.REFORDBILL = OP.VCHRNO AND M.DIVISION = op.DIVISION
        WHERE M.TRNDATE BETWEEN @FDate AND @TDate

    ) A
) B 
JOIN MENUITEM MI ON MI.MCODE = B.MCODE
LEFT JOIN RMD_ORDERPROD OP ON OP.VCHRNO = B.OrderNo AND OP.MCODE = B.MCODE AND OP.UNIT = B.UNIT
LEFT JOIN RMD_TRNMAIN TM ON B.OrderNo = TM.REFORDBILL
LEFT JOIN RMD_TRNPROD TP ON TM.VCHRNO = TP.VCHRNO AND TM.DIVISION = TP.DIVISION AND TP.MCODE = B.MCODE
GROUP BY B.OrderNo, B.MCODE, B.UNIT, OP.QUANTITY, TP.Warehouse, MI.MENUCODE, MI.DESCA", this);
                if (_ReportList != null)
                {
                    SetDisplayMode(displayMode);
                }
            }
        }
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
