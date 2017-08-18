using GTransfer.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTransfer.Models
{
    class Requisition:BaseModel
    {
        private int _ReqId;
        private string _Division;
        private DateTime _TDate;
        private string _TUser;
        private DateTime _Exp_DeliveryDate;
        private byte _IsApproved;
        private ObservableCollection<Requisition_Detail> _Requisition_Details;

        public int ReqId {get{return _ReqId;}set{_ReqId=value;OnPropertyChanged("ReqId"); }}
        public string Division { get { return _Division; } set { _Division = value; OnPropertyChanged("Division"); } }
        public DateTime TDate { get { return _TDate; } set { _TDate = value; OnPropertyChanged("TDate"); } }
        public string TUser { get { return _TUser; } set { _TUser = value; OnPropertyChanged("TUser"); } }
        public DateTime Exp_DeliveryDate { get { return _Exp_DeliveryDate; } set { _Exp_DeliveryDate = value; OnPropertyChanged("Exp_DeliveryDate"); } }
        public byte IsApproved { get { return _IsApproved; } set { _IsApproved = value; OnPropertyChanged("IsApproved"); } }
        public ObservableCollection<Requisition_Detail> Requisition_Details { get { return _Requisition_Details; }set { _Requisition_Details = value;OnPropertyChanged("Requisition_Details"); } }
    }
    class Requisition_Detail : BaseModel
    {
        private int _ReqId;
        private string _Mcode;
        private decimal _Quantity;
        private decimal _ApprovedQty;
        private string _Unit;
        private Product _Item;

        public Product Item { get { return _Item; } set { _Item = value; OnPropertyChanged("Item"); } }
        public int ReqId { get { return _ReqId; } set { _ReqId = value; OnPropertyChanged("ReqId"); } }
        public string Mcode { get { return _Mcode; } set { _Mcode = value; OnPropertyChanged("Mcode"); } }
        public decimal Quantity { get { return _Quantity; } set { _Quantity = value; OnPropertyChanged("Quantity"); } }
        public decimal ApprovedQty { get { return _ApprovedQty; } set { _ApprovedQty = value; OnPropertyChanged("ApprovedQty"); } }
        public string Unit { get { return _Unit; } set { _Unit = value; OnPropertyChanged("Unit"); } }
    }
}
