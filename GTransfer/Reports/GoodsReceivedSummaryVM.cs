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
        private ObservableCollection<GoodsReceivedDetailModel> _ReportDataList;
        public ObservableCollection<GoodsReceivedDetailModel> ReportDataList { get { return _ReportDataList; } set { _ReportDataList = value; OnPropertyChanged("ReportDataList"); } }

        public DateTime FDate { get { return _FDate; } set { _FDate = value; OnPropertyChanged("FDate"); } }
        public DateTime TDate { get { return _TDate; } set { _TDate = value; OnPropertyChanged("TDate"); } }

        public GoodsReceivedSummaryVM()
        {
            FDate = TDate = DateTime.Today;
        }

        public override void LoadMethod(object obj)
        {
            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                var result = con.Query<GoodsReceivedDetailModel>(@"SELECT CAST(RIGHT(M.VCHRNO,LEN(M.VCHRNO)-2) AS INT) VNUM, M.REFORDBILL, M.VCHRNO, CONVERT(VARCHAR, M.TRNDATE, 101) Date, I.MENUCODE, I.DESCA, PD.UNIT, PD.WAREHOUSE, L.LocationCode, PD.InQty FROM RMD_TRNMAIN M 
JOIN RMD_TRNPROD_DETAIL PD ON M.VCHRNO = PD.VCHRNO AND M.DIVISION = PD.DIVISION AND M.PhiscalID = PD.PhiscalID
JOIN MENUITEM I ON PD.MCODE = I.MCODE
JOIN TBL_LOCATIONS L ON PD.LocationId = L.LocationId
WHERE LEFT(M.VCHRNO,2) IN ('PI') AND M.TRNDATE BETWEEN @FDate AND @TDate
ORDER BY VNUM, WAREHOUSE, LocationCode", this);
                if (result != null)
                {
                    ReportDataList = new ObservableCollection<GoodsReceivedDetailModel>(result);
                }
            }
        }
    }

    class GoodsReceivedSummaryModel
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
