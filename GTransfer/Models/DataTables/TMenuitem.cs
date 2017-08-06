using GTransfer.Library;
using System;
namespace ImsPosLibrary.Models
{
    public class TMenuitem : BaseModel
    {
        #region Members
		
        private int _Serial;
        private string _MCODE;
        private string _MENUCODE;
        private string _DESCA;
        private string _DESCB;
        private string _PARENT;
        private  string _TYPE;
        private string _BASEUNIT;
        private string _ALTUNIT;
        private decimal _CONFACTOR;
        private decimal _RATE_A;
        private decimal _RATE_B;
        private decimal _PRATE_A;
        private decimal _PRATE_B;
        private byte _VAT;
        private decimal _MINLEVEL;
        private decimal _MAXLEVEL;
        private decimal _ROLEVEL;
        private byte _MINWARN;
        private byte _MAXWARN;
        private byte _ROWARN;
        private byte _LEVELS;
        private string _BRAND;
        private string _MODEL;
        private string _MGROUP;
        private decimal _FCODE;
        private decimal _ECODE;
        private string _DISMODE;
        private decimal _DISRATE;
        private decimal _DISAMOUNT;
        private decimal _RECRATE;
        private decimal _MARGIN;
        private decimal _PRERATE;
        private decimal _PRESRATE;
        private byte _DISCONTINUE;
        private string _MODES;
        private decimal _PRERATE1;
        private decimal _PRERATE2;
        private decimal _SCHEME_A;
        private decimal _SCHEME_B;
        private decimal _SCHEME_C;
        private decimal _SCHEME_D;
        private decimal _SCHEME_E;
        private byte _FLGNEW;
        private byte _SALESMANID;
        private byte _TDAILY;
        private byte _TMONTHLY;
        private byte _TYEARLY;
        private decimal _VPRATE;
        private decimal _VSRATE;
        private string _PATH;
        private byte _PTYPE;
        private string _SUPCODE;
        private string _LATESTBILL;
        private byte _ZEROROLEVEL;
        private string _MCAT;
        private string _MIDCODE;
        private string _SAC;
        private string _SRAC;
        private string _PAC;
        private string _PRAC;
        private decimal _RATE_C;
        private decimal _CRATE;
        private string _GENERIC;
        private byte _ISUNKNOWN;
        private Nullable<DateTime> _EDATE;
        private byte _TSTOP;
        private string _BARCODE;
        private byte _HASSERIAL;
        private byte _HASSERVICECHARGE;
        private string _DIMENSION;
        private decimal _FOB;
        private string _COLOR;
        private string _PACK;
        private string _PRODTYPE;
        private string _GWEIGHT;
        private string _NWEIGHT;
        private string _CBM;
        private byte _HASBATCH;
        private string _SUPITEMCODE;
        private Nullable<DateTime> _LPDATE;
        private Nullable<DateTime> _CRDATE;
        private int _TAXGROUP_ID;
        private decimal _LEADTIME;
        private string _TRNUSER;
        #endregion

        #region Properties
        public string TRNUSER { get { return _TRNUSER; } set { _TRNUSER = value; OnPropertyChanged("TRNUSER"); } }
        public int Serial{ get{return _Serial; } set { _Serial = value; OnPropertyChanged("Serial");}}
    
        public string MCODE{ get{return _MCODE; } set { _MCODE = value; OnPropertyChanged("MCODE");}}
    
        public string MENUCODE{ get{return _MENUCODE; } set { _MENUCODE = value; OnPropertyChanged("MENUCODE");}}
    
        public string DESCA{ get{return _DESCA; } set { _DESCA = value; OnPropertyChanged("DESCA");}}
    
        public string DESCB{ get{return _DESCB; } set { _DESCB = value; OnPropertyChanged("DESCB");}}
    
        public string PARENT{ get{return _PARENT; } set { _PARENT = value; OnPropertyChanged("PARENT");}}
    
        public string  TYPE{ get{return _TYPE; } set {
            if (value == null) return;
            if( (value.ToUpper()  =="G" ) || (value.ToUpper()  =="A"))
            {
                _TYPE = value;
            }
            else
            {
                _TYPE = "A";
            }
                    ; OnPropertyChanged("TYPE");}}
    
        public string BASEUNIT{ get{return _BASEUNIT; } set { _BASEUNIT = value; OnPropertyChanged("BASEUNIT");}}
    
        public string ALTUNIT{ get{return _ALTUNIT; } set { _ALTUNIT = value; OnPropertyChanged("ALTUNIT");}}
    
        public decimal CONFACTOR{ get{return _CONFACTOR; } set { _CONFACTOR = value; OnPropertyChanged("CONFACTOR");}}
    
        public decimal RATE_A{ get{return _RATE_A; } set { _RATE_A = value; OnPropertyChanged("RATE_A");}}
    
        public decimal RATE_B{ get{return _RATE_B; } set { _RATE_B = value; OnPropertyChanged("RATE_B");}}
    
        public decimal PRATE_A{ get{return _PRATE_A; } set { _PRATE_A = value; OnPropertyChanged("PRATE_A");}}
    
        public decimal PRATE_B{ get{return _PRATE_B; } set { _PRATE_B = value; OnPropertyChanged("PRATE_B");}}
    
        public byte VAT{ get{return _VAT; } set { _VAT = value; OnPropertyChanged("VAT");}}
    
        public decimal MINLEVEL{ get{return _MINLEVEL; } set { _MINLEVEL = value; OnPropertyChanged("MINLEVEL");}}
    
        public decimal MAXLEVEL{ get{return _MAXLEVEL; } set { _MAXLEVEL = value; OnPropertyChanged("MAXLEVEL");}}
    
        public decimal ROLEVEL{ get{return _ROLEVEL; } set { _ROLEVEL = value; OnPropertyChanged("ROLEVEL");}}
    
        public byte MINWARN{ get{return _MINWARN; } set { _MINWARN = value; OnPropertyChanged("MINWARN");}}
    
        public byte MAXWARN{ get{return _MAXWARN; } set { _MAXWARN = value; OnPropertyChanged("MAXWARN");}}
    
        public byte ROWARN{ get{return _ROWARN; } set { _ROWARN = value; OnPropertyChanged("ROWARN");}}
    
        public byte LEVELS{ get{return _LEVELS; } set { _LEVELS = value; OnPropertyChanged("LEVELS");}}
    
        public string BRAND{ get{return _BRAND; } set { _BRAND = value; OnPropertyChanged("BRAND");}}
    
        public string MODEL{ get{return _MODEL; } set { _MODEL = value; OnPropertyChanged("MODEL");}}
    
        public string MGROUP{ get{return _MGROUP; } set { _MGROUP = value; OnPropertyChanged("MGROUP");}}
    
        public decimal FCODE{ get{return _FCODE; } set { _FCODE = value; OnPropertyChanged("FCODE");}}
    
        public decimal ECODE{ get{return _ECODE; } set { _ECODE = value; OnPropertyChanged("ECODE");}}
    
        public string DISMODE{ get{return _DISMODE; } set { _DISMODE = value; OnPropertyChanged("DISMODE");}}
    
        public decimal DISRATE{ get{return _DISRATE; } set { _DISRATE = value; OnPropertyChanged("DISRATE");}}
    
        public decimal DISAMOUNT{ get{return _DISAMOUNT; } set { _DISAMOUNT = value; OnPropertyChanged("DISAMOUNT");}}
    
        public decimal RECRATE{ get{return _RECRATE; } set { _RECRATE = value; OnPropertyChanged("RECRATE");}}
    
        public decimal MARGIN{ get{return _MARGIN; } set { _MARGIN = value; OnPropertyChanged("MARGIN");}}
    
        public decimal PRERATE{ get{return _PRERATE; } set { _PRERATE = value; OnPropertyChanged("PRERATE");}}
    
        public decimal PRESRATE{ get{return _PRESRATE; } set { _PRESRATE = value; OnPropertyChanged("PRESRATE");}}
    
        public byte DISCONTINUE{ get{return _DISCONTINUE; } set { _DISCONTINUE = value; OnPropertyChanged("DISCONTINUE");}}
    
        public string MODES{ get{return _MODES; } set { _MODES = value; OnPropertyChanged("MODES");}}
    
        public decimal PRERATE1{ get{return _PRERATE1; } set { _PRERATE1 = value; OnPropertyChanged("PRERATE1");}}
    
        public decimal PRERATE2{ get{return _PRERATE2; } set { _PRERATE2 = value; OnPropertyChanged("PRERATE2");}}
    
        public decimal SCHEME_A{ get{return _SCHEME_A; } set { _SCHEME_A = value; OnPropertyChanged("SCHEME_A");}}
    
        public decimal SCHEME_B{ get{return _SCHEME_B; } set { _SCHEME_B = value; OnPropertyChanged("SCHEME_B");}}
    
        public decimal SCHEME_C{ get{return _SCHEME_C; } set { _SCHEME_C = value; OnPropertyChanged("SCHEME_C");}}
    
        public decimal SCHEME_D{ get{return _SCHEME_D; } set { _SCHEME_D = value; OnPropertyChanged("SCHEME_D");}}
    
        public decimal SCHEME_E{ get{return _SCHEME_E; } set { _SCHEME_E = value; OnPropertyChanged("SCHEME_E");}}
    
        public byte FLGNEW{ get{return _FLGNEW; } set { _FLGNEW = value; OnPropertyChanged("FLGNEW");}}
    
        public byte SALESMANID{ get{return _SALESMANID; } set { _SALESMANID = value; OnPropertyChanged("SALESMANID");}}
    
        public byte TDAILY{ get{return _TDAILY; } set { _TDAILY = value; OnPropertyChanged("TDAILY");}}
    
        public byte TMONTHLY{ get{return _TMONTHLY; } set { _TMONTHLY = value; OnPropertyChanged("TMONTHLY");}}
    
        public byte TYEARLY{ get{return _TYEARLY; } set { _TYEARLY = value; OnPropertyChanged("TYEARLY");}}
    
        public decimal VPRATE{ get{return _VPRATE; } set { _VPRATE = value; OnPropertyChanged("VPRATE");}}
    
        public decimal VSRATE{ get{return _VSRATE; } set { _VSRATE = value; OnPropertyChanged("VSRATE");}}
    
        public string PATH{ get{return _PATH; } set { _PATH = value; OnPropertyChanged("PATH");}}
    
        public byte PTYPE{ get{return _PTYPE; } set { _PTYPE = value; OnPropertyChanged("PTYPE");}}
    
        public string SUPCODE{ get{return _SUPCODE; } set { _SUPCODE = value; OnPropertyChanged("SUPCODE");}}
    
        public string LATESTBILL{ get{return _LATESTBILL; } set { _LATESTBILL = value; OnPropertyChanged("LATESTBILL");}}
    
        public byte ZEROROLEVEL{ get{return _ZEROROLEVEL; } set { _ZEROROLEVEL = value; OnPropertyChanged("ZEROROLEVEL");}}
    
        public string MCAT{ get{return _MCAT; } set { _MCAT = value; OnPropertyChanged("MCAT");}}
    
        public string MIDCODE{ get{return _MIDCODE; } set { _MIDCODE = value; OnPropertyChanged("MIDCODE");}}
    
        public string SAC{ get{return _SAC; } set { _SAC = value; OnPropertyChanged("SAC");}}
    
        public string SRAC{ get{return _SRAC; } set { _SRAC = value; OnPropertyChanged("SRAC");}}
    
        public string PAC{ get{return _PAC; } set { _PAC = value; OnPropertyChanged("PAC");}}
    
        public string PRAC{ get{return _PRAC; } set { _PRAC = value; OnPropertyChanged("PRAC");}}
    
        public decimal RATE_C{ get{return _RATE_C; } set { _RATE_C = value; OnPropertyChanged("RATE_C");}}
    
        public decimal CRATE{ get{return _CRATE; } set { _CRATE = value; OnPropertyChanged("CRATE");}}
    
        public string GENERIC{ get{return _GENERIC; } set { _GENERIC = value; OnPropertyChanged("GENERIC");}}
    
        public byte ISUNKNOWN{ get{return _ISUNKNOWN; } set { _ISUNKNOWN = value; OnPropertyChanged("ISUNKNOWN");}}
    
        public Nullable<DateTime> EDATE{ get{return _EDATE; } set { _EDATE = value; OnPropertyChanged("EDATE");}}
    
        public byte TSTOP{ get{return _TSTOP; } set { _TSTOP = value; OnPropertyChanged("TSTOP");}}
    
        public string BARCODE{ get{return _BARCODE; } set { _BARCODE = value; OnPropertyChanged("BARCODE");}}

        public byte HASSERIAL { get { return _HASSERIAL; } set { _HASSERIAL = value; OnPropertyChanged("HASSERIAL"); } }

        public byte HASSERVICECHARGE { get { return _HASSERVICECHARGE; } set { _HASSERVICECHARGE = value; OnPropertyChanged("HASSERVICECHARGE"); } }

        public string DIMENSION { get { return _DIMENSION; } set { _DIMENSION = value; OnPropertyChanged("DIMENSION"); } }

        public decimal FOB { get { return _FOB; } set { _FOB = value; OnPropertyChanged("FOB"); } }

        public string COLOR { get { return _COLOR; } set { _COLOR = value; OnPropertyChanged("COLOR"); } }

        public string PACK { get { return _PACK; } set { _PACK = value; OnPropertyChanged("PACK"); } }

        public string PRODTYPE { get { return _PRODTYPE; } set { _PRODTYPE = value; OnPropertyChanged("PRODTYPE"); } }

        public string GWEIGHT { get { return _GWEIGHT; } set { _GWEIGHT = value; OnPropertyChanged("GWEIGHT"); } }

        public string NWEIGHT { get { return _NWEIGHT; } set { _NWEIGHT = value; OnPropertyChanged("NWEIGHT"); } }

        public string CBM { get { return _CBM; } set { _CBM = value; OnPropertyChanged("CBM"); } }

        public byte HASBATCH { get { return _HASBATCH; } set { _HASBATCH = value; OnPropertyChanged("HASBATCH"); } }

        public string SUPITEMCODE { get { return _SUPITEMCODE; } set { _SUPITEMCODE = value; OnPropertyChanged("SUPITEMCODE"); } }

        public Nullable<DateTime> LPDATE { get { return _LPDATE; } set { _LPDATE = value; OnPropertyChanged("LPDATE"); } }

        public Nullable<DateTime> CRDATE { get { return _CRDATE; } set { _CRDATE = value; OnPropertyChanged("CRDATE"); } }

        public int TAXGROUP_ID { get { return _TAXGROUP_ID; } set { _TAXGROUP_ID = value; OnPropertyChanged("TAXGROUP_ID"); } }

        public decimal LEADTIME { get { return _LEADTIME; } set { _LEADTIME = value; OnPropertyChanged("LEADTIME"); } }

    #endregion
       
    }
}
