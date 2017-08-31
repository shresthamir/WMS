using GTransfer.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTransfer.Models
{
    class PickingList:BaseModel
    {
        private int _ReqId;
        private string _Mcode;
        private string _Unit;
        private string _LocationId;
        private decimal _ReqQty;
        private decimal _Quantity;
        private string _LocationCode;
        private decimal _Balance;
        private string _MENUCODE;
        private string _DESCA;
        private byte _Status;
        private string _Bcode;
        private string _MCAT;
        private string _DeviceId;
        private string _DeviceName;

        public string Bcode { get { return _Bcode; }set { _Bcode = value;OnPropertyChanged("Bcode"); } }
        public byte Status { get { return _Status; }set { _Status = value;OnPropertyChanged("Status"); } }
        public string DESCA { get { return _DESCA; } set { _DESCA = value; OnPropertyChanged("DESCA"); } }
        public string MENUCODE { get { return _MENUCODE; }set { _MENUCODE = value;OnPropertyChanged("MENUCODE"); } }
        public string Unit { get { return _Unit; }set { _Unit = value;OnPropertyChanged("Unit"); } }
        public decimal Quantity {get { return _Quantity;  }  set {_Quantity = value; OnPropertyChanged("Quantity"); }}
        public decimal ReqQty { get { return _ReqQty; } set { _ReqQty = value; OnPropertyChanged("ReqQty"); } }
        public decimal Balance { get { return _Balance; } set { _Balance = value; OnPropertyChanged("Balance"); } }
        public string LocationId { get { return _LocationId; } set { _LocationId = value; OnPropertyChanged("LocationId"); } }
        public string LocationCode { get { return _LocationCode; } set { _LocationCode = value; OnPropertyChanged("LocationCode"); } }
        public string Mcode { get { return _Mcode; } set { _Mcode = value; OnPropertyChanged("Mcode"); } }
        public int ReqId { get { return _ReqId; } set { _ReqId = value; OnPropertyChanged("ReqId"); } }
        public string MCAT { get { return _MCAT; }set { _MCAT = value;OnPropertyChanged("MCAT"); } }
        public string DeviceId { get { return _DeviceId; }set { _DeviceId = value;OnPropertyChanged("DeviceId"); } }
        public string DeviceName { get { return _DeviceName; } set { _DeviceName = value; OnPropertyChanged("DeviceName"); } }
    }
}
