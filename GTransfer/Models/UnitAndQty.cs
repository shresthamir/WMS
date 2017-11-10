using GTransfer.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTransfer.Models
{
    public class UnitAndQty : BaseModel
    {
        string _Unit;
        double _Qty;
        double _Rate;
        string _BaseUnit;
        double _BaseQty;
        double _BaseRate;
        double _ConversionFactor;

        public string Unit { get { return _Unit; } set { _Unit = value; OnPropertyChanged("Unit"); } }
        public double Qty { get { return _Qty; } set { _Qty = value; OnPropertyChanged("Qty"); } }
        public double Rate { get { return _Rate; } set { _Rate = value; OnPropertyChanged("Rate"); } }
        public string BaseUnit { get { return _BaseUnit; } set { _BaseUnit = value; OnPropertyChanged("BaseUnit"); } }
        public double BaseQty { get { return _BaseQty; } set { _BaseQty = value; OnPropertyChanged("BaseQty"); } }
        public double BaseRate { get { return _BaseRate; } set { _BaseRate = value; OnPropertyChanged("BaseRate"); } }
        public double ConversionFactor { get { return _ConversionFactor; } set { _ConversionFactor = value; OnPropertyChanged("ConversionFactor"); } }

        public UnitAndQty()
        {
            this.PropertyChanged += UnitAndQty_PropertyChanged;
        }

        void UnitAndQty_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Qty":
                    BaseQty = Qty * ConversionFactor;
                    break;
                case "Rate":
                    BaseRate = Rate * ConversionFactor;
                    break;
                case "ConversionFactor":
                    BaseQty = Qty * ConversionFactor;
                    BaseRate = Rate * ConversionFactor;
                    break;
            }
        }

    }
    public class Unit : BaseModel
    {
        string _UNITS;
        public string UNITS { get { return _UNITS; } set { _UNITS = value; OnPropertyChanged("UNITS"); } }
    }
    public class TSettlementMode : BaseModel
    {
        private string _DESCRIPTION;
        private byte _DECREASE;
        private string _LocationPrefix;

        public byte DECREASE
        {
            get { return _DECREASE; }
            set { _DECREASE = value; OnPropertyChanged("DECREASE"); }
        }

        public string DESCRIPTION
        {
            get { return _DESCRIPTION; }
            set { _DESCRIPTION = value; OnPropertyChanged("DESCRIPTION"); }
        }

        public string LocationPrefix
        {
            get { return _LocationPrefix; }
            set { _LocationPrefix = value; OnPropertyChanged("LocationPrefix"); }
        }
    }
}
