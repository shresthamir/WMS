using GTransfer.Library;
using GTransfer.UserInterfaces;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Cells;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dapper;
namespace GTransfer.Reports
{
    /// <summary>
    /// Interaction logic for pickingList.xaml
    /// </summary>
    public partial class PickingListVariance : UserControl
    {
        public PickingListVariance()
        {
            InitializeComponent();
            this.DataContext = new vmPickingListVariance(Report);
        }
    }

    class vmPickingListVariance : BaseViewModel
    {
        private string _ReqId;
        public string ReqId { get { return _ReqId; } set { _ReqId = value; OnPropertyChanged("ReqId"); } }

        private ObservableCollection<PickingListVarianceModel> _ReportDataList;
        private SfDataGrid Report;

        public ObservableCollection<PickingListVarianceModel> ReportDataList { get { return _ReportDataList; } set { _ReportDataList = value; OnPropertyChanged("ReportDataList"); } }

        public vmPickingListVariance(SfDataGrid _Report)
        {
            Report = _Report;
        }

        public override void LoadMethod(object obj)
        {
            try
            {
                if (!string.IsNullOrEmpty(ReqId))
                    using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
                    {
                        var result = con.Query<PickingListVarianceModel>(@"SELECT RD.MCODE, MG.DESCA Category, MI.MENUCODE ItemCode, MI.DESCA ItemName, RD.BCODE Barcode, RD.Unit, RD.ApprovedQty ReqQty, SUM(ISNULL(PL.Quantity,0)) PickedQty, RD.ApprovedQty - SUM(ISNULL(PL.Quantity,0)) VarianceQty FROM TBL_REQUISITION_DETAILS RD
JOIN MENUITEM MI ON MI.MCODE = RD.MCODE
JOIN MENUITEM MG ON MI.MGROUP = MG.MCODE
LEFT JOIN tblPickingList PL ON RD.ReqId = Pl.ReqId AND RD.MCODE = PL.MCODE AND RD.BCODE = PL.BCODE AND RD.Unit = PL.Unit WHERE RD.ReqId = " + ReqId + @"
GROUP BY RD.MCODE, MG.DESCA, MI.MENUCODE, MI.DESCA, RD.BCODE, RD.Unit, RD.ApprovedQty
ORDER BY Category, ItemName, Unit
");
                        if (result != null)
                        {
                            ReportDataList = new ObservableCollection<PickingListVarianceModel>(result);
                        }
                    }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                MessageBox.Show(ex.Message, "Picking List Variance", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        #region PrintExport
        public override void ExecuteExport(object obj)
        {
            GlobalClass.ReportName = "Picking List Variance";
            GlobalClass.ReportParams = string.Format("Requisition Id : {0}", ReqId);

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
            GlobalClass.ReportName = "Picking List Variance";
            GlobalClass.ReportParams = string.Format("Requisition Id : {0}", ReqId);

            Report.PrintSettings.PrintPageMargin = new Thickness(30);
            Report.PrintSettings.AllowColumnWidthFitToPrintPage = false;
            Report.ShowPrintPreview();
        }

        public override void ExecutePrint(object obj)
        {

            GlobalClass.ReportName = "Picking List Variance";
            GlobalClass.ReportParams = string.Format("Requisition Id : {0}", ReqId);

            Report.PrintSettings.PrintPageMargin = new Thickness(30);
            Report.PrintSettings.AllowColumnWidthFitToPrintPage = false;
            Report.Print();
        }
        #endregion
    }

    public class PickingListVarianceModel
    {
        public string Category { get; set; }
        public string Barcode { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public decimal ReqQty { get; set; }
        public decimal PickedQty { get; set; }
        public decimal VarianceQty { get; set; }
    }
}
