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
using System.Data;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;

namespace GTransfer.ViewModels
{
    class RequisitionEntryViewModel : BaseViewModel
    {
        private Requisition _RequisitionObj;
        private Requisition_Detail _Requisition_DetailObj;
        private Requisition_Detail _selectedReqDetails;
        private ObservableCollection<Division> _divisionList;
        private ObservableCollection<Unit> _UnitList;
        private ObservableCollection<Product> _ItemList;
        private string _SelectedItemMcode;

       
        public ObservableCollection<Division> divisionList { get { return _divisionList; } set { _divisionList = value; OnPropertyChanged("divisionList"); } }
        public ObservableCollection<Unit> UnitList { get { return _UnitList; } set { _UnitList = value; OnPropertyChanged("UnitList"); } }
        public ObservableCollection<Product> ItemList { get { return _ItemList; } set { _ItemList = value; OnPropertyChanged("ItemList"); } }
        public Requisition RequisitionObj { get { return _RequisitionObj; } set { _RequisitionObj = value; OnPropertyChanged("RequisitionObj"); } }
        public Requisition_Detail Requisition_DetailObj { get { return _Requisition_DetailObj; } set { _Requisition_DetailObj = value; OnPropertyChanged("Requisition_DetailObj"); } }
        public Requisition_Detail selectedReqDetails { get { return _selectedReqDetails; } set { _selectedReqDetails = value; OnPropertyChanged("selectedReqDetails"); } }
        public string SelectedItemMcode {
            get { return _SelectedItemMcode; }
            set {
                if (_SelectedItemMcode == value) return;
                _SelectedItemMcode = value;
                if (!string.IsNullOrEmpty(_SelectedItemMcode)) {
                    changeItemEvent();
                }
                OnPropertyChanged("SelectedItemMcode"); } }

        private void changeItemEvent(bool IsbarcodeChangeEvent = false) {
            if (Requisition_DetailObj == null) Requisition_DetailObj = new Requisition_Detail();
            Requisition_DetailObj.Mcode = _SelectedItemMcode;
            Requisition_DetailObj.Item = new Product(_SelectedItemMcode);
            Requisition_DetailObj.Unit = Requisition_DetailObj.Item.BASEUNIT;
            if (IsbarcodeChangeEvent == false)
            {
                Requisition_DetailObj.Bcode = Requisition_DetailObj.Item.BARCODE;
            }
        }

        public RelayCommand AddCommand { get { return new RelayCommand(ExecuteAdd); } }
        public RelayCommand GridDoubleClickEvent { get { return new RelayCommand(ExecuteGridDoubleClickEvent); } }
        public RelayCommand ExcelImportEvent { get { return new RelayCommand(ExecuteExcelImportEvent); } }
        public RelayCommand BarcodeChangeCommand { get { return new RelayCommand(ExecuteBarcodeChangeCommand); } }

        private void ExecuteBarcodeChangeCommand(object obj)
        {try {
                if (!string.IsNullOrEmpty(Requisition_DetailObj.Bcode)) {
                    using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString)) {
                       var result= con.Query("SELECT A.BCODE,A.MCODE,A.UNIT,A.ISSUENO,A.EDATE,A.BCODEID,A.SUPCODE,A.BATCHNO,A.EXPIRY,A.REMARKS,A.INVNO,A.DIV,A.FYEAR,A.SRATE,B.DESCA FROM BARCODE A inner join Menuitem B on A.mcode=B.mcode WHERE A.BCODE='"+Requisition_DetailObj.Bcode+"'").FirstOrDefault();
                        if (result == null) { MessageBox.Show("Invalid Barcode");return; }
                        SelectedItemMcode = result.MCODE;
                        changeItemEvent(true);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            
        }

        public RequisitionEntryViewModel()
        {try
            {
                EditVisible = true;
                DeleteVisible = true;
                RequisitionObj = new Requisition();
                divisionList = new ObservableCollection<Division>(GetDivisionList());
                UnitList = new ObservableCollection<Unit>(GetUnitList());
                ItemList = new ObservableCollection<Product>(GetItemList());
                RequisitionObj = new Requisition() { Requisition_Details = new ObservableCollection<Requisition_Detail>() };
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void ExecuteAdd(object obj)
        {
            if (Requisition_DetailObj.Item == null) { return; }
            if (Requisition_DetailObj.Quantity <= 0) { MessageBox.Show("Please provide valid Quantity");return; }
            if (RequisitionObj.Requisition_Details.Any((item) => item.Mcode == Requisition_DetailObj.Mcode))
            {
                    MessageBox.Show("Item  is already in the Table below", "Duplicate Data", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }

            RequisitionObj.Requisition_Details.Add(Requisition_DetailObj);
            Requisition_DetailObj = new Requisition_Detail();
            SelectedItemMcode = null;
        }
        private void ExecuteGridDoubleClickEvent(object obj)
        {
           var RD = GParse.CopyPropertyValues(selectedReqDetails, new Requisition_Detail()) as Requisition_Detail;
            SelectedItemMcode = RD.Mcode;
            Requisition_DetailObj.Unit = RD.Unit;
            Requisition_DetailObj.Quantity = RD.Quantity;
            RequisitionObj.Requisition_Details.Remove(selectedReqDetails);
             }
        public override bool UndoMethod(object obj)
        {
            RequisitionObj = new Requisition() { Requisition_Details = new ObservableCollection<Requisition_Detail>() };
            Requisition_DetailObj = new Requisition_Detail();
            SelectedItemMcode = null;
            return false;
        }
        public override void DeleteMethod(object obj)
        {
            if (MessageBox.Show("You are about to delete current Requisition.Once you Delete you cannot recover it back. Do you want to proceed?", "Delete Data", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
            {
                Conn.Open();
                Conn.Execute("DELETE FROM TBL_REQUISITION_DETAILS WHERE ReqId=" + RequisitionObj.ReqId + "");
                Conn.Execute("DELETE FROM TBL_REQUISITION WHERE ReqId=" + RequisitionObj.ReqId + "");
            }
            MessageBox.Show("Requisition is Deleted Successfully...");
     
            ExecuteUndo(null);
        }
        public override void LoadMethod(object obj)
        {
            if (RequisitionObj == null || RequisitionObj.ReqId == 0) return;
            using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
            {
                Conn.Open();
               var RQ=  Conn.Query<Requisition>("SELECT * FROM TBL_REQUISITION WHERE ReqId=" + RequisitionObj.ReqId + "");
                if (RQ != null) {
                    var RQD = Conn.Query<Requisition_Detail>("SELECT * FROM TBL_REQUISITION_DETAILS WHERE ReqId=" + RequisitionObj.ReqId + "");
                    RequisitionObj = RQ.FirstOrDefault();
                    RequisitionObj.Requisition_Details = new ObservableCollection<Requisition_Detail>(RQD);
                    foreach (var r in RequisitionObj.Requisition_Details) {
                        r.Item = ItemList.FirstOrDefault(x => x.MCODE == r.Mcode);
                    }
                    SetAction(ButtonAction.Selected);
                }

               
            }

        }
        public override void NewMethod(object obj)
        {
            RequisitionObj = new Requisition() { Requisition_Details = new ObservableCollection<Requisition_Detail>(),ReqId=GetNewReqId(),TDate=DateTime.Today,Exp_DeliveryDate=DateTime.Today };
            Requisition_DetailObj = new Requisition_Detail();

        }
        public override void SaveMethod(object obj)
        {
            if (MessageBox.Show("You are about to Save the Requisition. Do you want to proceed?", "Save Data", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            if (RequisitionObj == null || RequisitionObj.Requisition_Details == null || RequisitionObj.Requisition_Details.Count <= 0) return;
            if (string.IsNullOrEmpty(RequisitionObj.Division)) { MessageBox.Show("Please select a Division"); return; }
            try {
                if (_action == ButtonAction.New)
                {
                    RequisitionObj.ReqId = GetNewReqId();
                    RequisitionObj.TUser = GlobalClass.CurrentUser.UNAME;
                    foreach (var rd in RequisitionObj.Requisition_Details)
                    {
                        rd.ReqId = RequisitionObj.ReqId;
                        rd.ApprovedQty = rd.Quantity;
                    }

                    using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
                    {
                        Conn.Open();
                        SqlTransaction tran = Conn.BeginTransaction();
                        try
                        {
                            Conn.Execute("INSERT INTO TBL_REQUISITION(ReqId, Division, TDate, TUser, Exp_DeliveryDate, IsApproved) VALUES(@ReqId,@Division,@TDate,@TUser,@Exp_DeliveryDate, @IsApproved)", RequisitionObj, tran);
                            Conn.Execute("INSERT INTO TBL_REQUISITION_DETAILS(ReqId, Mcode, Quantity, ApprovedQty, Unit) VALUES(@ReqId,@Mcode,@Quantity,@ApprovedQty,@Unit)", RequisitionObj.Requisition_Details, tran);
                            tran.Commit();
                            MessageBox.Show("Requisition is Saved Sucessfully...");
                        }
                        catch(Exception ex) { tran.Rollback();MessageBox.Show(ex.Message);return; }

                    }
                }
                else if (_action == ButtonAction.Edit) {
                    RequisitionObj.TUser = GlobalClass.CurrentUser.UNAME;
                    foreach (var rd in RequisitionObj.Requisition_Details)
                    {
                        rd.ReqId = RequisitionObj.ReqId;
                        rd.ApprovedQty = rd.Quantity;
                    }

                    using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
                    {
                        Conn.Open();
                        SqlTransaction tran = Conn.BeginTransaction();
                        try
                        {
                            Conn.Execute("UPDATE TBL_REQUISITION SET Division=@Division, TDate=@TDate, TUser=@TUser, Exp_DeliveryDate=@Exp_DeliveryDate, IsApproved=@IsApproved WHERE ReqId=@ReqId", RequisitionObj, tran);
                            Conn.Execute("DELETE FROM TBL_REQUISITION_DETAILS WHERE ReqId=" + RequisitionObj.ReqId + "",null,tran);
                            Conn.Execute("INSERT INTO TBL_REQUISITION_DETAILS(ReqId, Mcode, Quantity, ApprovedQty, Unit) VALUES(@ReqId,@Mcode,@Quantity,@ApprovedQty,@Unit)", RequisitionObj.Requisition_Details, tran);
                            tran.Commit();
                            MessageBox.Show("Requisition is Updated Sucessfully...");
                        }
                        catch(Exception ex) { tran.Rollback();MessageBox.Show(ex.Message);return; }

                    }
                }
                ExecuteUndo(null);
                } catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private int GetNewReqId() {
            using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
            {
                              
                 return Convert.ToInt32(Conn.ExecuteScalar("SELECT ISNULL(MAX(ReqId),0)+1 FROM TBL_REQUISITION"));
            }
        }

        private IEnumerable<Product> GetItemList()
        {
            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                return con.Query<Product>("SELECT * FROM MENUITEM WHERE TYPE='A'");
            }
        }
        private IEnumerable<Unit> GetUnitList()
        {
            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                return con.Query<Unit>("SELECT UNITS FROM UNIT");
            }
        }
        private IEnumerable<Division> GetDivisionList()
        {
            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                return con.Query<Division>("SELECT * FROM DIVISION");
            }
        }

        private void ExecuteExcelImportEvent(object obj)
        {try
            {
                if (_action != ButtonAction.New) { MessageBox.Show("Please create new Id By Clicking New And Then Import the file...");return; }
                convertDataTableToObsCollection();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        #region importFromExcel
        private DataTable getExcelDataToDataTable()
        {
            DataTable dt = new DataTable();
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.DefaultExt = ".xlsx";
            openFile.Filter = "(.xlsx)|*.xlsx|All Files(*.*)|*.*";

            var browsefile = openFile.ShowDialog();
            if (browsefile == true)
            {

                string txtFilePath = (openFile.FileName).ToString();
                Excel.Application excelApp = new Excel.Application();
                //Static File From Base Path...........
                //Microsoft.Office.Interop.Excel.Workbook excelBook = excelApp.Workbooks.Open(AppDomain.CurrentDomain.BaseDirectory + "TestExcel.xlsx", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                //Dynamic File Using Uploader...........

                Excel.Workbook excelBook = excelApp.Workbooks.Open(txtFilePath, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                Excel.Worksheet excelSheet = (Excel.Worksheet)excelBook.Worksheets.get_Item(1); ;
                Excel.Range excelRange = excelSheet.UsedRange;

                string strCellData = "";
                double douCellData;
                int rowCnt = 0;
                int colCnt = 0;



                for (colCnt = 1; colCnt <= excelRange.Columns.Count; colCnt++)
                {
                    string strColumn = "";
                    strColumn = (string)(excelRange.Cells[1, colCnt] as Excel.Range).Value2;
                    dt.Columns.Add(strColumn, typeof(string));
                }
                for (rowCnt = 2; rowCnt <= excelRange.Rows.Count; rowCnt++)
                {
                    string strData = "";
                    for (colCnt = 1; colCnt <= excelRange.Columns.Count; colCnt++)
                    {
                        try
                        {
                            strCellData = (string)(excelRange.Cells[rowCnt, colCnt] as Excel.Range).Value2;
                            strData += strCellData + "|";
                        }
                        catch (Exception ex)
                        {
                            douCellData = (excelRange.Cells[rowCnt, colCnt] as Excel.Range).Value2;
                            strData += douCellData.ToString() + "|";
                        }
                    }
                    strData = strData.Remove(strData.Length - 1, 1);
                    dt.Rows.Add(strData.Split('|'));

                }
                excelBook.Close(true, null, null);
                excelApp.Quit();
                return dt;
            }
            else { return null; }
            
        }
        private void convertDataTableToObsCollection()
        {
            var _dataTable = getExcelDataToDataTable();
            if (_dataTable == null) return;
            DataColumnCollection columns = _dataTable.Columns;
            if (columns.Contains("BARCODE") || columns.Contains("MENUCODE"))
            {      
                }
            else
            {
                MessageBox.Show("Invalid Excel File.Must contain MENUCODE Or BARCODE Column And QUANTITY Column");return;
            }
            if (columns.Contains("QUANTITY")) { }
            else { MessageBox.Show("Invalid Excel File.Must contain QUANTITY Column"); return; }
            var productList = new ObservableCollection<Requisition_Detail>(_dataTable.AsEnumerable().Select(i => {
                var RD = new Requisition_Detail();
                Product P=new Product();
                if (columns.Contains("BARCODE"))
                {
                    P = ItemList.FirstOrDefault(item => item.BARCODE == i["BARCODE"].ToString());
                    if (P == null)
                    {
                        if (columns.Contains("MENUCODE"))
                        {
                            P = ItemList.FirstOrDefault(item => item.MENUCODE == i["MENUCODE"].ToString());
                        }
                    }
                }
                else if (columns.Contains("MENUCODE"))
                {
                    P = ItemList.FirstOrDefault(item => item.MENUCODE == i["MENUCODE"].ToString());
                }               
                var Q = Convert.ToDecimal(i["QUANTITY"]);
                if (P == null || Q <= 0) { return new Requisition_Detail(); }
                RD.Item = P;
                RD.Mcode = P.MCODE;
                RD.Unit = P.BASEUNIT;
                RD.ApprovedQty = Q;
                RD.Quantity = Q;
                return RD;
            }));
            if (productList != null) { this.RequisitionObj.Requisition_Details =new ObservableCollection<Requisition_Detail>(productList.Where(item=>item.Item!=null)); }
       
        }
        #endregion
    }
}
