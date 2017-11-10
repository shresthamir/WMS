using GTransfer.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Windows;
using GTransfer.Models;

namespace GTransfer.ViewModels
{
    class ShipmentVarianceViewModel : BaseViewModel
    {
        private int displayMode = 1;
        private IEnumerable<TrnProd> _AllProdList;
        private TrnMain _TMain;
        private ObservableCollection<TrnProd> _ProdList;
        private string _SupplierName;
        private bool NewShipment = false;

        public TrnMain TMain { get { return _TMain; } set { _TMain = value; OnPropertyChanged("TMain"); } }
        public RelayCommand OrderReferenceCommand { get { return new RelayCommand(ExecuteOrderReferenceCommand); } }
        public RelayCommand GeneratePICommand { get { return new RelayCommand(GeneratePI, CanGeneratePI); } }



        public ObservableCollection<TrnProd> ProdList { get { return _ProdList; } set { _ProdList = value; OnPropertyChanged("ProdList"); } }
        public string SupplierName { get { return _SupplierName; } set { _SupplierName = value; OnPropertyChanged("SupplierName"); } }
        public bool ShowAll { get { return displayMode == 1; } set { if (value) SetDisplayMode(1); } }
        public bool ShowVariant { get { return displayMode == 2; } set { if (value) SetDisplayMode(2); } }
        public bool ShowNonVariant { get { return displayMode == 3; } set { if (value) SetDisplayMode(3); } }


        void SetDisplayMode(int mode)
        {
            displayMode = mode;
            if (_AllProdList != null)
            {
                switch (displayMode)
                {
                    case 1:
                        ProdList = new ObservableCollection<TrnProd>(_AllProdList);
                        break;
                    case 2:
                        ProdList = new ObservableCollection<TrnProd>(_AllProdList.Where(x => x.VarianceQty != 0));
                        break;
                    case 3:
                        ProdList = new ObservableCollection<TrnProd>(_AllProdList.Where(x => x.VarianceQty == 0));
                        break;
                }
            }
            OnPropertyChanged("ShowAll");
            OnPropertyChanged("ShowVariant");
            OnPropertyChanged("ShowNonVariant");
        }
        public override void NewMethod(object obj)
        {
            UndoMethod(null);
            string vnum = string.Empty;
            string vno = string.Empty;
            string chalanNo = string.Empty;
            GlobalClass.GetBillSequences("Purchase", TMain.VoucherPrefix, TMain.VoucherName, TMain.VoucherPrefix, ref vno, ref chalanNo, ref vnum);
            TMain.VCHRNO = vno;
            TMain.VNUM = vnum;
        }

        public override bool UndoMethod(object obj)
        {
            TMain = new TrnMain
            {
                VoucherName = "Purchase Invoice",
                VoucherPrefix = "PI"
            };
            if (ProdList != null)
                ProdList.Clear();
            TMain.PropertyChanged += TMain_PropertyChanged;
            NewShipment = false;
            return false;
        }

        private void TMain_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "REFORDBILL")
            {
                TMain = new TrnMain
                {
                    VoucherName = "Purchase Invoice",
                    VoucherPrefix = "PI",
                    REFORDBILL = TMain.REFORDBILL
                };
                if (ProdList != null)
                    ProdList.Clear();
                TMain.PropertyChanged += TMain_PropertyChanged;
                _action = ButtonAction.Init;
            }
        }

        private void ExecuteOrderReferenceCommand(object obj)
        {
            if (string.IsNullOrEmpty(TMain.REFORDBILL))
                return;
            try
            {
                using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    NewShipment = con.ExecuteScalar<int>("SELECT COUNT(*) FROM tblStockInVerificationLog WHERE OrderNo = '" + TMain.REFORDBILL + "' AND ISNULL([Status], 0) = 0") > 0;
                    var orderTran = con.Query<TrnMain>(@"SELECT TRNAC, ACNAME PARAC FROM RMD_TRNMAIN T JOIN RMD_ACLIST A ON T.TRNAC = A.ACID
                                    WHERE LEFT(VCHRNO, 2) IN ('PO', 'OR') AND VCHRNO = '" + TMain.REFORDBILL + "' AND DIVISION = '" + GlobalClass.DIVISION + "'").FirstOrDefault();
                    if (orderTran == null)
                    {
                        MessageBox.Show("Invalid order no.", "GRN", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                    TMain.PARAC = orderTran.TRNAC;
                    SupplierName = orderTran.PARAC;
                    string ProdListQry = @"SELECT B.MCODE, MI.MENUCODE, MI.DESCA ITEMDESC, ISNULL(OP.RATE, MI.PRATE_A) REALRATE, ISNULL(OP.RATE, MI.PRATE_A) RATE, 
B.UNIT, LOC.Warehouse, ISNULL(OP.QUANTITY, 0) OrderQty, ISNULL(SUM(PL.REALQTY_IN), 0) AltQty, 
ISNULL(SUM(L.REALQTY_IN), 0) Quantity, ISNULL(SUM(L.REALQTY_IN), 0) REALQTY_IN, 
ISNULL(SUM(PL.REALQTY_IN), 0) + ISNULL(SUM(L.REALQTY_IN), 0) ALTQTY_IN, MI.RATE_A SRATE, MI.VAT ISVAT, MAX(L.SyncDate) MFGDATE FROM
(
    SELECT DISTINCT * FROM
    (
        SELECT VCHRNO OrderNo, MCODE, UNIT FROM RMD_ORDERPROD WHERE VCHRNO = @OrderNo
        UNION ALL
        SELECT OrderNo, MCODE, UNIT FROM tblStockInVerificationLog WHERE OrderNo = @OrderNo

    ) A
) B 
JOIN MENUITEM MI ON MI.MCODE = B.MCODE
LEFT JOIN RMD_ORDERPROD OP ON OP.VCHRNO = B.OrderNo AND OP.MCODE = B.MCODE AND OP.UNIT = B.UNIT
LEFT JOIN (SELECT * FROM tblStockInVerificationLog WHERE ISNULL([STATUS], 0) = 1) PL ON PL.OrderNo = B.OrderNo AND PL.MCODE = B.MCODE AND PL.UNIT = B.UNIT
LEFT JOIN (SELECT * FROM tblStockInVerificationLog WHERE ISNULL([STATUS], 0) = 0) L ON L.OrderNo = B.OrderNo AND L.MCODE = B.MCODE AND L.UNIT = B.UNIT
LEFT JOIN TBL_LOCATIONS LOC ON l.LocationId = LOC.LocationId                                            
GROUP BY B.MCODE, B.UNIT, OP.QUANTITY, OP.RATE, LOC.Warehouse, MI.MENUCODE, MI.DESCA, MI.PRATE_A, MI.RATE_A, MI.VAT";
                    _AllProdList = con.Query<TrnProd>(ProdListQry, new { OrderNo = TMain.REFORDBILL });
                    int sno = 1;
                    foreach (TrnProd tpod in _AllProdList)
                    {
                        
                        tpod.AMOUNT = tpod.Quantity * tpod.REALRATE;
                        tpod.TAXABLE = tpod.AMOUNT - tpod.NONTAXABLE - tpod.DISCOUNT;
                        if (tpod.ISVAT == 1)
                            tpod.VAT = tpod.TAXABLE * Settings.VatRate;
                        tpod.NETAMOUNT = tpod.TAXABLE + tpod.VAT;
                        tpod.VarianceQty = tpod.ALTQTY_IN - tpod.OrderQty;
                        tpod.SNO = sno++;
                    }
                    TMain.TOTAMNT = _AllProdList.Sum(x => x.AMOUNT);
                    TMain.TAXABLE = _AllProdList.Sum(x => x.TAXABLE);
                    TMain.VATAMNT = _AllProdList.Sum(x => x.VAT);
                    TMain.NETAMNT = _AllProdList.Sum(x => x.NETAMOUNT);
                    SetDisplayMode(displayMode);
                    _action = ButtonAction.Loaded;
                }
            }
            catch (Exception Ex)
            {
                while (Ex.InnerException != null)
                    Ex = Ex.InnerException;
                MessageBox.Show(Ex.Message, "Shipment Receive Variance", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GeneratePI(object obj)
        {
            try
            {
                TrnMain main = new TrnMain
                {
                    DIVISION = GlobalClass.DIVISION,
                    TRNDATE = DateTime.Today,
                    BSDATE = GlobalClass.GetBSDate(DateTime.Today),
                    TRNTIME = DateTime.Now.ToString("hh:mm tt"),
                    TRNUSER = GlobalClass.CurrentUser.UNAME,
                    REFORDBILL = TMain.REFORDBILL,
                    TRNAC = "AT01002",
                    PARAC = TMain.PARAC,
                    TRNMODE = "Cash",
                    TOTAMNT = TMain.TOTAMNT,
                    TAXABLE = TMain.TAXABLE,
                    VATAMNT = TMain.VATAMNT,
                    NETAMNT = TMain.NETAMNT,
                    NONTAXABLE = TMain.NONTAXABLE,
                    DCAMNT = TMain.DCAMNT
                };

                using (SqlConnection conn = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    conn.Open();
                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        main.VCHRNO = conn.ExecuteScalar<string>(
@"IF EXISTS (SELECT * FROM RMD_SEQUENCES WHERE VNAME = @VNAME AND DIV = @DIV)
	SELECT 'PI' + CAST(CurNo AS VARCHAR) FROM RMD_SEQUENCES WHERE VNAME = @VNAME AND DIV = @DIV
ELSE 
	INSERT INTO RMD_SEQUENCES(VNAME, AUTO, Start, CurNo, DIV, DIVISION, VoucherType, VoucherName)
	OUTPUT 'PI' + CAST(inserted.CurNo AS VARCHAR)
	VALUES (@VNAME, 1, 1, 1, @DIV, @DIV, 'PI', 'Goods Received Voucher')", new { VNAME = "Purchase", DIV = GlobalClass.DIVISION }, tran);


                        conn.Execute(
@"INSERT INTO RMD_TRNMAIN(VCHRNO, DIVISION, CHALANNO, TRNDATE, BSDATE, TRNTIME, TRNUSER, REFORDBILL, TRNAC, PARAC, TOTAMNT, TAXABLE, NONTAXABLE, DCAMNT, VATAMNT, NETAMNT, TRNMODE, CHEQUENO, REMARKS, EditUser, SHIFT, CHEQUEDATE)
VALUES(@VCHRNO, @DIVISION, @VCHRNO, @TRNDATE, @BSDATE, @TRNTIME, @TRNUSER, @REFORDBILL, @TRNAC, @PARAC, @TOTAMNT, @TAXABLE, @NONTAXABLE, @DCAMNT, @VATAMNT, @NETAMNT, @TRNMODE, '', '', '', '', @TRNDATE)", main, tran);

                        foreach(TrnProd tpod in _AllProdList)
                        {
                            tpod.AltQty = 0;
                            tpod.ALTQTY_IN = 0;
                        }

                        conn.Execute(
@"INSERT INTO RMD_TRNPROD (VCHRNO, DIVISION, MCODE, UNIT, Quantity, RealQty, RATE, AMOUNT, DISCOUNT, VAT, REALRATE, REALQTY_IN, WAREHOUSE, TAXABLE, NONTAXABLE, SNO, IDIS)
VALUES ('" + main.VCHRNO + "', '" + GlobalClass.DIVISION + "', @MCODE, @UNIT, @Quantity, @RealQty, @RATE, @AMOUNT, @DISCOUNT, @VAT, @REALRATE, @REALQTY_IN, @WAREHOUSE, @TAXABLE, @NONTAXABLE, @SNO, 0)", _AllProdList.Where(x => x.REALQTY_IN > 0), tran);

                        conn.Execute(
@"INSERT INTO RMD_TRNPROD_DETAIL (VCHRNO, DIVISION, PhiscalID, MCODE, UNIT, Warehouse, LocationId, InQty, OutQty, SNO)
SELECT M.VCHRNO, M.DIVISION, M.PhiscalID, L.MCODE, L.Unit, P.WAREHOUSE, L.LocationId,L.RealQty_In , 0, P.SNO  FROM [tblStockInVerificationLog] L
JOIN RMD_TRNMAIN M ON L.OrderNo = M.REFORDBILL
JOIN RMD_TRNPROD P ON M.VCHRNO = P.VCHRNO AND M.DIVISION = P.DIVISION AND M.PhiscalID = P.PhiscalID AND L.MCODE = P.MCODE AND L.Unit = P.UNIT
WHERE L.OrderNo = @REFORDBILL", main, tran);

                        conn.Execute("UPDATE tblStockInVerificationLog SET [Status] = 1 WHERE OrderNo = @OrderNo AND SyncDate <= @SyncDate", new { OrderNo = main.REFORDBILL, SyncDate = _AllProdList.Max(x=>x.MFGDATE) }, tran);

                        conn.Execute("UPDATE RMD_SEQUENCES SET CURNO = CURNO + 1 WHERE VNAME = @VNAME AND DIV = @DIV", new { VNAME = "Purchase", DIV = GlobalClass.DIVISION }, tran);
                        tran.Commit();
                        MessageBox.Show("Voucher successfully generated");
                        ExecuteUndo(null);
                    }
                }
            }
            catch (Exception Ex)
            {
                while (Ex.InnerException != null)
                    Ex = Ex.InnerException;
                MessageBox.Show(Ex.Message, "Shipment Receive Variance", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanGeneratePI(object obj)
        {
            return _action == ButtonAction.Loaded && NewShipment;
        }
    }

    class ShipmentLogViewModel : BaseViewModel
    {
        private string _PONumber;
        private ObservableCollection<dynamic> _SRLog;

        public string PONumber { get { return _PONumber; } set { _PONumber = value; OnPropertyChanged("PONumber"); } }
        public ObservableCollection<dynamic> SRLog { get { return _SRLog; } set { _SRLog = value; OnPropertyChanged("SRLog"); } }
        public RelayCommand OrderReferenceCommand { get { return new RelayCommand(ExecuteOrderReferenceCommand); } }

        private void ExecuteOrderReferenceCommand(object obj)
        {
            if (string.IsNullOrEmpty(PONumber))
                return;
            try
            {
                using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    string strQry = @"SELECT 0 SNO, MI.MENUCODE, MI.DESCA, L.Unit, L.RealQty_In, LOC.Warehouse, LOC.LocationName, LOC.Path, L.UserId, D.DeviceName, L.TrnDate, L.TrnTime, L.SyncDate   FROM tblStockInVerificationLog L 
                                            JOIN MENUITEM MI ON L.MCODE = MI.MCODE
                                            JOIN TBL_LOCATIONS LOC ON L.LocationId = LOC.LocationId
                                            JOIN tblDevices D ON L.DeviceId= D.DeviceId
                                            WHERE OrderNo = '" + PONumber + "'";
                    SRLog = new ObservableCollection<dynamic>(con.Query(strQry));
                }
            }
            catch (Exception Ex)
            {
                while (Ex.InnerException != null)
                    Ex = Ex.InnerException;
                MessageBox.Show(Ex.Message, "Shipment Receive Variance", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
