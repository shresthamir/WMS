﻿using GTransfer.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Windows;
using Syncfusion.UI.Xaml.Grid;
using GTransfer.UserInterfaces;

namespace GTransfer.Reports
{
    class Item : BaseModel
    {
        private string _MCODE;
        private string _MENUCODE;
        private string _DESCA;

        public string MCODE { get { return _MCODE; } set { _MCODE = value; OnPropertyChanged("MCODE"); } }
        public string MENUCODE { get { return _MENUCODE; } set { _MENUCODE = value; OnPropertyChanged("MENUCODE"); } }
        public string DESCA { get { return _DESCA; } set { _DESCA = value; OnPropertyChanged("DESCA"); } }
    }
    class vmItemWiseStockReport : BaseViewModel
    {
        private string _Barcode;
        private string _selectedWarehouse;
        public string selectedWarehouse { get { return _selectedWarehouse; } set { _selectedWarehouse = value; OnPropertyChanged("selectedWarehouse"); } }
        public string Barcode { get { return _Barcode; } set { _Barcode = value; OnPropertyChanged("Barcode"); } }
        private string _selectedItem;
        public string selectedItem { get { return _selectedItem; } set { _selectedItem = value; OnPropertyChanged("selectedItem"); } }
        private ObservableCollection<Item> _ItemList;
        private ObservableCollection<string> _warehouseList;
        public ObservableCollection<string> warehouseList { get { return _warehouseList; } set { _warehouseList = value; OnPropertyChanged("warehouseList"); } }
        public ObservableCollection<Item> ItemList { get { return _ItemList; } set { _ItemList = value; OnPropertyChanged("ItemList"); } }
        private ObservableCollection<ItemWiseStockModel> _ReportDataList;
        private SfDataGrid Report;

        public ObservableCollection<ItemWiseStockModel> ReportDataList { get { return _ReportDataList; } set { _ReportDataList = value; OnPropertyChanged("ReportDataList"); } }

        
        public RelayCommand BarcodeChangeCommand { get { return new RelayCommand(ExecuteBarcodeChangeCommand); } }


        public vmItemWiseStockReport(SfDataGrid _Report)
        {
            Report = _Report;
            GetList();
        }
        private void ExecuteBarcodeChangeCommand(object obj)
        {
            try
            {
                if (!string.IsNullOrEmpty(Barcode))
                {
                    using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
                    {
                        var result = con.Query("SELECT A.BCODE,A.MCODE,A.UNIT,A.ISSUENO,A.EDATE,A.BCODEID,A.SUPCODE,A.BATCHNO,A.EXPIRY,A.REMARKS,A.INVNO,A.DIV,A.FYEAR,A.SRATE,B.DESCA FROM BARCODE A inner join Menuitem B on A.mcode=B.mcode WHERE A.BCODE='" + Barcode + "'").FirstOrDefault();
                        if (result == null) { MessageBox.Show("Invalid Barcode"); return; }
                        selectedItem = result.MCODE;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }


        }

        public override void LoadMethod(object obj)
        {
            if (string.IsNullOrEmpty(selectedItem)) { selectedItem = "%"; }
            if (string.IsNullOrEmpty(selectedWarehouse)) { selectedWarehouse = "%"; }

            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                var result = con.Query<ItemWiseStockModel>("SELECT sum(InQty-OutQty)Stock,TPD.MCODE,MI.MENUCODE,MI.DESCA,TPD.Warehouse,TPD.LocationId,TL.LocationCode,'' Bcode FROM RMD_TRNPROD_DETAIL TPD INNER JOIN MENUITEM MI ON TPD.MCODE=MI.MCODE INNER JOIN TBL_LOCATIONS TL ON TPD.LocationId=TL.LocationId WHERE TPD.MCODE LIKE '" + selectedItem + "' AND TPD.Warehouse like '" + selectedWarehouse + "' group by TPD.MCODE,TPD.Warehouse,TPD.LocationId,UNIT,MENUCODE,DESCA,TL.LocationCode");
                if (result != null)
                {
                    ReportDataList = new ObservableCollection<ItemWiseStockModel>(result);

                    //if (!string.IsNullOrEmpty(Barcode))
                    //{
                    //    var bList = con.Query<string>("select Bcode from barcode where mcode='" + selectedItem + "'");
                    //    if (bList != null)
                    //    {
                    //        var bc = bList.FirstOrDefault(b => b == Barcode);
                    //        if (!string.IsNullOrEmpty(bc))
                    //        {
                    //            foreach (var i in ReportDataList)
                    //            {
                    //                i.Bcode = bc;
                    //            }
                    //        }
                    //    }
                    //}
                }
            }

        }

        private void GetList()
        {
            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                warehouseList = new ObservableCollection<string>(con.Query<string>("SELECT NAME FROM RMD_WAREHOUSE"));
                ItemList = new ObservableCollection<Item>(con.Query<Item>("SELECT MCODE,MENUCODE,DESCA FROM MENUITEM WHERE TYPE='A'"));
            }
        }

        #region PrintExport
        public override void ExecuteExport(object obj)
        {
            GlobalClass.ReportName = "Item Wise Stock Report";

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

            GlobalClass.ReportName = "Item Wise Stock Report";
            GlobalClass.ReportParams = "";// string.Format("From Date : {0} To {1}", FDate.ToString("MM/dd/yyyy"), TDate.ToString("MM/dd/yyyy"));

            Report.PrintSettings.PrintPageMargin = new Thickness(30);
            Report.PrintSettings.AllowColumnWidthFitToPrintPage = false;
            
            Report.ShowPrintPreview();
        }

        public override void ExecutePrint(object obj)
        {

            GlobalClass.ReportName = "Item Wise Stock Report";
            GlobalClass.ReportParams = "";// string.Format("From Date : {0} To {1}", FDate.ToString("MM/dd/yyyy"), TDate.ToString("MM/dd/yyyy"));

            Report.PrintSettings.PrintPageMargin = new Thickness(30);
            Report.PrintSettings.AllowColumnWidthFitToPrintPage = false;
            Report.Print();
        }
        #endregion
    }
}
