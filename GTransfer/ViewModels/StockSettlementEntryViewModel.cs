using GTransfer.Library;
using GTransfer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dapper;

namespace GTransfer.ViewModels
{
    class StockSettlementEntryViewModel : BaseViewModel
    {
        #region member and properties
        private TSettlementMode _SelectedSettlementMode;

        public TSettlementMode SelectedSettlementMode
        {
            get { return _SelectedSettlementMode; }
            set { _SelectedSettlementMode = value; OnPropertyChanged("SelectedSettlementMode"); }
        }

        private decimal _TQuantity;

        public decimal TQuantity
        {
            get { return _TQuantity; }
            set { _TQuantity = value; OnPropertyChanged("TQuantity"); }
        }
        private TrnProd _TrnProdObj;
        public TrnProd TrnProdObj { get { return _TrnProdObj; } set { _TrnProdObj = value; OnPropertyChanged("TrnProdObj"); } }

        private string _SelectedWarehouse;

        public string SelectedWarehouse
        {
            get { return _SelectedWarehouse; }
            set { _SelectedWarehouse = value; OnPropertyChanged("SelectedWarehouse"); }
        }
        private TrnProd _selectedTrnProd;

        public TrnProd SelectedTrnProd
        {
            get { return _selectedTrnProd; }
            set { _selectedTrnProd = value; OnPropertyChanged("selectedTrnProd"); }
        }
        // private TrnMain _TrnmainSaveAndLoadObj;
        // public TrnMain TrnmainSaveAndLoadObj { get { return _TrnmainSaveAndLoadObj; } set { _TrnmainSaveAndLoadObj = value; OnPropertyChanged("TrnmainSaveAndLoadObj"); } }
        public TrnMain _TrnMainBaseModelObj;
        public TrnMain TrnMainBaseModelObj
        {
            get { return _TrnMainBaseModelObj; }
            set
            {

                _TrnMainBaseModelObj = value; OnPropertyChanged("TrnMainBaseModelObj");
            }
        }
        //private Division _selectedDivision;
        //public Division selectedDivision
        //{
        //    get
        //    {
        //        if (_selectedDivision == null)
        //        { _selectedDivision = _divisions.Where(x => x.INITIAL == TrnMainBaseModelObj.DIVISION).FirstOrDefault(); }
        //        return _selectedDivision;
        //    }
        //    set
        //    {
        //        _selectedDivision = value;
        //        if (_selectedDivision != null) { TrnMainBaseModelObj.DIVISION = _selectedDivision.INITIAL; }
        //        OnPropertyChanged("selectedDivision");
        //    }
        //}
        public string _BARCODE;

        public string BARCODE
        {
            get { return _BARCODE; }
            set
            {
                _BARCODE = value;
                OnPropertyChanged("BARCODE");
            }
        }
        public Product _productObj;

        public Product productObj
        {
            get { return _productObj; }
            set { _productObj = value; OnPropertyChanged("productObj"); }
        }
        public string _SelectedProduct;
        public string SelectedProductMCODE
        {
            get { return _SelectedProduct; }
            set
            {
                if (_SelectedProduct == value) return;
                _SelectedProduct = value;
                if (value != null)
                {
                    changeItemEvent();
                }
                OnPropertyChanged("SelectedProductMCODE");

            }
        }
        private AlternateUnit _selectedAltUnit;
        public AlternateUnit SelectedAltUnit
        {
            get { return _selectedAltUnit; }
            set
            {
                _selectedAltUnit = value; OnPropertyChanged("SelectedAltUnit");
                if (value != null)
                {
                    if (TrnProdObj == null)
                    {
                        TrnProdObj = new TrnProd();
                        TrnProdObj.PropertyChanged += TrnProdObj_PropertyChanged;
                    }
                    TrnProdObj.RATE = _selectedAltUnit.RATE;
                    TrnProdObj.REALRATE = TrnProdObj.RATE;
                    TrnProdObj.UNIT = _selectedAltUnit.ALTUNIT;

                }
            }
        }

        private void TrnProdObj_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RATE" || e.PropertyName == "Quantity") { TrnProdObj.CalculateNormal(); }
        }
        #endregion

        #region Observablecollection
        private ObservableCollection<Division> _divisions;
        public ObservableCollection<Division> Divisions
        {
            get { return _divisions; }
            set { _divisions = value; OnPropertyChanged("Divisions"); }
        }
        private ObservableCollection<Item> _ProductList;

        public ObservableCollection<Item> ProductList
        {
            get { return _ProductList; }
            set { _ProductList = value; OnPropertyChanged("ProductList"); }
        }
        private ObservableCollection<TWarehouse> _WarehouseList;

        public ObservableCollection<TWarehouse> WarehouseList
        {
            get { return _WarehouseList; }
            set { _WarehouseList = value; OnPropertyChanged("WarehouseList"); }
        }
        private ObservableCollection<TSettlementMode> _SettlementModeList;

        public ObservableCollection<TSettlementMode> SettlementModeList
        {
            get { return _SettlementModeList; }
            set { _SettlementModeList = value; OnPropertyChanged("SettlementModeList"); }
        }
        #endregion


        #region Relaycommand
        //  public RelayCommand ItemCodeFocusLostEvent { get; set; }
        //  public RelayCommand BarcodeFocusLostEvent { get; set; }
        public RelayCommand LoadGridData { get; set; }
        public RelayCommand BarcodeChangeCommand { get { return new RelayCommand(ExecuteBarcodeChangeCommand); } }

        private void ExecuteBarcodeChangeCommand(object obj)
        {
            GetProductFromBarcode();
        }

        #endregion

        #region constructor
        public StockSettlementEntryViewModel()
        {
            EditVisible = true;
            DeleteVisible = true;
            AddCommand = new RelayCommand(ExecuteAdd);
            LoadGridData = new RelayCommand(ExecuteGridLoad);
            //  ItemCodeFocusLostEvent = new RelayCommand(ExecuteItemCodeFocusLostEvent);
            //  BarcodeFocusLostEvent = new RelayCommand(ExecuteBarcodeFocusLostEvent);

            GetComboBoxList();


        }




        #endregion

        #region forTabNavigation
        private string _ItemCodeFocus;
        public string ItemCodeFocus { get { return _ItemCodeFocus; } set { _ItemCodeFocus = value; OnPropertyChanged("ItemCodeFocus"); } }

        private string _UnitFocus;
        public string UnitFocus { get { return _UnitFocus; } set { _UnitFocus = value; OnPropertyChanged("UnitFocus"); } }

        private bool _RateFocus;
        public bool RateFocus { get { return _RateFocus; } set { _RateFocus = value; OnPropertyChanged("RateFocus"); } }

        private void isFocusable()
        {
            ItemCodeFocus = "Local";
            UnitFocus = "Local";
            RateFocus = true;
        }
        //private void ExecuteItemCodeFocusLostEvent(object obj)
        //{
        //    isFocusable();
        //    if (!string.IsNullOrEmpty(SelectedProductMCODE))
        //    {


        //        if (_SetOnlyDefaultUnitInPurcahse == false)
        //        {
        //            if (productObj.AlternateUnits.Count == 1)
        //            {
        //                UnitFocus = "None";
        //            }
        //        }
        //        else { UnitFocus = "None"; }
        //        if (_PrateEdit == false)
        //        {
        //            RateFocus = false;
        //        }
        //    }

        //}

        //private void ExecuteBarcodeFocusLostEvent(object obj)
        //{
        //    isFocusable();
        //    if (!string.IsNullOrEmpty(BARCODE))
        //    {
        //        ItemCodeFocus = "None";


        //        if (_SetOnlyDefaultUnitInPurcahse == false)
        //        {
        //            if (productObj.AlternateUnits.Count == 1)
        //            {
        //                UnitFocus = "None";
        //            }
        //        }
        //        else { UnitFocus = "None"; }
        //        if (_PrateEdit == false)
        //        {
        //            RateFocus = false;
        //        }
        //    }
        //}
        #endregion
        #region execute command



        private void ExecuteGridLoad(object obj)
        {
            if (SelectedTrnProd != null)
            {
                TrnProdObj = GParse.CopyPropertyValues(SelectedTrnProd, new TrnProd()) as TrnProd;
                SelectedProductMCODE = TrnProdObj.MCODE;
                SelectedAltUnit = _productObj.AlternateUnits.Where(x => x.ALTUNIT == SelectedTrnProd.UNIT).FirstOrDefault();
                SelectedWarehouse = TrnProdObj.WAREHOUSE;
                _BARCODE = TrnProdObj.BC;
                OnPropertyChanged("BARCODE");
                TrnMainBaseModelObj.ProdList.Remove(SelectedTrnProd);
            }

        }

        public virtual void ExecuteAdd(object obj)
        {
            if (TrnProdObj == null) return;
            if (string.IsNullOrEmpty(SelectedProductMCODE))
            {
                MessageBox.Show("please select the item first to add...", "Alert Message", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                // FocusedElement = (short)FocusElements.ItemCode;

                return;
            }

            if (TrnProdObj.Quantity <= 0)
            {
                MessageBox.Show("Please enter the valid Quantity for the item", "Alert Message", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                //  FocusedElement = (short)FocusElements.quantity;

                return;
            }

            if (string.IsNullOrEmpty(SelectedWarehouse))
            {
                MessageBox.Show("Please select the warehouse from the list", "Alert Message", MessageBoxButton.OK, MessageBoxImage.Error);
                //FocusedElement = (short)FocusElements.warehouse;
                return;
            }

            if (TrnMainBaseModelObj.ProdList.Any((item) => item.MCODE == SelectedProductMCODE))
            {
                var product = TrnMainBaseModelObj.ProdList.Where((item) => item.MCODE == SelectedProductMCODE);
                if (product.Any((i) => i.UNIT == SelectedAltUnit.ALTUNIT))
                {
                    MessageBox.Show("Item  is already in the Table below", "Duplicate Data", MessageBoxButton.OK, MessageBoxImage.Error);
                    // FocusedElement = (short)FocusElements.ItemCode;
                    return;
                }
            }



            TrnProdObj.WAREHOUSE = SelectedWarehouse;
            TrnProdObj.UNIT = SelectedAltUnit.ALTUNIT;
            TrnProdObj.VoucherType = VoucherTypeEnum.StockSettlement;
            TrnProdObj.REALRATE = SelectedAltUnit.CONFACTOR * TrnProdObj.RATE;
            TrnProdObj.PRATE = productObj.PRATE_A;
            TrnMainBaseModelObj.ProdList.Add(TrnProdObj);
            TrnMainBaseModelObj.ReCalculateBill();
            // GetMultiWarehousePerTransactionSetting(TrnMainBaseModelObj.ProdList, SelectedWarehouse);
            TrnProdObj = null;
            SelectedProductMCODE = null;
            SelectedAltUnit = null;
            BARCODE = null;
            //SelectedWarehouse = null;
            // FocusedElement = (short)FocusElements.ItemCode;

        }
        #endregion

        #region override method
        public override string Tmode
        {
            get
            {
                return base.Tmode;
            }
            set
            {
                base.Tmode = value;
                TrnMainBaseModelObj.Mode = value;


            }
        }
        public override void DeleteMethod(object obj)
        {
            if (MessageBox.Show("You are about to delete current Stock Settlement.Once you Delete you cannot recover it back. Do you want to proceed?", "Delete Data", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                con.Open();
                string que;
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    que = @"Delete from RMD_TRNPROD where Vchrno=@VCHRNO and Division =@DIVISION AND PHISCALID = @PHISCALID;";
                    con.Execute(que, TrnMainBaseModelObj, tran);
                    que = @"Delete from RMD_TRNMAIN where Vchrno=@VCHRNO and Division =@DIVISION AND PHISCALID = @PHISCALID;";
                    con.Execute(que, TrnMainBaseModelObj, tran);
                    que = @"Delete from RMD_TRNPROD_DETAIL where VCHRNO=@VCHRNO and DIVISION=@DIVISION AND PHISCALID = @PHISCALID;";
                    con.Execute(que, TrnMainBaseModelObj, tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show(ex.Message);
                }
            }
            MessageBox.Show("StockSettlement is Deleted Successfully...");

            ExecuteUndo(null);
        }
        public override void NewMethod(object obj)
        {

            UndoMethod(null);
            string Div;

            Tmode = "NEW";
            TrnMainBaseModelObj.DIVISION = string.IsNullOrEmpty(TrnMainBaseModelObj.DIVISION) ? GlobalClass.DIVISION : TrnMainBaseModelObj.DIVISION;            
            TrnMainBaseModelObj.TRNDATE = DateTime.Now;
            TrnProdObj = new TrnProd();
            TrnProdObj.PropertyChanged += TrnProdObj_PropertyChanged;
            string vnum = string.Empty;
            string vno = string.Empty;
            string chalanNo = string.Empty;
            GlobalClass.GetBillSequences("StockSettlement", TrnMainBaseModelObj.VoucherPrefix, TrnMainBaseModelObj.VoucherName, TrnMainBaseModelObj.VoucherPrefix, ref vno, ref chalanNo, ref vnum);
            TrnMainBaseModelObj.VCHRNO = vno;
            TrnMainBaseModelObj.VNUM = vnum;
            SelectedWarehouse = GlobalClass.Warehouse;
            //FocusedElement = (short)FocusElements.RefNo;
        }


        public override bool UndoMethod(object obj)
        {

            TrnMainBaseModelObj = new TrnMain() { DIVISION = GlobalClass.DIVISION };
            TrnMainBaseModelObj.VoucherPrefix = "ST";
            TrnMainBaseModelObj.VoucherType = VoucherTypeEnum.StockSettlement;
            TrnMainBaseModelObj.ProdList = new ObservableCollection<TrnProd>();
            TrnMainBaseModelObj.ProdList.CollectionChanged += ProdList_CollectionChanged;
            // if (_action != ButtonAction.Selected) { vn = 0; }
            TrnProdObj = null;
            TQuantity = 0;
            SelectedSettlementMode = null;
            SelectedProductMCODE = null;
            SelectedAltUnit = null;
            BARCODE = null;
            SelectedWarehouse = null;
            return false;
        }
        public override void SaveMethod(object obj)
        {
            if (SelectedSettlementMode == null)
            {
                MessageBox.Show("Please select the settlement mode to proceed...");
                // FocusedElement = (short)FocusElements.settlementmode;
                return;
            }

            if (MessageBox.Show("You are about to save current operation. Do you want to proceed?", "Save Data", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            //TrnmainSaveAndLoadObj = new TrnmainSaveAndLoad(TrnMainBaseModelObj);
            //  TrnmainSaveAndLoadObj.DoSaveProd = true;



            //For TrnUser and EditedUser

            if (string.IsNullOrEmpty(TrnMainBaseModelObj.TRNUSER))
            {
                TrnMainBaseModelObj.TRNUSER = GlobalClass.CurrentUser.UNAME;
            }
            else
            {
                TrnMainBaseModelObj.EditUser = GlobalClass.CurrentUser.UNAME;
            }

            //  TrnMainBaseModelObj.TERMINAL = GlobalClass.TERMINAL;
            TrnMainBaseModelObj.EDATE = TrnMainBaseModelObj.TRNDATE = DateTime.Today;
            TrnMainBaseModelObj.TRN_DATE = DateTime.Today;
            TrnMainBaseModelObj.TRNMODE = SelectedSettlementMode.DESCRIPTION;
            TrnMainBaseModelObj.TRNTIME = DateTime.Now.ToString("hh:mm tt");
            TrnMainBaseModelObj.PhiscalID = GlobalClass.CurFiscalYear.ToString();
            foreach (var tp in TrnMainBaseModelObj.ProdList)
            {
                tp.VCHRNO = TrnMainBaseModelObj.VCHRNO;
                tp.DIVISION = TrnMainBaseModelObj.DIVISION;
                tp.PhiscalID = TrnMainBaseModelObj.PhiscalID;
                if (SelectedSettlementMode.DECREASE == 0)
                {
                    tp.REALQTY_IN = (tp.REALRATE / tp.RATE) * tp.Quantity;
                    tp.ALTQTY_IN = tp.Quantity;
                    tp.RealQty = 0;
                    tp.AltQty = 0;

                }
                else if (SelectedSettlementMode.DECREASE == 1)
                {
                    tp.RealQty = (tp.REALRATE / tp.RATE) * tp.Quantity;
                    tp.AltQty = tp.Quantity;
                    tp.REALQTY_IN = 0;
                    tp.ALTQTY_IN = 0;
                }
                else { MessageBox.Show("Error in selectedSettlement mode... "); return; }
            }
            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    if (Tmode == "EDIT")
                    {

                        string que;

                        que = @"Delete from RMD_TRNPROD where Vchrno=@VCHRNO and Division =@DIVISION AND PHISCALID = @PHISCALID;";
                        con.Execute(que, TrnMainBaseModelObj, tran);
                        que = @"Delete from RMD_TRNMAIN where Vchrno=@VCHRNO and Division =@DIVISION AND PHISCALID = @PHISCALID;";
                        con.Execute(que, TrnMainBaseModelObj, tran);
                        que = @"Delete from RMD_TRNPROD_DETAIL where VCHRNO=@VCHRNO and DIVISION=@DIVISION AND PHISCALID = @PHISCALID;";
                        con.Execute(que, TrnMainBaseModelObj, tran);

                    }
                    if (Tmode == "NEW")
                    {
                        string vno = string.Empty;
                        string chalanNo = string.Empty;
                        string vnum = string.Empty;
                        GlobalClass.GetBillSequences("StockSettlement", TrnMainBaseModelObj.VoucherPrefix, TrnMainBaseModelObj.VoucherName, TrnMainBaseModelObj.VoucherPrefix, ref vno, ref chalanNo, ref vnum);
                        TrnMainBaseModelObj.VCHRNO = vno;
                        TrnMainBaseModelObj.VNUM = vnum;
                    }
                    string qstring = "insert into RMD_TRNMAIN (VCHRNO,DIVISION,CHALANNO,TRNDATE,BSDATE,TRNTIME,TOTAMNT,DCAMNT,DCRATE,VATAMNT,NETAMNT,ADVANCE,TRNMODE,BILLTO,BILLTOADD,TRNUSER,VATBILL,DELIVERYDATE,DELIVERYTIME,ORDERS,REFORDBILL,INDDIS,CREBATE,CREDIT,DUEDATE,TRNAC,PARAC,PARTRNAMNT,RETTO,REFBILL,CHEQUENO,CHEQUEDATE,REMARKS,POST,POSTUSER,FPOSTUSER,TERMINAL,SHIFT,EXPORT,CHOLDER,CARDNO,EditUser,MEMBERNO,MEMBERNAME,EDITED,TAXABLE,NONTAXABLE,VMODE,BILLTOTEL,BILLTOMOB,TRN_DATE,BS_DATE,STAX,TOTALCASH,TOTALCREDIT,TOTALCREDITCARD,TOTALGIFTVOUCHER,TENDER,CHANGE,CardTranID,ReturnVchrID,TranID,VoucherStatus,VoucherType,PRESCRIBEBY,DISPENSEBY,RECEIVEBY,Edate,STATUS,CONFIRMEDBY,CONFIRMEDTIME,PhiscalID,Stamp,ROUNDOFF,Customer_Count,DBUSER,HOSTID)" +
                              "Values(@VCHRNO,@DIVISION,@CHALANNO,@TRNDATE,@BSDATE,@TRNTIME,@TOTAMNT,@DCAMNT,@DCRATE,@VATAMNT,@NETAMNT,@ADVANCE,@TRNMODE,@BILLTO,@BILLTOADD,@TRNUSER,@VATBILL,@DELIVERYDATE,@DELIVERYTIME,@ORDERS,@REFORDBILL,@INDDIS,@CREBATE,@CREDIT,@DUEDATE,@TRNAC,@PARAC,@PARTRNAMNT,@RETTO,@REFBILL,@CHEQUENO,@CHEQUEDATE,@REMARKS,@POST,@POSTUSER,@FPOSTUSER,@TERMINAL,@SHIFT,@EXPORT,@CHOLDER,@CARDNO,@EditUser,@MEMBERNO,@MEMBERNAME,@EDITED,@TAXABLE,@NONTAXABLE,@VMODE,@BILLTOTEL,@BILLTOMOB,@TRN_DATE,@BS_DATE,@STAX,@TOTALCASH,@TOTALCREDIT,@TOTALCREDITCARD,@TOTALGIFTVOUCHER,@TENDER,@CHANGE,@CardTranID,@ReturnVchrID,@TranID,@VoucherStatus,@VoucherType,@PRESCRIBEBY,@DISPENSEBY,@RECEIVEBY,@Edate,@STATUS,@CONFIRMEDBY,@CONFIRMEDTIME,@PhiscalID,@Stamp,@ROUNDOFF,@Customer_Count,@DBUSER,@HOSTID)";

                    con.Execute(qstring, TrnMainBaseModelObj, tran);
                    string qstring2 = @"insert into RMD_TRNPROD (VCHRNO,CHALANNO,DIVISION,MCODE,UNIT,Quantity,RealQty,AltQty,RATE,AMOUNT,DISCOUNT,VAT,REALRATE,EXPORT,IDIS,OPQTY,REALQTY_IN,ALTQTY_IN,WAREHOUSE,REFBILLQTY,SPRICE,EXPDATE,REFOPBILL,ALTUNIT,TRNAC,PRATE,VRATE,ALTRATE,VPRATE,VSRATE,TAXABLE,NONTAXABLE,INVRATE,EXRATE,NCRATE,CRATE,SNO,CUSTOM,WEIGHT,DRATE,CARTON,INVOICENO,ISSUENO,BC,GENERIC,BATCH,MFGDATE,MANUFACTURER,SERVICETAX,BCODEID,VoucherType,SALESMANID,PhiscalID,STAMP)" +
                 "VALUES('" + TrnMainBaseModelObj.VCHRNO + "', @CHALANNO, @DIVISION, @MCODE, @UNIT, @Quantity, @RealQty, @AltQty, @RATE, @AMOUNT, @DISCOUNT,@VAT,@REALRATE,@EXPORT,@IDIS,@OPQTY,@REALQTY_IN,@ALTQTY_IN,@WAREHOUSE,@REFBILLQTY,@SPRICE,@EXPDATE,@REFOPBILL,@ALTUNIT,@TRNAC,@PRATE,@VRATE,@ALTRATE,@VPRATE,@VSRATE,@TAXABLE,@NONTAXABLE,@INVRATE,@EXRATE,@NCRATE,@CRATE,@SNO,@CUSTOM,@WEIGHT,@DRATE,@CARTON,@INVOICENO,@ISSUENO,@BC,@GENERIC,@BATCH,@MFGDATE,@MANUFACTURER,@SERVICETAX,@BCODEID,@VoucherType,@SALESMANID,@PhiscalID,@STAMP)";
                    int RowsEffected = con.Execute(qstring2, TrnMainBaseModelObj.ProdList, tran);

                    foreach (var tp in TrnMainBaseModelObj.ProdList)
                    {
                        string LocationId = con.ExecuteScalar<string>("SELECT LocationId FROM TBL_LOCATIONS WHERE Warehouse = '" + tp.WAREHOUSE + "' AND LocationCode LIKE '" + SelectedSettlementMode.LocationPrefix + "%'", transaction: tran);
                        string prod_detail_query = @"INSERT INTO RMD_TRNPROD_DETAIL(VCHRNO, DIVISION, PhiscalID, MCODE, UNIT, Warehouse, LocationId, InQty, OutQty, SNO)VALUES(@VCHRNO, @DIVISION, @PhiscalID, @MCODE, @UNIT, @Warehouse, @LocationId, @InQty, @OutQty, @SNO)";
                        con.Execute(prod_detail_query, new { VCHRNO = TrnMainBaseModelObj.VCHRNO, DIVISION = TrnMainBaseModelObj.DIVISION, PhiscalID = TrnMainBaseModelObj.PhiscalID, MCODE = tp.MCODE, UNIT = tp.UNIT, Warehouse = tp.WAREHOUSE, LocationId = LocationId, InQty = tp.REALQTY_IN, OutQty = tp.RealQty, SNO = 0 }, tran);
                    }

                    if (Tmode == "NEW")
                    {
                        con.Execute("UPDATE RMD_SEQUENCES SET CURNO = CURNO + 1 WHERE VNAME = @VNAME AND DIVISION = @DIVISION", new { VNAME = "StockSettlement", DIVISION = TrnMainBaseModelObj.DIVISION }, tran);

                    }
                    tran.Commit();
                    SetAction(ButtonAction.Init);
                    MessageBox.Show("Records are saved sucessfully...");
                    UndoMethod(null);
                }
                catch (Exception e)
                {
                    tran.Rollback(); MessageBox.Show("Error :" + e);
                }

            }

            //bool IsSaved = TrnmainSaveAndLoadObj.SaveData().status == "ok";
            //if (IsSaved == true)
            //{
            //    SetAction(ButtonAction.Init);
            //    voucharList = null;
            //    MessageBox.Show("Records are saved sucessfully...");
            //}
        }

        public override void LoadMethod(object obj)
        {
            string vchrno = TrnMainBaseModelObj.VCHRNO = "ST" + TrnMainBaseModelObj.VNUM;
            string division = string.IsNullOrEmpty(TrnMainBaseModelObj.DIVISION) ? GlobalClass.DIVISION : TrnMainBaseModelObj.DIVISION;
            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                con.Open();
                string query = @"Select * from rmd_trnmain A  where VCHRNO=@VCHRNO AND DIVISION = @DIVISION AND PHISCALID = @PHISCALID;" +
                                        @"Select A.*,B.MENUCODE,B.DESCA  ITEMDESC,B.Ptype from RMD_TRNPROD A INNER JOIN MENUITEM B ON A.MCODE=B.MCODE  where A.VCHRNO=@VCHRNO AND A.DIVISION = @DIVISION AND A.PHISCALID = @PHISCALID ;";
                var Multiresult = con.QueryMultiple(query, new { VCHRNO = vchrno, DIVISION = division, PhiscalID = GlobalClass.CurFiscalYear.ToString() });
                var Mn = Multiresult.Read<TrnMain>().SingleOrDefault();
                var Prod = Multiresult.Read<TrnProd>().ToList();
                if (Mn != null)
                {

                    TrnMainBaseModelObj = Mn;
                    TrnMainBaseModelObj.VNUM = TrnMainBaseModelObj.VCHRNO.Substring(2);
                    TrnMainBaseModelObj.VoucherPrefix = "ST";
                    TrnMainBaseModelObj.VoucherType = VoucherTypeEnum.StockSettlement;
                    TrnMainBaseModelObj.ProdList = new ObservableCollection<TrnProd>(Prod);
                    TrnMainBaseModelObj.ProdList.CollectionChanged += ProdList_CollectionChanged;
                    if (!string.IsNullOrEmpty(TrnMainBaseModelObj.TRNMODE))
                    {
                        SelectedSettlementMode = SettlementModeList.Single(x => x.DESCRIPTION == TrnMainBaseModelObj.TRNMODE);
                    }
                    TQuantity = TrnMainBaseModelObj.ProdList.Sum(x => x.Quantity);
                    TrnMainBaseModelObj.TOTAMNT = TrnMainBaseModelObj.ProdList.Sum(x => x.AMOUNT);

                    OnPropertyChanged("SelectedDivision");
                    TrnMainBaseModelObj.PropertyChanged += TrnMainBaseModelObj_PropertyChanged;

                    SetAction(ButtonAction.Selected);
                    Tmode = "LOADED";

                }
                else
                {
                    MessageBox.Show("Invalid Vouchar Number.Please enter valid vouchar number");

                    return;
                }
            }
        }



        #endregion

        #region ChangeEvent
        public void ProdList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            TQuantity = TrnMainBaseModelObj.ProdList.Sum(x => x.Quantity);
            TrnMainBaseModelObj.TOTAMNT = TrnMainBaseModelObj.ProdList.Sum(x => x.AMOUNT);
        }
        public void TrnMainBaseModelObj_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_action == ButtonAction.Selected && e.PropertyName == "VNUM")
            {
                ExecuteUndo(null);
            }
        }
        #endregion

        #region methods
        private void changeItemEvent(bool IsbarcodeChangeEvent = false)
        {

            _productObj = new Product(SelectedProductMCODE, true);
            if (_productObj.AlternateUnits != null && _productObj.AlternateUnits.Count > 0) SelectedAltUnit = _productObj.AlternateUnits.First();
            _BARCODE = _productObj.BARCODE;

            if (TrnProdObj == null)
            {
                TrnProdObj = new TrnProd();
                TrnProdObj.PropertyChanged += TrnProdObj_PropertyChanged;
            }
            TrnProdObj.MENUCODE = _productObj.MENUCODE;
            TrnProdObj.ITEMDESC = _productObj.DESCA;
            TrnProdObj.MCODE = _productObj.MCODE;

            TrnProdObj.ALTUNIT = _productObj.BASEUNIT;
            TrnProdObj.UNIT = SelectedAltUnit.ALTUNIT;
            // TrnProdObj.RATE = _productObj.RATE_A;
            TrnProdObj.REALRATE = TrnProdObj.RATE;
            TrnProdObj.ISVAT = _productObj.VAT;
            TrnProdObj.ISSERVICECHARGE = _productObj.HASSERVICECHARGE;

            OnPropertyChanged("productObj");
            OnPropertyChanged("BARCODE");

            if (IsbarcodeChangeEvent == false)
            {
                TrnProdObj.BC = _BARCODE;
            }
        }
        public void GetProductFromBarcode()
        {
            try
            {
                if (!string.IsNullOrEmpty(TrnProdObj.BC))
                {
                    using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
                    {
                        var result = con.Query("SELECT A.BCODE,A.MCODE,A.UNIT,A.ISSUENO,A.EDATE,A.BCODEID,A.SUPCODE,A.BATCHNO,A.EXPIRY,A.REMARKS,A.INVNO,A.DIV,A.FYEAR,A.SRATE,B.DESCA FROM BARCODE A inner join Menuitem B on A.mcode=B.mcode WHERE A.BCODE='" + TrnProdObj.BC + "'").FirstOrDefault();
                        if (result == null) { MessageBox.Show("Invalid Barcode"); return; }
                        SelectedProductMCODE = result.MCODE;
                        changeItemEvent(true);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }
        private void GetComboBoxList()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    con.Open();
                    ProductList = new ObservableCollection<Item>(con.Query<Item>("select MENUCODE,MCODE,DESCA,BASEUNIT UNIT,PRATE_A PRATE,RATE_A RATE FROM MENUITEM WHERE TYPE='A'"));
                    WarehouseList = new ObservableCollection<TWarehouse>(con.Query<TWarehouse>("select NAME from RMD_WAREHOUSE WHERE DIVISION = '" + GlobalClass.DIVISION + "'"));
                    SettlementModeList = new ObservableCollection<TSettlementMode>(con.Query<TSettlementMode>("select * from settlementmode"));
                    Divisions = new ObservableCollection<Division>(con.Query<Division>("select * from division"));
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        #endregion
    }
}
