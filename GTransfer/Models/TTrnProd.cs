using System;
using GTransfer.Library;

namespace GTransfer.Models
{
    public class TTrnProd : BaseModel
    {
        #region Database Members
        private string _VCHRNO;
        private string _CHALANNO;
        private string _DIVISION;
        private string _MCODE;
        private string _UNIT;
        private decimal _Quantity;
        private decimal _RealQty;
        private decimal _AltQty;
        private decimal _RATE;
        private decimal _AMOUNT;
        private decimal _DISCOUNT;
        private decimal _VAT;
        private decimal _REALRATE;
        private byte _EXPORT;
        private string _IDIS;
        private decimal _OPQTY;
        private decimal _REALQTY_IN;
        private decimal _ALTQTY_IN;
        private string _WAREHOUSE;
        private decimal _REFBILLQTY;
        private decimal _SPRICE;
        private Nullable<DateTime> _EXPDATE;
        private decimal _REFOPBILL;
        private string _ALTUNIT;
        private string _TRNAC;
        private decimal _PRATE;
        private decimal _VRATE;
        private decimal _ALTRATE;
        private decimal _VPRATE;
        private decimal _VSRATE;
        private decimal _TAXABLE;
        private decimal _NONTAXABLE;
        private decimal _INVRATE;
        private decimal _EXRATE = 1;
        private decimal _NCRATE;
        private decimal _CRATE;
        private int _SNO;
        private decimal _CUSTOM;
        private decimal _WEIGHT;
        private decimal _DRATE;
        private decimal _CARTON;
        private string _INVOICENO;
        private string _ISSUENO;
        private string _BC;
        private string _GENERIC;
        private string _BATCH;
        private Nullable<DateTime> _MFGDATE;
        private string _MANUFACTURER;
        private decimal _SERVICETAX;
        private decimal _BCODEID;
        private VoucherTypeEnum _VoucherType;
        private int _SALESMANID;
        private string _PhiscalID;
        private float _STAMP;
        private string _ITEMDESC;
        private byte _ADDTIONALROW;
        private byte _ISVAT;
        private byte _ISSERVICECHARGE;
        private decimal _PROMOTION;
        private decimal _LOYALTY;
        //private decimal _TOTALDISCOUNT;
        private decimal _FLATDISCOUNT;
        private decimal _INDDISCOUNT;
        private decimal _NETAMOUNT;
        private decimal _SQTY;
        private string _REMARKS;
        private string _COSTCENTER;
        private string _PRODCONDITION;
        private string _PRODMODE;
        #endregion

        #region Database Properties
        public virtual decimal INDDISCOUNT { get { return _INDDISCOUNT; } set { _INDDISCOUNT = value; OnPropertyChanged("INDDISCOUNT"); } }
        public virtual decimal FLATDISCOUNT { get { return _FLATDISCOUNT; } set { _FLATDISCOUNT = value; OnPropertyChanged("FLATDISCOUNT"); } }
        public virtual decimal NETAMOUNT { get { return _NETAMOUNT; } set { _NETAMOUNT = value; OnPropertyChanged("NETAMOUNT"); } }
        //public virtual decimal TOTALDISCOUNT { get { return _TOTALDISCOUNT; } set { _TOTALDISCOUNT = value; OnPropertyChanged("TOTALDISCOUNT"); } }
        public virtual decimal LOYALTY { get { return _LOYALTY; } set { _LOYALTY = value; OnPropertyChanged("LOYALTY"); } }
        public virtual decimal PROMOTION { get { return _PROMOTION; } set { _PROMOTION = value; OnPropertyChanged("PROMOTION"); } }
        /// <summary>
        /// set to 1 if there is service charge in the product
        /// </summary>
        public byte ISSERVICECHARGE { get { return _ISSERVICECHARGE; } set { _ISSERVICECHARGE = value; OnPropertyChanged("ISSERVICECHARGE"); } }
        /// <summary>
        /// set to 1 if the product is vat applicable
        /// </summary>
        public byte ISVAT { get { return _ISVAT; } set { _ISVAT = value; OnPropertyChanged("ISVAT"); } }
        public byte ADDTIONALROW { get { return _ADDTIONALROW; } set { _ADDTIONALROW = value; OnPropertyChanged("ADDTIONALROW"); } }
        public string COSTCENTER { get { return _COSTCENTER; } set { _COSTCENTER = value; OnPropertyChanged("COSTCENTER"); } }
        public string VCHRNO { get { return _VCHRNO; } set { _VCHRNO = value; OnPropertyChanged("VCHRNO"); } }

        public string CHALANNO { get { return _CHALANNO; } set { _CHALANNO = value; OnPropertyChanged("CHALANNO"); } }

        public string DIVISION { get { return _DIVISION; } set { _DIVISION = value; OnPropertyChanged("DIVISION"); } }

        public string MCODE { get { return _MCODE; } set { _MCODE = value; OnPropertyChanged("MCODE"); } }

        public string UNIT { get { return _UNIT; } set { _UNIT = value; OnPropertyChanged("UNIT"); } }

        public virtual decimal Quantity { get { return _Quantity; } set { _Quantity = value; OnPropertyChanged("Quantity"); } }

        public decimal RealQty { get { return _RealQty; } set { _RealQty = value; OnPropertyChanged("RealQty"); } }

        public decimal AltQty { get { return _AltQty; } set { _AltQty = value; OnPropertyChanged("AltQty"); } }

        public virtual decimal RATE { get { return _RATE; } set { _RATE = value; OnPropertyChanged("RATE"); } }

        public virtual decimal AMOUNT { get { return _AMOUNT; } set { _AMOUNT = value; OnPropertyChanged("AMOUNT"); } }

        public virtual decimal DISCOUNT { get { return _DISCOUNT; } set { _DISCOUNT = value; OnPropertyChanged("DISCOUNT"); } }
        public decimal VAT { get { return _VAT; } set { _VAT = value; OnPropertyChanged("VAT"); } }

        public decimal REALRATE { get { return _REALRATE; } set { _REALRATE = value; OnPropertyChanged("REALRATE"); } }

        public byte EXPORT { get { return _EXPORT; } set { _EXPORT = value; OnPropertyChanged("EXPORT"); } }

        public virtual string IDIS { get { return _IDIS; } set { _IDIS = value; OnPropertyChanged("IDIS"); } }

        public decimal OPQTY { get { return _OPQTY; } set { _OPQTY = value; OnPropertyChanged("OPQTY"); } }

        public decimal REALQTY_IN { get { return _REALQTY_IN; } set { _REALQTY_IN = value; OnPropertyChanged("REALQTY_IN"); } }

        public decimal ALTQTY_IN { get { return _ALTQTY_IN; } set { _ALTQTY_IN = value; OnPropertyChanged("ALTQTY_IN"); } }

        public string WAREHOUSE { get { return _WAREHOUSE; } set { _WAREHOUSE = value; OnPropertyChanged("WAREHOUSE"); } }

        public decimal REFBILLQTY { get { return _REFBILLQTY; } set { _REFBILLQTY = value; OnPropertyChanged("REFBILLQTY"); } }
        /// <summary>
        /// SalePrice if possible
        /// </summary>
        public decimal SPRICE { get { return _SPRICE; } set { _SPRICE = value; OnPropertyChanged("SPRICE"); } }

        public Nullable<DateTime> EXPDATE { get { return _EXPDATE; } set { _EXPDATE = value; OnPropertyChanged("EXPDATE"); } }

        public decimal REFOPBILL { get { return _REFOPBILL; } set { _REFOPBILL = value; OnPropertyChanged("REFOPBILL"); } }

        public string ALTUNIT { get { return _ALTUNIT; } set { _ALTUNIT = value; OnPropertyChanged("ALTUNIT"); } }
        /// <summary>
        /// Transaction Account
        /// </summary>
        public string TRNAC { get { return _TRNAC; } set { _TRNAC = value; OnPropertyChanged("TRNAC"); } }
        /// <summary>
        /// Purchase Rate
        /// </summary>
        public decimal PRATE { get { return _PRATE; } set { _PRATE = value; OnPropertyChanged("PRATE"); } }

        public decimal VRATE { get { return _VRATE; } set { _VRATE = value; OnPropertyChanged("VRATE"); } }

        public decimal ALTRATE { get { return _ALTRATE; } set { _ALTRATE = value; OnPropertyChanged("ALTRATE"); } }

        public decimal VPRATE { get { return _VPRATE; } set { _VPRATE = value; OnPropertyChanged("VPRATE"); } }

        public decimal VSRATE { get { return _VSRATE; } set { _VSRATE = value; OnPropertyChanged("VSRATE"); } }

        public decimal TAXABLE { get { return _TAXABLE; } set { _TAXABLE = value; OnPropertyChanged("TAXABLE"); } }

        public decimal NONTAXABLE { get { return _NONTAXABLE; } set { _NONTAXABLE = value; OnPropertyChanged("NONTAXABLE"); } }

        public decimal SQTY { get { return _SQTY; } set { _SQTY = value; OnPropertyChanged("SQTY"); } }
        public string REMARKS { get { return _REMARKS; } set { _REMARKS = value; OnPropertyChanged("REMARKS"); } }
        /// <summary>
        /// Invoice Rate of foreing currency
        /// </summary>
        public decimal INVRATE { get { return _INVRATE; } set { _INVRATE = value; OnPropertyChanged("INVRATE"); } }
        /// <summary>
        /// Exchange Rate
        /// </summary>
        public decimal EXRATE { get { return _EXRATE; } set { _EXRATE = value; OnPropertyChanged("EXRATE"); } }
        /// <summary>
        /// Net cost rate after deducting discount
        /// </summary>
        public decimal NCRATE { get { return _NCRATE; } set { _NCRATE = value; OnPropertyChanged("NCRATE"); } }

        public decimal CRATE { get { return _CRATE; } set { _CRATE = value; OnPropertyChanged("CRATE"); } }

        public int SNO { get { return _SNO; } set { _SNO = value; OnPropertyChanged("SNO"); } }

        public decimal CUSTOM { get { return _CUSTOM; } set { _CUSTOM = value; OnPropertyChanged("CUSTOM"); } }

        public decimal WEIGHT { get { return _WEIGHT; } set { _WEIGHT = value; OnPropertyChanged("WEIGHT"); } }

        public decimal DRATE { get { return _DRATE; } set { _DRATE = value; OnPropertyChanged("DRATE"); } }

        public decimal CARTON { get { return _CARTON; } set { _CARTON = value; OnPropertyChanged("CARTON"); } }

        public string INVOICENO { get { return _INVOICENO; } set { _INVOICENO = value; OnPropertyChanged("INVOICENO"); } }

        public string ISSUENO { get { return _ISSUENO; } set { _ISSUENO = value; OnPropertyChanged("ISSUENO"); } }

        public virtual string BC { get { return _BC; } set { _BC = value; OnPropertyChanged("BC"); } }

        public string GENERIC { get { return _GENERIC; } set { _GENERIC = value; OnPropertyChanged("GENERIC"); } }

        public string BATCH { get { return _BATCH; } set { _BATCH = value; OnPropertyChanged("BATCH"); } }

        public Nullable<DateTime> MFGDATE { get { return _MFGDATE; } set { _MFGDATE = value; OnPropertyChanged("MFGDATE"); } }

        public string MANUFACTURER { get { return _MANUFACTURER; } set { _MANUFACTURER = value; OnPropertyChanged("MANUFACTURER"); } }

        public decimal SERVICETAX { get { return _SERVICETAX; } set { _SERVICETAX = value; OnPropertyChanged("SERVICETAX"); } }

        public decimal BCODEID { get { return _BCODEID; } set { _BCODEID = value; OnPropertyChanged("BCODEID"); } }

        public VoucherTypeEnum VoucherType { get { return _VoucherType; } set { _VoucherType = value; OnPropertyChanged("VoucherType"); } }

        public int SALESMANID { get { return _SALESMANID; } set { _SALESMANID = value; OnPropertyChanged("SALESMANID"); } }

        public string PhiscalID { get { return _PhiscalID; } set { _PhiscalID = value; OnPropertyChanged("PhiscalID"); } }

        public float STAMP { get { return _STAMP; } set { _STAMP = value; OnPropertyChanged("STAMP"); } }

        public string ITEMDESC { get { return _ITEMDESC; } set { _ITEMDESC = value; OnPropertyChanged("ITEMDESC"); } }
        public string PRODCONDITION { get { return _PRODCONDITION; } set { _PRODCONDITION = value; OnPropertyChanged("PRODCONDITION"); } }
        public string PRODMODE { get { return _PRODMODE; } set { _PRODMODE = value; OnPropertyChanged("PRODMODE"); } }
        public string ITEMTYPE { get; set; }
        public string RECEIVEDTYPE { get; set; }
        #endregion
    }

    public class TrnProd : TTrnProd
    {
        private int _Ptype;
        private decimal _OrderQty;
        private decimal _VarianceQty;
        public int Ptype { get { return _Ptype; } set { _Ptype = value; OnPropertyChanged("Ptype"); } }
        private string _MENUCODE;

        public string MENUCODE { get { return _MENUCODE; } set { _MENUCODE = value; OnPropertyChanged("MENUCODE"); } }

        public decimal VarianceQty { get { return _VarianceQty; } set { _VarianceQty = value; OnPropertyChanged("VarianceQty"); } }
        public decimal OrderQty { get { return _OrderQty; } set { _OrderQty = value; OnPropertyChanged("OrderQty"); } }

        public void CalculateNormal()
        {

            AMOUNT = Quantity * RATE;
            if (ISVAT == 1)
            {
                TAXABLE = (AMOUNT - DISCOUNT) + SERVICETAX;
                NONTAXABLE = 0;
                VAT = TAXABLE * Settings.VatRate;
            }
            else
            {
                TAXABLE = 0;
                NONTAXABLE = (AMOUNT - DISCOUNT) + SERVICETAX;
            }
            NETAMOUNT = TAXABLE + NONTAXABLE + VAT;


        }
    }
}
