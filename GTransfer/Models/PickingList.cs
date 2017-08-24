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
        private int _RequitionId;
        private string _Mcode;
        private string _Location;
        private decimal _RequestQty;
        private decimal _AppprovedQty;

        public decimal AppprovedQty{get { return _AppprovedQty;  }  set {_AppprovedQty = value; OnPropertyChanged("AppprovedQty"); }}
        public decimal RequestQty { get { return _RequestQty; } set { _RequestQty = value; OnPropertyChanged("RequestQty"); } }
        public string Location { get { return _Location; } set { _Location = value; OnPropertyChanged("Location"); } }
        public string Mcode { get { return _Mcode; } set { _Mcode = value; OnPropertyChanged("Mcode"); } }
        public int RequitionId { get { return _RequitionId; } set { _RequitionId = value; OnPropertyChanged("RequitionId"); } }
    }
}
