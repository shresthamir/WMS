using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using GTransfer.Library;

namespace GTransfer.Models
{
    public class Company:BaseModel
    {
        private string _INITIAL;
        private String _Name;
        private String _TELB;
        private String _ADDRESS;
        private String _TELA;
        private String _VAT;
        public string INITIAL { get { return _INITIAL; } set { _INITIAL = value; OnPropertyChanged("INITIAL"); } }        
        public String NAME { get { return _Name; } set { _Name = value; OnPropertyChanged("NAME"); } }        
        public String ADDRESS { get { return _ADDRESS; } set { _ADDRESS = value; OnPropertyChanged("ADDRESS"); } }        
        public String TELA { get { return _TELA; } set { _TELA = value; OnPropertyChanged("TELA"); } }        
        public String VAT { get { return _VAT; } set { _VAT = value; OnPropertyChanged("VAT"); } }        
        public String TELB { get { return _TELB; } set { _TELB = value; OnPropertyChanged("TELB"); } }
    }

    public class FiscalYear:BaseModel,IDataErrorInfo
    {
        private string _PhiscalID;
        private float _Stamp;
        private DateTime _EndDate;
        private DateTime _BeginDate;
        private string _BeginBsDate;
        private string _EndBsDate;
        public string PhiscalID { get { return _PhiscalID; } set { _PhiscalID = value; OnPropertyChanged("PhiscalID"); } }
        public DateTime BeginDate { get { return _BeginDate; } set { _BeginDate = value; OnPropertyChanged("BeginDate"); } }
        public DateTime EndDate { get { return _EndDate; } set { _EndDate = value; OnPropertyChanged("EndDate"); } }
        public string BeginBsDate { get { return _BeginBsDate; }
            set { _BeginBsDate = value; OnPropertyChanged("BeginBsDate"); } }
        public string EndBsDate
        {
            get { return _EndBsDate; }
            set { _EndBsDate = value; OnPropertyChanged("EndBsDate"); }
        }
        public float Stamp { get { return _Stamp; } set { _Stamp = value; OnPropertyChanged("Stamp"); } }

        public override string ToString()
        {
            return PhiscalID ;
        }
        public FiscalYear() {
            using (IDbConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                try
                {
                    var ret = con.Query<dynamic>("select FBDATE BeginDate,FEDATE EndDate,FBDATE_BS BeginBsDate,FEDATE_BS EndBsDate,PhiscalID from company ").SingleOrDefault();
                    if (ret != null)
                    {
                        this.PhiscalID = ret.PhiscalID;
                        this.BeginDate = ret.BeginDate;
                        this.EndDate = ret.EndDate;
                        this.BeginBsDate = ret.BeginBsDate;
                        this.EndBsDate = ret.EndBsDate;
                    }
                }
                catch(Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message + "Fiscal year initialization");
                }
            }
        }
        string IDataErrorInfo.Error
        {
            
            get 
            {
                if (EndDate <= BeginDate)
                    return "End date must be latter than begin date";
                return string.Empty; 
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get 
            {
                if (EndDate <= BeginDate)
                    return "End date must be latter than begin date";
                return string.Empty;
            }
        }
    }
}
