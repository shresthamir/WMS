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
using System.Windows;

namespace GTransfer.Reports
{
    class StockMovementReportVM : BaseViewModel
    {
        private bool _ItemWise = true;
        private bool _MGroupWise;
        private bool _GroupWise;
        private ObservableCollection<Item> _MGroupList;
        private ObservableCollection<Item> _GroupList;
        private ObservableCollection<Item> _ItemList;
        private Item _SelectedItem;
        private DateTime _FDate;
        private DateTime _TDate;


        public DateTime FDate { get { return _FDate; } set { _FDate = value; OnPropertyChanged("FDate"); } }
        public DateTime TDate { get { return _TDate; } set { _TDate = value; OnPropertyChanged("TDate"); } }

        public bool ItemWise { get { return _ItemWise; } set { _ItemWise = value; OnPropertyChanged("ItemWise"); } }
        public bool MGroupWise { get { return _MGroupWise; } set { _MGroupWise = value; OnPropertyChanged("MGroupWise"); } }
        public bool GroupWise { get { return _GroupWise; } set { _GroupWise = value; OnPropertyChanged("MGroupWise"); } }
        public Item SelectedItem { get { return _SelectedItem; } set { _SelectedItem = value; OnPropertyChanged("SelectedItem"); } }

        public ObservableCollection<Item> ItemList { get { return _ItemList; } set { _ItemList = value; OnPropertyChanged("ItemList"); } }
        public ObservableCollection<Item> MGroupList { get { return _MGroupList; } set { _MGroupList = value; OnPropertyChanged("MGroupList"); } }
        public ObservableCollection<Item> GroupList { get { return _GroupList; } set { _GroupList = value; OnPropertyChanged("GroupList"); } }

        private ObservableCollection<StockMovementModel> _ReportDataList;
        public ObservableCollection<StockMovementModel> ReportDataList { get { return _ReportDataList; } set { _ReportDataList = value; OnPropertyChanged("ReportDataList"); } }




        public StockMovementReportVM()
        {
            FDate = TDate = DateTime.Today;
            GetAllList();
        }

        public override void LoadMethod(object obj)
        {
            string ItemSelctionClause = string.Empty;
            if (SelectedItem == null)
            {
                MessageBox.Show("Please select an item or category first!", "Stock Movement Report", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (ItemWise)
                ItemSelctionClause = "MI.MCODE  = '" + SelectedItem.MCODE + "'";
            else if (MGroupWise)
                ItemSelctionClause = "MI.MGROUP  = '" + SelectedItem.MCODE + "'";
            else
                ItemSelctionClause = "MI.PARENT  = '" + SelectedItem.MCODE + "'";

            string strSql = @"SELECT '' VCHRNO, '' [Date], '' BSDATE, 'Opening Balance' Particulars, 
MI.MENUCODE, MI.DESCA, D.UNIT, D.Warehouse, L.LocationCode, 0 InQty, 0 OutQty, SUM(D.InQty - D.OutQty) Balance   FROM RMD_TRNMAIN M 
JOIN  RMD_TRNPROD_DETAIL D ON M.VCHRNO = D.VCHRNO AND M.DIVISION = D.DIVISION AND M.PhiscalID = D.PhiscalID
JOIN MENUITEM MI ON D.MCODE = MI.MCODE
JOIN TBL_LOCATIONS L ON D.LocationId = L.LocationId
WHERE " + ItemSelctionClause + @" AND M.TRNDATE < @FDate
GROUP BY MI.MENUCODE, MI.DESCA, D.UNIT, D.Warehouse, L.LocationCode
UNION ALL
SELECT M.VCHRNO, CONVERT(VARCHAR, M.TRNDATE, 102) [Date], M.BSDATE, 
CASE WHEN LEFT(M.VCHRNO,2) = 'PI' THEN 'Goods Received'
WHEN LEFT(M.VCHRNO, 2) = 'TO' THEN 'Stock Issue To ' + B.NAME
WHEN LEFT(M.VCHRNO, 2) = 'LT' THEN 'Location Transfer' END Particulars,
MI.MENUCODE, MI.DESCA, D.UNIT, D.Warehouse, L.LocationCode, D.InQty, D.OutQty, 0 Balance--,row_number() OVER (ORDER BY trndate) AS row  
FROM RMD_TRNMAIN M 
JOIN  RMD_TRNPROD_DETAIL D ON M.VCHRNO = D.VCHRNO AND M.DIVISION = D.DIVISION AND M.PhiscalID = D.PhiscalID
JOIN MENUITEM MI ON D.MCODE = MI.MCODE
JOIN TBL_LOCATIONS L ON D.LocationId = L.LocationId
LEFT JOIN DIVISION B ON B.INITIAL = M.BILLTOADD
WHERE " + ItemSelctionClause + @" AND M.TRNDATE BETWEEN @FDate AND @TDate
ORDER BY  DESCA, LocationCode, [Date]";

            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                var result = con.Query<StockMovementModel>(strSql, this);
                if (result != null)
                {

                    decimal balance = 0;
                    string item = string.Empty;
                    string location = string.Empty;
                    ReportDataList = new ObservableCollection<StockMovementModel>();
                    foreach (StockMovementModel m in result)
                    {
                        if(location!=m.LocationCode || item != m.MENUCODE)
                        {
                            ReportDataList.Add(new StockMovementModel());
                        }
                        if (location != m.LocationCode)
                        {
                            balance = m.Balance;
                            location = m.LocationCode;
                            
                        }
                        if (item != m.MENUCODE)
                        {
                            balance = m.Balance;
                            location = m.LocationCode;
                            item = m.MENUCODE;
                        }
                        balance += m.InQty - m.OutQty;
                        m.Balance = balance;
                        ReportDataList.Add(m);

                    }
                    
                }
            }
        }

        private void GetAllList()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    ItemList = new ObservableCollection<Item>(conn.Query<Item>("SELECT MCODE,MENUCODE,DESCA FROM MENUITEM WHERE TYPE='A'  ORDER BY DESCA"));
                    MGroupList = new ObservableCollection<Item>(conn.Query<Item>("SELECT MCODE,MENUCODE,DESCA FROM MENUITEM WHERE PARENT = 'MI'  ORDER BY DESCA"));
                    GroupList = new ObservableCollection<Item>(conn.Query<Item>("SELECT MCODE,MENUCODE,DESCA FROM MENUITEM WHERE MCODE IN (SELECT PARENT FROM MENUITEM WHERE TYPE = 'A') ORDER BY DESCA"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Stock Movement Report", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    class StockMovementModel
    {
        public string BSDATE { get; set; }
        public string VCHRNO { get; set; }
        public string Date { get; set; }
        public string Particulars { get; set; }
        public string MENUCODE { get; set; }
        public string DESCA { get; set; }
        public string UNIT { get; set; }
        public string WAREHOUSE { get; set; }
        public string LocationCode { get; set; }
        public decimal InQty { get; set; }
        public decimal OutQty { get; set; }
        public decimal Balance { get; set; }
    }
}
