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
using GTransfer.UserInterfaces;
using System.Windows;
using Syncfusion.UI.Xaml.Grid;

namespace GTransfer.Reports
{
    class LocationWiseStockReportVM : BaseViewModel
    {
       
        private string _selectedWarehouse;
        public string selectedWarehouse { get { return _selectedWarehouse; } set { _selectedWarehouse = value;GetLocationList(); OnPropertyChanged("selectedWarehouse"); } }
        
        private string _selectedLocation;
        public string selectedLocation { get { return _selectedLocation; } set { _selectedLocation = value; OnPropertyChanged("selectedLocation"); } }
        private ObservableCollection<Location> _LocationList;
        private ObservableCollection<string> _warehouseList;
        public ObservableCollection<string> warehouseList { get { return _warehouseList; } set { _warehouseList = value; OnPropertyChanged("warehouseList"); } }
        public ObservableCollection<Location> LocationList { get { return _LocationList; } set { _LocationList = value; OnPropertyChanged("LocationList"); } }
        private ObservableCollection<ItemWiseStockModel> _ReportDataList;
        private SfDataGrid Report;

        public ObservableCollection<ItemWiseStockModel> ReportDataList { get { return _ReportDataList; } set { _ReportDataList = value; OnPropertyChanged("ReportDataList"); } }

        



        public LocationWiseStockReportVM(SfDataGrid _Report)
        {
            Report = _Report;
            GetWarehouseList();
        }
        public override void LoadMethod(object obj)
        {
            if (string.IsNullOrEmpty(selectedLocation)) { selectedLocation = "%"; }
            if (string.IsNullOrEmpty(selectedWarehouse)) { selectedWarehouse = "%"; }

            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                var result = con.Query<ItemWiseStockModel>("SELECT sum(InQty-OutQty)Stock,TPD.MCODE,MI.MENUCODE,MI.DESCA,TPD.Warehouse,TPD.LocationId,TL.LocationCode FROM RMD_TRNPROD_DETAIL TPD INNER JOIN MENUITEM MI ON TPD.MCODE=MI.MCODE INNER JOIN TBL_LOCATIONS TL ON TPD.LocationId=TL.LocationId WHERE TPD.LocationId LIKE '" + selectedLocation + "' AND TPD.Warehouse like '" + selectedWarehouse + "' group by TPD.Warehouse,TPD.LocationId,TPD.MCODE,UNIT,MENUCODE,DESCA,TL.LocationCode");
                if (result != null)
                {
                    ReportDataList = new ObservableCollection<ItemWiseStockModel>(result.Where(x=>x.Stock>0));
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

        #region PrintExport
        public override void ExecuteExport(object obj)
        {
            GlobalClass.ReportName = "Location Wise Stock Report";

            GlobalClass.ReportParams = "";// string.Format("From Date : {0} To {1}", FDate.ToString("MM/dd/yyyy"), TDate.ToString("MM/dd/yyyy"));

            wExportFormat ef = new wExportFormat(Report);
            ef.ShowDialog();
        }
        protected override bool CanExecutePrint(object obj)

        {
            return ReportDataList != null && ReportDataList.Count > 0;
        }

        protected override bool CanExecuteExport(object obj)
        {
            return ReportDataList != null && ReportDataList.Count > 0;
        }
        protected override bool CanExecutePreview(object obj)
        {
            return ReportDataList != null && ReportDataList.Count > 0;
        }


        public override void ExecutePreview(object obj)
        {

            GlobalClass.ReportName = "Location Wise Stock Report";
            GlobalClass.ReportParams = "";// string.Format("From Date : {0} To {1}", FDate.ToString("MM/dd/yyyy"), TDate.ToString("MM/dd/yyyy"));

            Report.PrintSettings.PrintPageMargin = new Thickness(30);
            Report.PrintSettings.AllowColumnWidthFitToPrintPage = false;
            Report.ShowPrintPreview();
        }

        public override void ExecutePrint(object obj)
        {

            GlobalClass.ReportName = "Location Wise Stock Report";
            GlobalClass.ReportParams = "";// string.Format("From Date : {0} To {1}", FDate.ToString("MM/dd/yyyy"), TDate.ToString("MM/dd/yyyy"));

            Report.PrintSettings.PrintPageMargin = new Thickness(30);
            Report.PrintSettings.AllowColumnWidthFitToPrintPage = false;
            Report.Print();
        }
        #endregion
    }

    class ItemWiseStockModel
    {
        public double Stock { get; set; }
        public string MENUCODE { get; set; }
        public string DESCA { get; set; }
        public string Warehouse { get; set; }
        public string LocationCode { get; set; }
    }
}
