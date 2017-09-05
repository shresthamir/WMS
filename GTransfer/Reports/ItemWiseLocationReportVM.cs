using GTransfer.Library;
using GTransfer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace GTransfer.Reports
{
    class ItemWiseLocationReportVM:BaseViewModel
    {
       
        private string _selectedWarehouse;
        public string selectedWarehouse { get { return _selectedWarehouse; } set { _selectedWarehouse = value;GetLocationList(); OnPropertyChanged("selectedWarehouse"); } }
        
        private string _selectedLocation;
        public string selectedLocation { get { return _selectedLocation; } set { _selectedLocation = value; OnPropertyChanged("selectedLocation"); } }
        private ObservableCollection<Location> _LocationList;
        private ObservableCollection<string> _warehouseList;
        public ObservableCollection<string> warehouseList { get { return _warehouseList; } set { _warehouseList = value; OnPropertyChanged("warehouseList"); } }
        public ObservableCollection<Location> LocationList { get { return _LocationList; } set { _LocationList = value; OnPropertyChanged("LocationList"); } }
        private ObservableCollection<dynamic> _ReportDataList;
        public ObservableCollection<dynamic> ReportDataList { get { return _ReportDataList; } set { _ReportDataList = value; OnPropertyChanged("ReportDataList"); } }

        public RelayCommand LoadReportCommand { get { return new RelayCommand(ExecuteLoadReportCommand); } }



        public ItemWiseLocationReportVM()
        {
            GetWarehouseList();
        }
        private void ExecuteLoadReportCommand(object obj)
        {
            if (string.IsNullOrEmpty(selectedLocation)) { selectedLocation = "%"; }
            if (string.IsNullOrEmpty(selectedWarehouse)) { selectedWarehouse = "%"; }

            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                var result = con.Query("SELECT sum(InQty-OutQty)Stock,TPD.MCODE,MI.MENUCODE,MI.DESCA,TPD.Warehouse,TPD.LocationId,TL.LocationCode FROM RMD_TRNPROD_DETAIL TPD INNER JOIN MENUITEM MI ON TPD.MCODE=MI.MCODE INNER JOIN TBL_LOCATIONS TL ON TPD.LocationId=TL.LocationId WHERE TPD.LocationId LIKE '" + selectedLocation + "' AND TPD.Warehouse like '" + selectedWarehouse + "' group by TPD.MCODE,TPD.Warehouse,TPD.LocationId,UNIT,MENUCODE,DESCA,TL.LocationCode");
                if (result != null)
                {
                    ReportDataList = new ObservableCollection<dynamic>(result);
                }
            }

        }

        private void GetWarehouseList()
        {
            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                warehouseList = new ObservableCollection<string>(con.Query<string>("SELECT NAME FROM RMD_WAREHOUSE"));
            }
        }
        private void GetLocationList()
        {
            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                LocationList = new ObservableCollection<Location>(con.Query<Location>("SELECT * FROM TBL_LOCATIONS where Warehouse='" + selectedWarehouse + "'and  Level=" + Settings.LocationLevelLimit));
            }
        }
    }
}
