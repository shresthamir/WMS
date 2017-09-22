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
    class PackagingListVM : BaseViewModel
    {
        private DateTime _FDate;
        private DateTime _TDate;
        private ObservableCollection<PackaginListModel> _ReportDataList;
        public ObservableCollection<PackaginListModel> ReportDataList { get { return _ReportDataList; } set { _ReportDataList = value; OnPropertyChanged("ReportDataList"); } }

        public DateTime FDate { get { return _FDate; } set { _FDate = value; OnPropertyChanged("FDate"); } }
        public DateTime TDate { get { return _TDate; } set { _TDate = value; OnPropertyChanged("TDate"); } }

        public PackagingListVM()
        {
            FDate = TDate = DateTime.Today;
        }

        public override void LoadMethod(object obj)
        {
            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                var result = con.Query<PackaginListModel>(@"SELECT CAST(RIGHT(M.VCHRNO,LEN(M.VCHRNO)-2) AS INT) VNUM, M.VCHRNO, CONVERT(VARCHAR,M.TRNDATE, 101) Date, ISNULL(P.GENERIC,'') PackageNo, I.MENUCODE, I.DESCA, P.UNIT, P.RealQty FROM RMD_TRNMAIN M 
JOIN RMD_TRNPROD P ON M.VCHRNO = P.VCHRNO AND M.DIVISION = P.DIVISION AND M.PhiscalID = P.PhiscalID
JOIN MENUITEM I ON P.MCODE = I.MCODE
WHERE LEFT(M.VCHRNO,2) IN ('TO') AND M.TRNDATE BETWEEN @FDate AND @TDate
ORDER BY VNUM, PackageNo, DESCA", this);
                if (result != null)
                {
                    ReportDataList = new ObservableCollection<PackaginListModel>(result);
                }
            }

        }       
    }

    class PackaginListModel
    {
        public string VCHRNO { get; set; }
        public string Date { get; set; }
        public string PackageNo { get; set; }
        public string MENUCODE { get; set; }
        public string DESCA { get; set; }
        public string UNIT { get; set; }
        public decimal RealQty { get; set; }
    }
}
