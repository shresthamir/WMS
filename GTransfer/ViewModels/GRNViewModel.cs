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
    class GRNViewModel : BaseViewModel
    {
        private TrnMain _TMain;
        private ObservableCollection<TrnProd> _ProdList;
        private string _SupplierName;

        public TrnMain TMain { get { return _TMain; } set { _TMain = value; OnPropertyChanged("TMain"); } }
        public RelayCommand OrderReferenceCommand { get { return new RelayCommand(ExecuteOrderReferenceCommand); } }
        public ObservableCollection<TrnProd> ProdList { get { return _ProdList; } set { _ProdList = value; OnPropertyChanged("ProdList"); } }
        public string SupplierName { get { return _SupplierName; } set { _SupplierName = value; OnPropertyChanged("SupplierName"); } }
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
            return false;
        }

        private void ExecuteOrderReferenceCommand(object obj)
        {
            if (string.IsNullOrEmpty(TMain.REFORDBILL))
                return;
            try
            {
                using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    var orderTran = con.Query<TrnMain>(@"SELECT TRNAC, ACNAME PARAC FROM RMD_TRNMAIN T JOIN RMD_ACLIST A ON T.TRNAC = A.ACID
                                    WHERE LEFT(VCHRNO, 2) = 'PO' AND VCHRNO = '" + TMain.REFORDBILL + "' AND DIVISION = '" + GlobalClass.DIVISION + "'").FirstOrDefault();
                    if (orderTran == null)
                    {
                        MessageBox.Show("Invalid order no.", "GRN", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                    TMain.PARAC = orderTran.TRNAC;
                    SupplierName = orderTran.PARAC;
                    string ProdListQry = @"SELECT B.MCODE, MI.MENUCODE, MI.DESCA ITEMDESC, MI.PRATE_A REALRATE,  MI.PRATE_A RATE, B.UNIT, LOC.Warehouse, ISNULL(OP.QUANTITY, 0) OrderQty, ISNULL(SUM(L.QUANTITY), 0) Quantity , ISNULL(SUM(L.QUANTITY), 0) REALQTY_IN, MI.RATE_A SRATE, MI.VAT ISVAT FROM
                                            (
                                                SELECT DISTINCT * FROM
	                                            (
                                                    SELECT MCODE, UNIT FROM RMD_ORDERPROD WHERE VCHRNO = @OrderNo
                                                    UNION ALL
                                                    SELECT MCODE, UNIT FROM tblStockInVerificationLog WHERE OrderNo = @OrderNo

                                                ) A
                                            ) B 
                                            JOIN MENUITEM MI ON MI.MCODE = B.MCODE
                                            LEFT JOIN RMD_ORDERPROD OP ON OP.MCODE = B.MCODE AND OP.UNIT = B.UNIT
                                            LEFT JOIN tblStockInVerificationLog L ON L.MCODE = B.MCODE AND L.UNIT = B.UNIT
                                            JOIN TBL_LOCATIONS LOC ON l.LocationId = LOC.LocationId
                                            GROUP BY B.MCODE, B.UNIT, OP.QUANTITY, LOC.Warehouse, MI.MENUCODE, MI.DESCA, MI.PRATE_A, MI.RATE_A, MI.VAT";
                    ProdList = new ObservableCollection<TrnProd>(con.Query<TrnProd>(ProdListQry, new { OrderNo = TMain.REFORDBILL }));
                    foreach(TrnProd tpod in ProdList)
                    {
                        tpod.REALQTY_IN = tpod.Quantity;
                        tpod.REALRATE = tpod.RATE;
                        tpod.AMOUNT = tpod.Quantity * tpod.RATE;
                        tpod.TAXABLE = tpod.AMOUNT - tpod.NONTAXABLE;
                        if (tpod.ISVAT == 1)
                            tpod.VAT = tpod.TAXABLE * Settings.VatRate;
                        tpod.NETAMOUNT = tpod.TAXABLE + tpod.VAT;
                        tpod.VarianceQty = tpod.Quantity - tpod.OrderQty;
                    }
                    TMain.TOTAMNT  = ProdList.Sum(x => x.AMOUNT);
                    TMain.TAXABLE = ProdList.Sum(x => x.TAXABLE);
                    TMain.VATAMNT = ProdList.Sum(x => x.VAT);
                    TMain.NETAMNT = ProdList.Sum(x => x.NETAMOUNT);
                }
            }
            catch (Exception ex) { MessageBox.Show("Error..." + ex); }
        }
    }
}
