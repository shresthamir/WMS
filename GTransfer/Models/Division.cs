using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using GTransfer.Library;

namespace GTransfer.Models
{
    class Division : BaseModel
    {
        #region members
        private string _INITIAL;
        private string _NAME;
        private string _REMARKS;
        private int _MAIN;
        private string _COMNAME;
        private string _COMADD;
        private string _COMID;
        private string _ACID;
        private string _ID;
        private string _MSG1;
        private string _MSG2;
        private string _MSG3;
        private int _RATEGROUPID;


        #endregion
        #region properties
        public string INITIAL
        { get { return _INITIAL; } set { _INITIAL = value; OnPropertyChanged("INITIAL"); } }
        public string NAME
        { get { return _NAME; } set { _NAME  = value; OnPropertyChanged("NAME"); } }
        public string REMARKS
        { get { return _REMARKS; } set { _REMARKS = value; OnPropertyChanged("REMARKS"); } }
        public int MAIN
        { get { return _MAIN; } set { _MAIN = value; OnPropertyChanged("MAIN"); } }

        public string COMNAME
        { get { return _COMNAME; } set { _COMNAME = value; OnPropertyChanged("COMNAME"); } }
      
        public string COMADD
        { get { return _COMADD; } set { _COMADD = value; OnPropertyChanged("COMADD"); } }
        public string COMID
        { get { return _COMID; } set { _COMID = value; OnPropertyChanged("COMID"); } }
        public string ACID
        { get { return _ACID; } set { _ACID = value; OnPropertyChanged("ACID"); } }
        public string ID
        { get { return _ID; } set { _ID = value; OnPropertyChanged("ID"); } }
        public string MSG1
        { get { return _MSG1; } set { _MSG1 = value; OnPropertyChanged("MSG1"); } }
        public string MSG2
        { get { return _MSG2; } set { _MSG2 = value; OnPropertyChanged("MSG2"); } }
        public string MSG3
        { get { return _MSG3; } set { _MSG3 = value; OnPropertyChanged("MSG3"); } }
        public int RATEGROUPID
        { get { return _RATEGROUPID; } set { _RATEGROUPID = value; OnPropertyChanged("RATEGROUPID"); } }      

        #endregion 

    
    }
}
