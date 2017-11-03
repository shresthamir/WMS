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
using System.Windows;
using Syncfusion.UI.Xaml.Grid;

namespace GTransfer.Reports
{
    class PackagingListVM : BaseViewModel
    {
        public SfDataGrid Report;
        private DateTime _FDate;
        private DateTime _TDate;
        private ObservableCollection<PackaginListModel> _ReportDataList;
        private ObservableCollection<Division> _divisionList;
        private string _SelectedVoucher;
        private string _SelectedBranch;
        private bool _FilterVoucher;
        private bool _FilterDate = true;
        private bool _FilterBranch;
        private bool _SummaryReport;

        public string DIVISION { get { return GlobalClass.DIVISION; } }
        public ObservableCollection<Division> DivisionList { get { return _divisionList; } set { _divisionList = value; OnPropertyChanged("DivisionList"); } }
        public ObservableCollection<PackaginListModel> ReportDataList { get { return _ReportDataList; } set { _ReportDataList = value; OnPropertyChanged("ReportDataList"); } }
        public DateTime FDate { get { return _FDate; } set { _FDate = value; OnPropertyChanged("FDate"); } }
        public DateTime TDate { get { return _TDate; } set { _TDate = value; OnPropertyChanged("TDate"); } }
        public string SelectedVoucher { get { return _SelectedVoucher; } set { _SelectedVoucher = value; OnPropertyChanged("SelectedVoucher"); } }
        public string SelectedBranch { get { return _SelectedBranch; } set { _SelectedBranch = value; OnPropertyChanged("SelectedBranch"); } }
        public bool FilterVoucher { get { return _FilterVoucher; } set { _FilterVoucher = value; OnPropertyChanged("FilterVoucher"); } }
        public bool FilterBranch { get { return _FilterBranch; } set { _FilterBranch = value; OnPropertyChanged("FilterBranch"); } }
        public bool FilterDate { get { return _FilterDate; } set { _FilterDate = value; OnPropertyChanged("FilterDate"); } }
        public bool SummaryReport { get { return _SummaryReport; } set { _SummaryReport = value; OnPropertyChanged("SummaryReport"); } }
        public PackagingListVM()
        {
            try
            {
                FDate = TDate = DateTime.Today;
                using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    DivisionList = new ObservableCollection<Division>(con.Query<Division>("SELECT * FROM DIVISION"));
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Packaging List Report", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public override void LoadMethod(object obj)
        {
            try
            {
                Report.GroupColumnDescriptions.Clear();
                using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    IEnumerable<PackaginListModel> result;
                    if (!SummaryReport)
                    {
                        Report.GroupColumnDescriptions.Add(new GroupColumnDescription() { ColumnName = "VCHRNO" });
                        Report.GroupColumnDescriptions.Add(new GroupColumnDescription() { ColumnName = "PackageNo" });
                        result = con.Query<PackaginListModel>(@"SELECT CAST(RIGHT(M.VCHRNO,LEN(M.VCHRNO)-2) AS INT) VNUM, M.VCHRNO, CONVERT(VARCHAR,M.TRNDATE, 101) Date, D.NAME, ISNULL(P.GENERIC,'') PackageNo, I.MENUCODE, I.DESCA, P.UNIT, P.RealQty FROM RMD_TRNMAIN M 
JOIN RMD_TRNPROD P ON M.VCHRNO = P.VCHRNO AND M.DIVISION = P.DIVISION AND M.PhiscalID = P.PhiscalID
JOIN MENUITEM I ON P.MCODE = I.MCODE
JOIN DIVISION D ON M.BILLTOADD = D.INITIAL
WHERE LEFT(M.VCHRNO,2) IN ('TO')" +
    (!FilterDate ? string.Empty : " AND M.TRNDATE BETWEEN @FDate AND @TDate AND M.DIVISION = @DIVISION") +
    (!FilterVoucher ? string.Empty : " AND M.VCHRNO = @SelectedVoucher") +
    (!FilterBranch ? string.Empty : " AND M.BILLTOADD = @SelectedBranch") + @"
ORDER BY VNUM, PackageNo, DESCA", this);
                    }
                    else
                    {
                        Report.GroupColumnDescriptions.Add(new GroupColumnDescription() { ColumnName = "VCHRNO" });                        
                        result = con.Query<PackaginListModel>(@"SELECT CAST(RIGHT(M.VCHRNO,LEN(M.VCHRNO)-2) AS INT) VNUM, M.VCHRNO, CONVERT(VARCHAR,M.TRNDATE, 101) Date, D.NAME, '' PackageNo, I.MENUCODE, I.DESCA, P.UNIT, SUM(P.RealQty) RealQty FROM RMD_TRNMAIN M 
JOIN RMD_TRNPROD P ON M.VCHRNO = P.VCHRNO AND M.DIVISION = P.DIVISION AND M.PhiscalID = P.PhiscalID
JOIN MENUITEM I ON P.MCODE = I.MCODE
JOIN DIVISION D ON M.BILLTOADD = D.INITIAL
WHERE LEFT(M.VCHRNO,2) IN ('TO')" +
        (!FilterDate ? string.Empty : " AND M.TRNDATE BETWEEN @FDate AND @TDate AND M.DIVISION = @DIVISION") +
        (!FilterVoucher ? string.Empty : " AND M.VCHRNO = @SelectedVoucher") +
        (!FilterBranch ? string.Empty : " AND M.BILLTOADD = @SelectedBranch") + @"
GROUP BY M.VCHRNO, M.TRNDATE, D.NAME, I.MENUCODE, I.DESCA, P.UNIT
ORDER BY VNUM, DESCA", this);
                    }
                    if (result != null)
                    {
                        ReportDataList = new ObservableCollection<PackaginListModel>(result);
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Packaging List Report", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }

    class PackaginListModel
    {
        public string VCHRNO { get; set; }
        public string NAME { get; set; }
        public string Date { get; set; }
        public string PackageNo { get; set; }
        public string MENUCODE { get; set; }
        public string DESCA { get; set; }
        public string UNIT { get; set; }
        public decimal RealQty { get; set; }
    }
}
