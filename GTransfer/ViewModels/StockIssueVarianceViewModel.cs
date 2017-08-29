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
    class StockIssueVarianceViewModel : BaseViewModel
    {
        private int displayMode = 1;
        private IEnumerable<TrnProd> _AllProdList;
        private TrnMain _TMain;
        private ObservableCollection<TrnProd> _ProdList;
        private string _SupplierName;
        private bool NewShipment = false;
        private Division _Division;

        public Division Division { get { return _Division; } set { _Division = value; OnPropertyChanged("Division"); } }
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
                    NewShipment = con.ExecuteScalar<int>("SELECT COUNT(*) FROM tblPickingList WHERE ReqId = " + TMain.REFORDBILL + " AND [Status] = 1") == 0;
                    Division = con.Query<Division>("SELECT INITIAL, NAME FROM DIVISION D JOIN TBL_REQUISITION R ON D.INITIAL = R.DIVISION WHERE ReqId = " + TMain.REFORDBILL).FirstOrDefault();
                    if (Division == null)
                        return;
                    string ProdListQry = @"SELECT PL.MCODE, PL.BCODE BC, MI.MENUCODE, MI.DESCA ITEMDESC, PL.UNIT, L.LocationCode BATCH, PL.Quantity OrderQty, 
ISNULL(PICK.PickedQuantity, 0) Quantity, Quantity - ISNULL(PICK.PickedQuantity, 0) VarianceQty, 
ISNULL(PACK.PackedQuantity, 0) REQLQTY_IN, Quantity - ISNULL(PACK.PackedQuantity, 0) AltQty FROM tblPickingList PL
JOIN MENUITEM MI ON MI.MCODE = PL.Mcode
JOIN TBL_LOCATIONS L ON PL.LocationId = L.LocationId
LEFT JOIN
(
	SELECT MCODE, Unit, LocationId, SUM(Quantity) PickedQuantity FROM tblStockInVerificationLog 
	WHERE LEFT(DeviceTrnId,2) = 'PL' AND OrderNo = @ReqId
	GROUP BY MCODE, Unit, LocationId
) PICK ON PL.MCODE = PICK.MCODE AND  PL.Unit = PICK.UNIT AND PL.LocationId = PICK.LocationId
LEFT JOIN
(
	SELECT MCODE, Unit, LocationId, SUM(Quantity) PackedQuantity FROM tblStockInVerificationLog 
	WHERE LEFT(DeviceTrnId,2) = 'PA' AND OrderNo = @ReqId
	GROUP BY MCODE, Unit, LocationId
) PACK ON PL.MCODE = PACK.MCODE AND  PL.Unit = PACK.UNIT AND PL.LocationId = PACK.LocationId
WHERE ReqId = @ReqId
ORDER BY L.LocationCode, MI.DESCA";
                    _AllProdList = con.Query<TrnProd>(ProdListQry, new { ReqId = TMain.REFORDBILL });
                   
                    SetDisplayMode(displayMode);
                    _action = ButtonAction.Loaded;
                }
            }
            catch (Exception Ex)
            {
                while (Ex.InnerException != null)
                    Ex = Ex.InnerException;
                MessageBox.Show(Ex.Message, "Stock Issue Variance", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    TRNMODE = "Cash",
                    BILLTO = GlobalClass.DIVISION,
                    BILLTOADD = Division.INITIAL
                };

                using (SqlConnection conn = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    conn.Open();
                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        main.VCHRNO = conn.ExecuteScalar<string>(
@"IF EXISTS (SELECT * FROM RMD_SEQUENCES WHERE VNAME = @VNAME AND DIV = @DIV)
	SELECT 'TO' + CAST(CurNo AS VARCHAR) FROM RMD_SEQUENCES WHERE VNAME = @VNAME AND DIV = @DIV
ELSE 
	INSERT INTO RMD_SEQUENCES(VNAME, AUTO, Start, CurNo, DIV, DIVISION, VoucherType, VoucherName)
	OUTPUT 'TO' + CAST(inserted.CurNo AS VARCHAR)
	VALUES (@VNAME, 1, 1, 1, @DIV, @DIV, 'TO', 'Branch Transfer')", new { VNAME = "BranchTransfer", DIV = GlobalClass.DIVISION }, tran);


                        conn.Execute(
@"INSERT INTO RMD_TRNMAIN(VCHRNO, DIVISION, CHALANNO, TRNDATE, BSDATE, TRNTIME, TRNUSER, REFORDBILL, BILLTO, BILLTOADD, TRNMODE)
VALUES(@VCHRNO, @DIVISION, @VCHRNO, @TRNDATE, @BSDATE, @TRNTIME, @TRNUSER, @REFORDBILL, @BILLTO, @BILLTOADD, @TRNMODE)", main, tran);

                        conn.Execute(
@"INSERT INTO RMD_TRNPROD (VCHRNO, DIVISION, MCODE, UNIT, Quantity, RealQty, REALQTY_IN, WAREHOUSE, SNO, IDIS, GENERIC)
SELECT @VCHRNO VCHRNO, @DIVISION DIVISION, MCODE, UNIT, SUM(Quantity) Quantity, SUM(Quantity) RealQty, 0 REALQTY_IN, 
L.Warehouse WAREHOUSE, ROW_NUMBER() OVER(ORDER BY MCODE, UNIT, Warehouse, PAckageNo DESC) AS SNO, 0 IDIS, PackageNo GENERIC FROM tblStockInVerificationLog SL 
JOIN TBL_LOCATIONS L ON SL.LocationId = L.LocationId
WHERE LEFT(DeviceTrnId, 2) = 'PA' AND OrderNo = @REFORDBILL
GROUP BY MCODE, UNIT, Warehouse, PackageNo", main, tran);

                        conn.Execute(
@"INSERT INTO RMD_TRNPROD_DETAIL (VCHRNO, DIVISION, PhiscalID, MCODE, UNIT, Warehouse, LocationId, InQty, OutQty, SNO)
SELECT M.VCHRNO, M.DIVISION, M.PhiscalID, L.MCODE, L.Unit, P.WAREHOUSE, L.LocationId, 0, L.Quantity, 0 SNO  FROM [tblStockInVerificationLog] L
JOIN RMD_TRNMAIN M ON L.OrderNo = M.REFORDBILL
JOIN
( 
	SELECT MCODE, VCHRNO, DIVISION, PhiscalID, Unit, WAREHOUSE, SUM(RealQty) RealQty FROM RMD_TRNPROD 
	GROUP BY MCODE, VCHRNO, DIVISION, PhiscalID, Unit, WAREHOUSE
)P ON M.VCHRNO = P.VCHRNO AND M.DIVISION = P.DIVISION AND M.PhiscalID = P.PhiscalID AND L.MCODE = P.MCODE AND L.Unit = P.UNIT
WHERE LEFT(DeviceTrnId, 2) = 'PA' AND L.OrderNo = @REFORDBILL", main, tran);

                        conn.Execute("UPDATE tblPickingList SET [Status] = 1 WHERE ReqId = @REFORDBILL", main, tran);

                        conn.Execute("UPDATE RMD_SEQUENCES SET CURNO = CURNO + 1 WHERE VNAME = @VNAME AND DIV = @DIV", new { VNAME = "BranchTransfer", DIV = GlobalClass.DIVISION }, tran);

                        main.REFORDBILL = main.VCHRNO;
                        main.DIVISION = main.BILLTO = main.BILLTOADD;
                        main.BILLTOADD = GlobalClass.DIVISION;

                        main.VCHRNO = conn.ExecuteScalar<string>(
@"IF EXISTS (SELECT * FROM RMD_SEQUENCES WHERE VNAME = @VNAME AND DIV = @DIV)
	SELECT 'TR' + CAST(CurNo AS VARCHAR) FROM RMD_SEQUENCES WHERE VNAME = @VNAME AND DIV = @DIV
ELSE 
	INSERT INTO RMD_SEQUENCES(VNAME, AUTO, Start, CurNo, DIV, DIVISION, VoucherType, VoucherName)
	OUTPUT 'TR' + CAST(inserted.CurNo AS VARCHAR)
	VALUES (@VNAME, 1, 1, 1, @DIV, @DIV, 'TR', 'Branch Transfer In')", new { VNAME = "BTransferIn", DIV = main.DIVISION }, tran);


                        conn.Execute(
@"INSERT INTO RMD_TRNMAIN(VCHRNO, DIVISION, CHALANNO, TRNDATE, BSDATE, TRNTIME, TRNUSER, REFORDBILL, BILLTO, BILLTOADD, TRNMODE)
VALUES(@VCHRNO, @DIVISION, @VCHRNO, @TRNDATE, @BSDATE, @TRNTIME, @TRNUSER, @REFORDBILL, @BILLTO, @BILLTOADD, @TRNMODE)", main, tran);

                        main.REFORDBILL = TMain.REFORDBILL;
                        conn.Execute(
@"INSERT INTO RMD_TRNPROD (VCHRNO, DIVISION, MCODE, UNIT, Quantity, RealQty, REALQTY_IN, WAREHOUSE, SNO, IDIS)
SELECT @VCHRNO VCHRNO, @DIVISION DIVISION, MCODE, UNIT, SUM(Quantity) Quantity, 0 RealQty, SUM(Quantity) REALQTY_IN, 
(SELECT TOP 1 NAME FROM RMD_WAREHOUSE WHERE DIVISION = @DIVISION) WAREHOUSE,
ROW_NUMBER() OVER(ORDER BY MCODE, UNIT DESC) AS SNO, 0 IDIS FROM tblStockInVerificationLog SL 
WHERE LEFT(DeviceTrnId, 2) = 'PA' AND OrderNo = @REFORDBILL
GROUP BY MCODE, UNIT", main, tran);

                        conn.Execute("UPDATE RMD_SEQUENCES SET CURNO = CURNO + 1 WHERE VNAME = @VNAME AND DIV = @DIV", new { VNAME = "BTransferIn", DIV = main.DIVISION }, tran);

                        tran.Commit();
                        MessageBox.Show("Stock successfully Transfered");
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

    
}
