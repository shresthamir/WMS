using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTransfer.Models;
using GTransfer.Library;
using System.Collections.ObjectModel;

namespace GTransfer.Models
{
    public class TTrnMain : BaseModel
    {
        #region Members
        //private int _TranId;
        string _BSDATE;
        private DateTime _TRNDATE;
        private string _TRNTIME;
        private decimal _TOTAMNT = 0;
        private decimal _DCAMNT = 0;
        protected decimal _DCRATE = 0;
        private decimal _VATAMNT = 0;
        private decimal _NETAMNT = 0;
        private string _TRNMODE = "";
        private string _BILLTO = "";
        private string _TRNUSER = "";
        private string _TRNAC;
        private string _TERMINAL;
        private decimal _TENDER = 0;
        private decimal _CHANGE = 0;
        private string _VCHRNO = "";
        private string _CHALANNO = "";
        private string _DIVISION = "";
        private decimal _TAXABLE = 0;
        private decimal _NONTAXABLE = 0;
        private decimal _ROUNDOFF = 0;
        private decimal _NETWITHOUTROUNDOFF = 0;
        private decimal _ServiceCharge = 0;
        private string _SalesManID;
        private string _WAREHOUSE;
        //private ObservableCollection<TrnProd> _TrnProdList;
        //private ObservableCollection<TrnProd> _AdditionProdList;
        //private ObservableCollection<Trntran> _AdditionTranList;
        //private Discount _BillDiscount = new Discount();
        //private Customer _BillCustomer;
        //private Order_Main _Order;
        private int _VATBILL;
        private Boolean _IsVATBill = false;
        private Boolean _IsTable = false;
        public bool KeepLog = false;
        private string _BILLTOADD = "";
        private string _REMARKS = "";
        private string _VoucherPrefix; // for voucher prefix li 'SI' for sales
        private string _VoucherName; // Voucher name like 'Sales', 'Journal','Purchase',etc
        private string _VoucherAbbName; //voucher abbrevieated name like 'Si for Sales','JV' for Journal, 'PI for purchase etc
        private string _COSTCENTER;
        private int _NextNumber; // next sequence Number
        private VoucherTypeEnum _VoucherType;
        private decimal _ADVANCE = 0;
        private Nullable<DateTime> _DeliveryDate;
        private string _DeliveryTime;
        private byte _ORDERS;
        private string _REFORDBILL;
        private byte _INDDIS;
        private decimal _CREBATE;
        private byte _CREDIT;
        private Nullable<DateTime> _DUEDATE;
        private string _PARAC;
        private decimal _PARTRNAMNT;
        private string _RETTO;
        private string _REFBILL;
        private string _CHEQUENO;
        private Nullable<DateTime> _CHEQUEDATE;
        private Nullable<DateTime> _EDATE;
        private byte _POST;
        private string _POSTUSER;
        private string _FPOSTUSER;

        private string _SHIFT;
        private byte _EXPORT;
        private string _CHOLDER;
        private string _CARDNO;
        private string _EditUser;
        private string _MEMBERNO;
        private string _MEMBERNAME;
        private short _EDITED;

        private byte _VMODE;
        private string _BILLTOTEL;
        private string _BILLTOMOB;
        private Nullable<DateTime> _TRN_DATE;
        private string _BS_DATE;
        private decimal _STAX;
        private decimal _TOTALCASH;
        private decimal _TOTALCREDIT;
        private decimal _TOTALCREDITCARD;
        private decimal _TOTALGIFTVOUCHER;

        private string _CardTranID;
        private string _ReturnVchrID;
        private decimal _TranID;
        private string _VoucherStatus;

        private string _PRESCRIBEBY;
        private string _DISPENSEBY;
        private string _RECEIVEBY;

        private byte _STATUS;
        private string _CONFIRMEDBY;
        private string _CONFIRMEDTIME;
        private string _PhiscalID;
        private float _Stamp;

        private decimal _Customer_Count;
        private string _DBUSER;
        private string _HOSTID;
        //private ObservableCollection<Trntran> _TrntranList;
        //private decimal _TOTALFLATDISCOUNT;
        // private decimal _TOTALPROMOTION;
        //private decimal _TOTALLOYALTY;
        //private decimal _TOTALINDDISCOUNT;
        //private byte _ReplaceIndividualWithFlatDiscount = GlobalSetting.GblReplaceIndividualWithFlat;

        private string _DeliveryAddress;
        private string _MobileNo;
        private decimal _Longitude;
        private decimal _Latitude;
        private string _DeliveryMiti;
        private string _OrderNo;
        private string _ADJWARE;
        private string _BATCHNO;
        private string _BATCHNAME;
        private string _DPSNO;
        private string _EDITTIME;
        private string _EID;
        private string _EXTRACHARGE;
        private string _ISSUEWARE;
        private string _BILLTOPAN;
        private string _VNUM;

        public string WAREHOUSE { get; set; }
        #endregion

        #region Properties

        public string guid { get; set; }
        public string COSTCENTER { get { return _COSTCENTER; } set { _COSTCENTER = value; OnPropertyChanged("COSTCENTER"); } }
        public string RECEIVEBY { get { return _RECEIVEBY; } set { _RECEIVEBY = value; OnPropertyChanged("RECEIVEBY"); } }

        public byte STATUS { get { return _STATUS; } set { _STATUS = value; OnPropertyChanged("STATUS"); } }

        public string CONFIRMEDBY { get { return _CONFIRMEDBY; } set { _CONFIRMEDBY = value; OnPropertyChanged("CONFIRMEDBY"); } }

        public string CONFIRMEDTIME { get { return _CONFIRMEDTIME; } set { _CONFIRMEDTIME = value; OnPropertyChanged("CONFIRMEDTIME"); } }

        public string PhiscalID
        {
            get
            {
                if (_PhiscalID == null) _PhiscalID = GlobalClass.CurFiscalYear.ToString();
                return _PhiscalID;
            }
            set { _PhiscalID = value; OnPropertyChanged("PhiscalID"); }
        }

        public float Stamp { get { return _Stamp; } set { _Stamp = value; OnPropertyChanged("Stamp"); } }

        public decimal Customer_Count { get { return _Customer_Count; } set { _Customer_Count = value; OnPropertyChanged("Customer_Count"); } }

        public string DBUSER { get { return _DBUSER; } set { _DBUSER = value; OnPropertyChanged("DBUSER"); } }

        public string HOSTID { get { return _HOSTID; } set { _HOSTID = value; OnPropertyChanged("HOSTID"); } }
        public byte ORDERS { get { return _ORDERS; } set { _ORDERS = value; OnPropertyChanged("ORDERS"); } }

        public string REFORDBILL { get { return _REFORDBILL; } set { _REFORDBILL = value; OnPropertyChanged("REFORDBILL"); } }

        public virtual byte INDDIS { get { return _INDDIS; } set { _INDDIS = value; OnPropertyChanged("INDDIS"); } }

        public decimal CREBATE { get { return _CREBATE; } set { _CREBATE = value; OnPropertyChanged("CREBATE"); } }

        public byte CREDIT { get { return _CREDIT; } set { _CREDIT = value; OnPropertyChanged("CREDIT"); } }

        public Nullable<DateTime> DUEDATE { get { return _DUEDATE; } set { _DUEDATE = value; OnPropertyChanged("DUEDATE"); } }

        public string TRNAC { get { return _TRNAC; } set { _TRNAC = value; OnPropertyChanged("TRNAC"); } }

        public string PARAC { get { return _PARAC; } set { _PARAC = value; OnPropertyChanged("PARAC"); } }

        public decimal PARTRNAMNT { get { return _PARTRNAMNT; } set { _PARTRNAMNT = value; OnPropertyChanged("PARTRNAMNT"); } }

        public string RETTO { get { return _RETTO; } set { _RETTO = value; OnPropertyChanged("RETTO"); } }

        public string REFBILL { get { return _REFBILL; } set { _REFBILL = value; OnPropertyChanged("REFBILL"); } }

        public string CHEQUENO { get { return _CHEQUENO; } set { _CHEQUENO = value; OnPropertyChanged("CHEQUENO"); } }

        public Nullable<DateTime> CHEQUEDATE { get { return _CHEQUEDATE; } set { _CHEQUEDATE = value; OnPropertyChanged("CHEQUEDATE"); } }

        public Nullable<DateTime> EDATE { get { return _EDATE; } set { _EDATE = value; OnPropertyChanged("EDATE"); } }

        public byte POST { get { return _POST; } set { _POST = value; OnPropertyChanged("POST"); } }

        public string POSTUSER { get { return _POSTUSER; } set { _POSTUSER = value; OnPropertyChanged("POSTUSER"); } }

        public string FPOSTUSER { get { return _FPOSTUSER; } set { _FPOSTUSER = value; OnPropertyChanged("FPOSTUSER"); } }



        public string SHIFT { get { return _SHIFT; } set { _SHIFT = value; OnPropertyChanged("SHIFT"); } }

        public byte EXPORT { get { return _EXPORT; } set { _EXPORT = value; OnPropertyChanged("EXPORT"); } }

        public string CHOLDER { get { return _CHOLDER; } set { _CHOLDER = value; OnPropertyChanged("CHOLDER"); } }

        public string CARDNO { get { return _CARDNO; } set { _CARDNO = value; OnPropertyChanged("CARDNO"); } }

        public string EditUser { get { return _EditUser; } set { _EditUser = value; OnPropertyChanged("EditUser"); } }

        public string MEMBERNO { get { return _MEMBERNO; } set { _MEMBERNO = value; OnPropertyChanged("MEMBERNO"); } }

        public string MEMBERNAME { get { return _MEMBERNAME; } set { _MEMBERNAME = value; OnPropertyChanged("MEMBERNAME"); } }

        public short EDITED { get { return _EDITED; } set { _EDITED = value; OnPropertyChanged("EDITED"); } }




        public byte VMODE { get { return _VMODE; } set { _VMODE = value; OnPropertyChanged("VMODE"); } }

        public string BILLTOTEL { get { return _BILLTOTEL; } set { _BILLTOTEL = value; OnPropertyChanged("BILLTOTEL"); } }

        public string BILLTOMOB { get { return _BILLTOMOB; } set { _BILLTOMOB = value; OnPropertyChanged("BILLTOMOB"); } }

        public Nullable<DateTime> TRN_DATE
        {
            get
            {
                return _TRN_DATE;
            }
            set
            {
                _TRN_DATE = value; OnPropertyChanged("TRN_DATE");
                if (value != null)
                    _BS_DATE = GlobalClass.GetBSDate((DateTime)_TRN_DATE);
                else
                    _BS_DATE = null;
                OnPropertyChanged("BS_DATE");
            }
        }

        public string BS_DATE
        {
            get
            {
                return _BS_DATE;
            }
            set
            {
                _BS_DATE = value; OnPropertyChanged("BS_DATE");
                //if (_BS_DATE == null)
                //    TRN_DATE = null;
                //else
                //    TRN_DATE = GlobalClass.GetAdDate(_BS_DATE);
                //OnPropertyChanged("TRN_DATE");
            }
        }

        public decimal STAX { get { return _STAX; } set { _STAX = value; OnPropertyChanged("STAX"); } }

        public decimal TOTALCASH { get { return _TOTALCASH; } set { _TOTALCASH = value; OnPropertyChanged("TOTALCASH"); } }

        public decimal TOTALCREDIT { get { return _TOTALCREDIT; } set { _TOTALCREDIT = value; OnPropertyChanged("TOTALCREDIT"); } }

        public decimal TOTALCREDITCARD { get { return _TOTALCREDITCARD; } set { _TOTALCREDITCARD = value; OnPropertyChanged("TOTALCREDITCARD"); } }

        public decimal TOTALGIFTVOUCHER { get { return _TOTALGIFTVOUCHER; } set { _TOTALGIFTVOUCHER = value; OnPropertyChanged("TOTALGIFTVOUCHER"); } }



        public string CardTranID { get { return _CardTranID; } set { _CardTranID = value; OnPropertyChanged("CardTranID"); } }

        public string ReturnVchrID { get { return _ReturnVchrID; } set { _ReturnVchrID = value; OnPropertyChanged("ReturnVchrID"); } }

        public decimal TranID { get { return _TranID; } set { _TranID = value; OnPropertyChanged("TranID"); } }

        public string VoucherStatus { get { return _VoucherStatus; } set { _VoucherStatus = value; OnPropertyChanged("VoucherStatus"); } }

        public string PRESCRIBEBY { get { return _PRESCRIBEBY; } set { _PRESCRIBEBY = value; OnPropertyChanged("PRESCRIBEBY"); } }

        public string DISPENSEBY { get { return _DISPENSEBY; } set { _DISPENSEBY = value; OnPropertyChanged("DISPENSEBY"); } }
        public decimal Longitude { get { return _Longitude; } set { _Longitude = value; OnPropertyChanged("_Longitude"); } }
        public decimal Latitude { get { return _Latitude; } set { _Latitude = value; OnPropertyChanged("_Latitude"); } }
        public string MobileNo { get { return _MobileNo; } set { _MobileNo = value; OnPropertyChanged("_MobileNo"); } }
        public string DeliveryAddress { get { return _DeliveryAddress; } set { _DeliveryAddress = value; OnPropertyChanged("DeliveryAddress"); } }
        public string DeliveryTime { get { return _DeliveryTime; } set { _DeliveryTime = value; OnPropertyChanged("DeliveryTime"); } }
        public string DeliveryMiti { get { return _DeliveryMiti; } set { _DeliveryMiti = value; OnPropertyChanged("DeliveryMiti"); } }
        public string OrderNo { get { return _OrderNo; } set { _OrderNo = value; OnPropertyChanged("OrderNo"); } }
        public string ADJWARE { get { return _ADJWARE; } set { _ADJWARE = value; OnPropertyChanged("ADJWARE"); } }
        public string BATCHNO { get { return _BATCHNO; } set { _BATCHNO = value; OnPropertyChanged("BATCHNO"); } }
        public string BATCHNAME { get { return _BATCHNAME; } set { _BATCHNAME = value; OnPropertyChanged("BATCHNAME"); } }
        public string DPSNO { get { return _DPSNO; } set { _DPSNO = value; OnPropertyChanged("DPSNO"); } }
        public string EDITTIME { get { return _EDITTIME; } set { _EDITTIME = value; OnPropertyChanged("EDITTIME"); } }
        public string EID { get { return _EID; } set { _EID = value; OnPropertyChanged("EID"); } }
        public string EXTRACHARGE { get { return _EXTRACHARGE; } set { _EXTRACHARGE = value; OnPropertyChanged("EXTRACHARGE"); } }
        public string ISSUEWARE { get { return _ISSUEWARE; } set { _ISSUEWARE = value; OnPropertyChanged("ISSUEWARE"); } }

        public Nullable<DateTime> DeliveryDate { get { return _DeliveryDate; } set { _DeliveryDate = value; OnPropertyChanged("DeliveryDate"); } }
        public decimal ADVANCE { get { return _ADVANCE; } set { _ADVANCE = value; OnPropertyChanged("ADVANCE"); } }
        //when vouchertype is the charcater of the trnmain (like Sales/purchase etc so the the vouchername and voucherabbname changes accordingly
        public VoucherTypeEnum VoucherType
        {
            get { return _VoucherType; }
            set
            {
                _VoucherType = value; OnPropertyChanged("VoucherType");
                //GetDefaultVoucherName(value);
            }
        }
        public string VoucherAbbName { get { return _VoucherAbbName; } set { _VoucherAbbName = value; OnPropertyChanged("VoucherAbbName"); } }
        public string VoucherName { get { return _VoucherName; } set { _VoucherName = value; OnPropertyChanged("VoucherName"); } }
        public string VoucherPrefix { get { return _VoucherPrefix; } set { _VoucherPrefix = value; OnPropertyChanged("VoucherPrefix"); } }
        public string VoucherSuffix { get; set; }
        public string VNUM { get { return _VNUM; } set { _VNUM = value; OnPropertyChanged("VNUM"); } }
        public int NextNumber { get { return _NextNumber; } set { _NextNumber = value; OnPropertyChanged("NextNumber"); } }
        public string SalesManID { get { return _SalesManID; } set { _SalesManID = value; OnPropertyChanged("SalesManID"); } }
        //public int  TranId { get { return _TranId; } set { _TranId = value; OnPropertyChanged("TranId"); } }
        public string VCHRNO { get { return _VCHRNO; } set { _VCHRNO = value; OnPropertyChanged("VCHRNO"); OnPropertyChanged("VoucherNumber"); } }
        public string CHALANNO { get { return _CHALANNO; } set { _CHALANNO = value; OnPropertyChanged("CHALANNO"); } }
        public string DIVISION
        {
            get
            {
                //if (string.IsNullOrEmpty(_DIVISION))
                //    _DIVISION = GlobalClass.DIVISION;
                return _DIVISION;
            }
            set { _DIVISION = value; OnPropertyChanged("DIVISION"); }
        }
        public DateTime TRNDATE
        {
            get { return _TRNDATE; }
            set
            {
                if (_TRNDATE == value) return;
                _TRNDATE = value;
                OnPropertyChanged("TRNDATE");
                _BSDATE = GlobalClass.GetBSDate(_TRNDATE);
                OnPropertyChanged("BSDATE");
            }
        }
        public string BSDATE
        {
            get { return _BSDATE; }
            set
            {
                _BSDATE = value;
                OnPropertyChanged("BSDATE");
                //_TRNDATE = GlobalClass.GetAdDate(_BSDATE);
                //OnPropertyChanged("TRNDATE");
            }
        }
        public string TRNTIME { get { return _TRNTIME; } set { _TRNTIME = value; OnPropertyChanged("TRNTIME"); } }
        public decimal TOTAMNT { get { return _TOTAMNT; } set { _TOTAMNT = value; OnPropertyChanged("TOTAMNT"); } }
        public decimal DCAMNT { get { return _DCAMNT; } set { _DCAMNT = value; OnPropertyChanged("DCAMNT"); } }
        public decimal DCRATE
        {
            get { return _DCRATE; }
            set
            {
                _DCRATE = value; OnPropertyChanged("DCRATE");
                //if (ReplaceIndividualWithFlatDiscount == 1)
                //    _TOTALFLATDISCOUNT = this.ProdList.Sum(x => x.AMOUNT - x.PROMOTION - x.LOYALTY) * value;
                //else
                //    _TOTALFLATDISCOUNT = this.ProdList.Sum(x => x.AMOUNT - x.INDDISCOUNT - x.PROMOTION - x.LOYALTY) * value;
                //ReCalculateBill();
            }
        }
        public decimal VATAMNT { get { return _VATAMNT; } set { _VATAMNT = value; OnPropertyChanged("VATAMNT"); } }
        public decimal NETAMNT
        {
            get { return _NETAMNT; }
            set
            {
                _NETAMNT = value; OnPropertyChanged("NETAMNT");
                ROUNDOFF = value - NETWITHOUTROUNDOFF;
            }
        }
        public string TRNMODE { get { return _TRNMODE; } set { _TRNMODE = value; OnPropertyChanged("TRNMODE"); } }
        public decimal TAXABLE { get { return _TAXABLE; } set { _TAXABLE = value; OnPropertyChanged("TAXABLE"); } }
        public decimal NONTAXABLE { get { return _NONTAXABLE; } set { _NONTAXABLE = value; OnPropertyChanged("NONTAXABLE"); } }
        public string BILLTO { get { return _BILLTO; } set { _BILLTO = value; OnPropertyChanged("BILLTO"); } }
        public string BILLTOADD { get { return _BILLTOADD; } set { _BILLTOADD = value; OnPropertyChanged("BILLTOADD"); } }
        public string BILLTOPAN { get { return _BILLTOPAN; } set { _BILLTOPAN = value; OnPropertyChanged("BILLTOPAN"); } }
        public string TRNUSER { get { return _TRNUSER; } set { _TRNUSER = value; OnPropertyChanged("TRNUSER"); } }

        public string TERMINAL { get { return _TERMINAL; } set { _TERMINAL = value; OnPropertyChanged("TERMINAL"); } }
        public decimal TENDER { get { return _TENDER; } set { _TENDER = value; OnPropertyChanged("TENDER"); } }
        public decimal CHANGE { get { return _CHANGE; } set { _CHANGE = value; OnPropertyChanged("CHANGE"); } }
        public decimal ROUNDOFF { get { return _ROUNDOFF; } set { _ROUNDOFF = value; OnPropertyChanged("ROUNDOFF"); } }
        public decimal NETWITHOUTROUNDOFF { get { return _NETWITHOUTROUNDOFF; } set { _NETWITHOUTROUNDOFF = value; OnPropertyChanged("NETWITHOUTROUNDOFF"); } }
        public decimal ServiceCharge { get { return _ServiceCharge; } set { _ServiceCharge = value; OnPropertyChanged("ServiceCharge"); } }
        public Boolean IsTable { get { return _IsTable; } set { _IsTable = value; OnPropertyChanged("IsTable"); } }
        public Boolean IsVATBill { get { return _IsVATBill; } set { _IsVATBill = value; VATAMNT = Convert.ToInt16(value); OnPropertyChanged("IsVATBill"); } }
        public int VATBILL { get { return _VATBILL; } set { _VATBILL = value; OnPropertyChanged("VATBILL"); } }
        //public ObservableCollection<TrnProd> ProdList { get { return _TrnProdList; } set { _TrnProdList = value; OnPropertyChanged("ProdList"); } }
        //public ObservableCollection<TrnProd> AdditionProdList { get { return _AdditionProdList; } set { _AdditionProdList = value; OnPropertyChanged("AdditionProdList"); } }
        //public ObservableCollection<Trntran> AdditionTranList { get { return _AdditionTranList; } set { _AdditionTranList = value; OnPropertyChanged("AdditionTranList"); } }
        //public Discount BillDiscount { get { return _BillDiscount; } set { _BillDiscount = value; OnPropertyChanged("BillDiscount"); } }
        //public Customer BillCustomer { get { return _BillCustomer; } set { _BillCustomer = value; OnPropertyChanged("BillCustomer"); } }
        //public Order_Main Order { get { return _Order; } set { _Order = value; OnPropertyChanged("Order"); } }
        public string REMARKS { get { return _REMARKS; } set { _REMARKS = value; OnPropertyChanged("REMARKS"); } }

        public int EntryNo { get; set; }
        /// <summary>
        /// Integer No of the voucher excluding the voucher prefix
        /// </summary>
        public int VoucherNumber
        {
            get
            {
                int n = 0;
                if (!string.IsNullOrEmpty(_VCHRNO) && _VoucherPrefix != null)
                {
                    n = Convert.ToInt32(VCHRNO.Substring(_VoucherPrefix.Length, _VCHRNO.Length - _VoucherPrefix.Length));

                }
                return n;
            }
            set
            {
                _VCHRNO = _VoucherPrefix + value.ToString();
                OnPropertyChanged("VoucherNumber");
            }
        }
        public string JOBNO { get; set; }
        #endregion
    }

    public class TrnMain : TTrnMain
    {
    }

}
