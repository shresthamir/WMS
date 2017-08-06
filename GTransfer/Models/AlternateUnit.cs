using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using GTransfer.Library;

namespace GTransfer.Models
{
    public class AlternateUnit:BaseModel,IDataErrorInfo
    {
        private short _SNO;
        private string _BRCODE;
        private bool  _ISDEFAULT;
        private decimal  _RATE;
        private decimal  _CONFACTOR;
        private string _ALTUNIT;
        private string _MCODE;
        private decimal  _RATE_A;
        private decimal  _RATE_B;
        private decimal  _RATE_D;
        private decimal  _RATE_C;
        private decimal _PRATE;
        private bool  _ISDEFAULTPRATE;
        public short SNO { get { return _SNO; } set { _SNO = value; OnPropertyChanged("SNO"); } }
        public string MCODE { get { return _MCODE; } set { _MCODE = value; OnPropertyChanged("MCODE"); } }
        public string ALTUNIT { get { return _ALTUNIT; } set { _ALTUNIT = value; OnPropertyChanged("ALTUNIT"); } }
        public decimal  CONFACTOR { get { return _CONFACTOR; } set { _CONFACTOR = value; OnPropertyChanged("CONFACTOR"); } }
        public decimal  RATE { get { return _RATE; } set { _RATE = value; OnPropertyChanged("RATE"); } }
        public bool  ISDEFAULT { get { return _ISDEFAULT; } set { _ISDEFAULT = value; OnPropertyChanged("ISDEFAULT"); } }
        public bool ISDEFAULTPRATE { get { return _ISDEFAULTPRATE; } set { _ISDEFAULTPRATE = value; OnPropertyChanged("ISDEFAULTPRATE"); } }
        public string BRCODE { get { return _BRCODE; } set { _BRCODE = value; OnPropertyChanged("BRCODE"); } }
        public decimal  RATE_A { get { return _RATE_A; } set { _RATE_A = value; OnPropertyChanged("RATE_A"); } }
        public decimal  RATE_B { get { return _RATE_B; } set { _RATE_B = value; OnPropertyChanged("RATE_B"); } }
        public decimal  RATE_C { get { return _RATE_C; } set { _RATE_C = value; OnPropertyChanged("RATE_C"); } }
        public decimal  RATE_D { get { return _RATE_D; } set { _RATE_D = value; OnPropertyChanged("RATE_D"); } }
        public decimal PRATE { get { return _PRATE; } set { _PRATE = value; OnPropertyChanged("PRATE"); } }

        #region AddedByAmir
        private byte _ISDISCONTINUE;
        public byte ISDISCONTINUE { get { return _ISDISCONTINUE; } set { _ISDISCONTINUE = value; OnPropertyChanged("ISDISCONTINUE"); } }

        #region data validation

        string IDataErrorInfo.Error
        {
            get { throw new NotImplementedException(); }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                switch (columnName)
                {

                    case "ALTUNIT":
                        {
                            if (this.ALTUNIT == string.Empty || this.ALTUNIT == null)
                                return "Alternate unit cannotbe empty";
                            break;
                        }
                    case "CONFACTOR":
                        {
                            if (CONFACTOR == 0)
                                return "Conversion Factor cannot be zero";
                            break;
                        }
                    case "MCODE":
                        {
                            if (MCODE == null)
                                return "Mcode is empty";
                            break;
                        }

                }

                return string.Empty;
            }
        }
        #endregion //data validation

      
        #endregion
    }
}
