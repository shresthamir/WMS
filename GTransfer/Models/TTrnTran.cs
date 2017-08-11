using GTransfer.Library;
using System.Collections.ObjectModel;

namespace GTransfer.Models
{
    public class TTrnTran : BaseModel
    {
        #region Members
        private string _VCHRNO;
        private string _CHALANNO;
        private string _DIVISION;
        private string _A_ACID;
        public decimal _DRAMNT;
        public decimal _CRAMNT;
        private string _B_ACID;
        private string _NARATION;
        private string _TOACID;
        private string _NARATION1;
        private VoucherTypeEnum _VoucherType;
        private string _ChequeNo;
        private string _ChequeDate;
        private string _FCurrency;
        private decimal _FCurAmount;
        private string _CostCenter;
        private decimal _MultiJournalSno;
        private string _PhiscalID;
        private decimal _SNO;

        #endregion

        #region Properties

        public string VCHRNO { get { return _VCHRNO; } set { _VCHRNO = value; OnPropertyChanged("VCHRNO"); } }

        public string CHALANNO { get { return _CHALANNO; } set { _CHALANNO = value; OnPropertyChanged("CHALANNO"); } }

        public string DIVISION { get { return _DIVISION; } set { _DIVISION = value; OnPropertyChanged("DIVISION"); } }

        public string A_ACID { get { return _A_ACID; } set { _A_ACID = value; OnPropertyChanged("A_ACID"); } }

        public virtual decimal DRAMNT { get { return _DRAMNT; } set { _DRAMNT = value; OnPropertyChanged("DRAMNT"); } }

        public virtual decimal CRAMNT { get { return _CRAMNT; } set { _CRAMNT = value; OnPropertyChanged("CRAMNT"); } }

        public string B_ACID { get { return _B_ACID; } set { _B_ACID = value; OnPropertyChanged("B_ACID"); } }

        public string NARATION { get { return _NARATION; } set { _NARATION = value; OnPropertyChanged("NARATION"); } }

        public string TOACID { get { return _TOACID; } set { _TOACID = value; OnPropertyChanged("TOACID"); } }

        public string NARATION1 { get { return _NARATION1; } set { _NARATION1 = value; OnPropertyChanged("NARATION1"); } }

        public VoucherTypeEnum VoucherType { get { return _VoucherType; } set { _VoucherType = value; OnPropertyChanged("VoucherType"); } }

        public string ChequeNo { get { return _ChequeNo; } set { _ChequeNo = value; OnPropertyChanged("ChequeNo"); } }

        public string ChequeDate { get { return _ChequeDate; } set { _ChequeDate = value; OnPropertyChanged("ChequeDate"); } }

        public string FCurrency { get { return _FCurrency; } set { _FCurrency = value; OnPropertyChanged("FCurrency"); } }

        public decimal FCurAmount { get { return _FCurAmount; } set { _FCurAmount = value; OnPropertyChanged("FCurAmount"); } }

        public string CostCenter { get { return _CostCenter; } set { _CostCenter = value; OnPropertyChanged("CostCenter"); } }

        public decimal MultiJournalSno { get { return _MultiJournalSno; } set { _MultiJournalSno = value; OnPropertyChanged("MultiJournalSno"); } }

        public string PhiscalID { get { return _PhiscalID; } set { _PhiscalID = value; OnPropertyChanged("PhiscalID"); } }
        public decimal SNO { get { return _SNO; } set { _SNO = value; OnPropertyChanged("SNO"); } }
        #endregion
    }

    public class TrnTran : TTrnTran
    {
        private string _ACNAME;  
        public int EntrySNo { get; set; }
        public string ACNAME { get { return _ACNAME; } set { _ACNAME = value; OnPropertyChanged("ACNAME"); } }       
    }


}
