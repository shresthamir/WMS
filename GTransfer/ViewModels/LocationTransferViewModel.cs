using GTransfer.Library;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Collections.ObjectModel;
using System.Windows;
using GTransfer.Models;

namespace GTransfer.ViewModels
{
    class LocationTransferViewModel:BaseViewModel
    {
        private string _LTCode;
        private string _DeviceId;
        private ObservableCollection<locationT> _LTItemList;

        public ObservableCollection<dynamic> DeviceList { get {
                using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString)) {
                  return new ObservableCollection<dynamic>(con.Query("SELECT DeviceId,DeviceName FROM tblDevices"));
                }
                    } }
        public string LTCode { get { return _LTCode; }set { _LTCode = value; LTItemList = null; OnPropertyChanged("LTCode"); } }
        public string DeviceId { get { return _DeviceId; } set { _DeviceId = value; LTItemList = null; OnPropertyChanged("DeviceId"); } }
        public ObservableCollection<locationT> LTItemList { get { return _LTItemList; }set { _LTItemList = value;OnPropertyChanged("LTItemList"); } }
        public RelayCommand LoadLTItemCommand { get { return new RelayCommand(ExecuteLoadLTItemCommand); } }
        public RelayCommand SaveLocationTransferCommand { get { return new RelayCommand(ExecuteSaveLocationTransferCommand); } }
        public RelayCommand CancelCommand { get { return new RelayCommand(ExecuteCancelCommand); } }

      
        public LocationTransferViewModel() { }

        private void ExecuteLoadLTItemCommand(object obj)
        {
            try
            {
                if (string.IsNullOrEmpty(LTCode) || LTCode.Length < 3 || LTCode.Substring(0, 2).ToUpper() != "LT") return;
                
                using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    con.Open();
                    var check = con.Query("SELECT * FROM RMD_TRNMAIN WHERE REFORDBILL ='"+LTCode+"' AND RETTO='"+DeviceId+"'");
                    if (check != null && check.Count()>0) { MessageBox.Show("Location Transfer already Done for this voucher.Try another");return; }
                    var result = con.Query<locationT>(@"select SLog.MCODE, MI.MENUCODE, MI.DESCA, SLog.Unit, L.LocationCode,L.LocationId,L.Warehouse, ISNULL(LB.Balance, 0) ActualStock, SLog.Quantity OutQty, SLog.RealQty_IN InQty from [tblStockInVerificationLog] SLog 
                                                  JOIN MENUITEM MI ON SLog.MCODE = MI.MCODE
                                                  JOIN TBL_LOCATIONS L ON L.LocationId = SLog.LocationId
                                                  LEFT JOIN vwLocationStockBalance LB ON SLog.MCODE = LB.MCODE AND SLog.Unit = LB.Unit AND SLog.LocationId = LB.LocationId
                                                   WHERE DeviceId = '" + DeviceId + "' AND DeviceTrnId = '" + LTCode + "' ORDER BY LocationCode, MENUCODE");
                    if (result != null)
                    {
                        LTItemList = new ObservableCollection<locationT>(result);
                        foreach (var i in LTItemList) {
                            if (i.ActualStock < i.OutQty) {
                                i.misMatchData = 1;
                            } }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ExecuteCancelCommand(object obj)
        {
            LTItemList = null;
            LTCode = null;
            DeviceId = null;
        }

        private void ExecuteSaveLocationTransferCommand(object obj)
        {try
            {
                if (LTItemList == null || LTItemList.Count <= 0) { MessageBox.Show("Please Load the Items For Transfer Verification"); return; }
                if (LTItemList.Sum(x => x.InQty) != LTItemList.Sum(y => y.OutQty)) { MessageBox.Show("In Quantity not match with Out Quantity.Please Check the data and try again.Thank you");return; }
               
                TrnMain main = new TrnMain
                {
                    DIVISION = GlobalClass.DIVISION,
                    TRNDATE = DateTime.Today,
                    BSDATE = GlobalClass.GetBSDate(DateTime.Today),
                    TRNTIME = DateTime.Now.ToString("hh:mm tt"),
                    TRNUSER = GlobalClass.CurrentUser.UNAME,
                    REFORDBILL = LTCode,
                    RETTO=DeviceId
                    // TRNAC = "AT01002",
                    // PARAC = TMain.PARAC,
                    // TRNMODE = "Cash",
                    // TOTAMNT = TMain.TOTAMNT,
                    // TAXABLE = TMain.TAXABLE,
                    //  VATAMNT = TMain.VATAMNT,
                    //  NETAMNT = TMain.NETAMNT,
                    //   NONTAXABLE = TMain.NONTAXABLE,
                    //  DCAMNT = TMain.DCAMNT
                };

                using (SqlConnection conn = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    conn.Open();
                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        main.VCHRNO = conn.ExecuteScalar<string>(
@"IF EXISTS (SELECT * FROM RMD_SEQUENCES WHERE VNAME = @VNAME AND DIV = @DIV)
	SELECT 'LT' + CAST(CurNo AS VARCHAR) FROM RMD_SEQUENCES WHERE VNAME = @VNAME AND DIV = @DIV
ELSE 
	INSERT INTO RMD_SEQUENCES(VNAME, AUTO, Start, CurNo, DIV, DIVISION, VoucherType, VoucherName)
	OUTPUT 'LT' + CAST(inserted.CurNo AS VARCHAR)
	VALUES (@VNAME, 1, 1, 1, @DIV, @DIV, 'LT', 'Location Transfer Voucher')", new { VNAME = "LocTransfer", DIV = GlobalClass.DIVISION }, tran);


                        conn.Execute(
@"INSERT INTO RMD_TRNMAIN(VCHRNO, DIVISION, CHALANNO, TRNDATE, BSDATE, TRNTIME, TRNUSER, REFORDBILL, TRNAC, PARAC, TOTAMNT, TAXABLE, NONTAXABLE, DCAMNT, VATAMNT, NETAMNT, TRNMODE, CHEQUENO, REMARKS, EditUser, SHIFT, CHEQUEDATE,RETTO)
VALUES(@VCHRNO, @DIVISION, @VCHRNO, @TRNDATE, @BSDATE, @TRNTIME, @TRNUSER, @REFORDBILL, @TRNAC, @PARAC, @TOTAMNT, @TAXABLE, @NONTAXABLE, @DCAMNT, @VATAMNT, @NETAMNT, @TRNMODE, '', '', '', '', @TRNDATE,@RETTO)", main, tran);


                        var LTSaveList = new ObservableCollection<locationT>();
                        int sn = 0;
                        string tempMcode = "";
                        foreach (var i in LTItemList.OrderBy(x=>x.MCODE))
                        {
                            if (tempMcode != i.MCODE) { sn++; }
                            tempMcode = i.MCODE;
                            i.VCHRNO = main.VCHRNO;
                            i.PhiscalID = GlobalClass.PhiscalId;
                            i.DIVISION = main.DIVISION;
                          
                            i.SNO = sn;
                            LTSaveList.Add(i);

                        }
                        conn.Execute(
@"INSERT INTO RMD_TRNPROD_DETAIL (VCHRNO, DIVISION, PhiscalID, MCODE, UNIT, Warehouse, LocationId, InQty, OutQty, SNO)
values(@VCHRNO, @DIVISION, @PhiscalID, @MCODE, @Unit, @Warehouse, @LocationId, @InQty, @OutQty, @SNO)", LTSaveList, tran);

                        conn.Execute("UPDATE RMD_SEQUENCES SET CURNO = CURNO + 1 WHERE VNAME = @VNAME AND DIV = @DIV", new { VNAME = "LocTransfer", DIV = GlobalClass.DIVISION }, tran);
                        tran.Commit();
                        MessageBox.Show("Location Transfer Successfull........");
                       ExecuteCancelCommand(null);
                    }
                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message);
            }
        }

    }
    public class locationT:BaseModel {
        private string _MCODE;
        private decimal _Quantity;
        private string _Unit;
        private string _LocationId;
        private string _DeviceId;
        private string _LocationCode;
        private string _Warehouse;
        private decimal _ActualStock;
        private decimal _OutQty;
        private decimal _InQty;
        private string _MENUCODE;
        private string _DESCA;
        private string _VCHRNO;
        private string _DIVISION;
        private string _PhiscalID;
        private int _SNO;
        private byte _misMatchData;

        public byte misMatchData { get { return _misMatchData; }set { _misMatchData = value;OnPropertyChanged("misMatchData"); } }
        public int SNO { get { return _SNO; }set { _SNO = value;OnPropertyChanged("SNO"); } }
        public string Warehouse { get { return _Warehouse; } set { _Warehouse = value; OnPropertyChanged("Warehouse"); } }
        public string VCHRNO { get { return _VCHRNO; } set { _VCHRNO = value; OnPropertyChanged("VCHRNO"); } }
        public string DIVISION { get { return _DIVISION; } set { _DIVISION = value; OnPropertyChanged("DIVISION"); } }
        public string PhiscalID { get { return _PhiscalID; }set { _PhiscalID = value;OnPropertyChanged("PhiscalID"); } }
        public string MCODE { get { return _MCODE; }set { _MCODE = value;OnPropertyChanged("MCODE"); } }
        public decimal Quantity { get { return _Quantity; } set { _Quantity = value; OnPropertyChanged("Quantity"); } }
        public string Unit { get { return _Unit; } set { _Unit = value; OnPropertyChanged("Unit"); } }
        public string LocationId { get { return _LocationId; } set { _LocationId = value; OnPropertyChanged("LocationId"); } }
        public string LocationCode { get { return _LocationCode; } set { _LocationCode = value; OnPropertyChanged("LocationCode"); } }
        public string MENUCODE { get { return _MENUCODE; } set { _MENUCODE = value; OnPropertyChanged("MENUCODE"); } }
        public string DESCA { get { return _DESCA; } set { _DESCA = value; OnPropertyChanged("DESCA"); } }
        public string DeviceId { get { return _DeviceId; } set { _DeviceId = value; OnPropertyChanged("DeviceId"); } }
        public decimal InQty { get { return _InQty; } set { _InQty = value; OnPropertyChanged("InQty"); } }
        public decimal OutQty { get { return _OutQty; } set { _OutQty = value; OnPropertyChanged("OutQty"); } }
        public decimal ActualStock { get { return _ActualStock; } set { _ActualStock = value; OnPropertyChanged("ActualStock"); } }

    }
}
