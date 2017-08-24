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

namespace GTransfer.ViewModels
{
    class ItemLocationMappingViewModel : BaseViewModel
    {
        private Product _SelectedMGroup;
        private bool _IsCheckedTree;
        private List<Product> _menuList;
        IEnumerable<Product> DataSource;
        public ObservableCollection<Product> _omenuitemlist;
        public ObservableCollection<Product> oMenuitemlist
        {
            get { return _omenuitemlist; }
            set { _omenuitemlist = value; OnPropertyChanged("oMenuitemlist"); }
        }

        private List<Product> _MGroupList;
        public List<Product> MGroupList { get { return _MGroupList; } set { _MGroupList = value; OnPropertyChanged("MGroupList"); } }
        public Product SelectedMGroup
        {
            get { return _SelectedMGroup; }
            set { _SelectedMGroup = value; OnPropertyChanged("SelectedMGroup"); }
        }
        public bool IsTreeFormat
        {
            get { return _IsCheckedTree; }
            set { _IsCheckedTree = value; OnPropertyChanged("IsTreeFormat"); }
        }
        public List<Product> MenuList
        {
            get { return _menuList; }
            set { _menuList = value; }
        }
        public RelayCommand RadioButtonValidationCommand { get; set; }
        public RelayCommand RunCommand { get; set; }
        public ItemLocationMappingViewModel()
        {
            try
            {
                oMenuitemlist = new ObservableCollection<Product>();

                using (SqlConnection conn = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    //DataSource = ;
                    MGroupList = new List<Product>(conn.Query<Product>("SELECT MCODE,PARENT,DESCA,MENUCODE,MCAT,MGROUP,TYPE,DISMODE,DISRATE, BASEUNIT, DISAMOUNT, RATE_A FROM MENUITEM WHERE PARENT = 'MI' ORDER BY DESCA"));
                }
                SaveCommand = new RelayCommand(ExecuteSave);
                RunCommand = new RelayCommand(ExecuteSearch);
                RadioButtonValidationCommand = new RelayCommand(ExecuteValidation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Discount Setting", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        void LoadTree(Product Parent)
        {
            foreach (Product p in DataSource.Where(x => x.PARENT == Parent.MCODE).OrderBy(x => x.DESCA))
            {
                Product NewNode = new Product();
                NewNode.MCODE = p.MCODE;
                NewNode.DESCA = p.DESCA;
                NewNode.PARENT = p.PARENT;
                NewNode.TYPE = p.TYPE;
                NewNode.MENUCODE = p.MENUCODE;
                NewNode.MCAT = p.MCAT;
                NewNode.MGROUP = p.MGROUP;
                NewNode.DISMODE = p.DISMODE;
                NewNode.DISRATE = p.DISRATE;
                NewNode.PDisMode = p.DISMODE;
                NewNode.PDisrate = p.DISRATE;
                NewNode.PMCAT = p.MCAT;
                NewNode.PMCAT1 = p.MCAT1;
                NewNode.RATE_A = p.RATE_A;
                NewNode.BASEUNIT = p.BASEUNIT;
                NewNode.DISAMOUNT = p.DISAMOUNT;
                NewNode.LocationVsItemId = p.LocationVsItemId;
                NewNode.PreviousLocation = p.PreviousLocation;

                NewNode.PropertyChanged += NewNode_PropertyChanged;
                if (p.TYPE == "G")
                    LoadTree(NewNode);
                Parent.Children.Add(NewNode);
            }
        }



        private void ExecuteValidation(object obj)
        {
            //if (IsCheackedNonTree == false || IsTreeFormat == false)
            //{
            //    oMenuitemlist = new ObservableCollection<Product>();
            //    ObservableCollection<Product> oc = new ObservableCollection<Product>();
            //    oc.Clear();
            //}
        }

        public void ExecuteSave(object obj)
        {
            //ExecuteValidation(obj);
            if (SelectedMGroup == null)
            {
                System.Windows.MessageBox.Show("Please select product Catageory Item for further Proceed", "Alert");
                return;
            }
            if (oMenuitemlist.Count <= 0) { return; }

            if (System.Windows.MessageBox.Show("Previous Item's Location  will be Permanantly Updated by current entered Location and will not able to recover.Do you want to continue?", "Alert", MessageBoxButton.YesNo, MessageBoxImage.Question) != System.Windows.MessageBoxResult.Yes)
                return;
            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    foreach (Product p in oMenuitemlist)
                    {
                        CatagoryGroupTreeMapping(p);
                        UpdateProduct(p,tran);
                    }
                    tran.Commit();
                    System.Windows.MessageBox.Show("Item Location Mapped Sucessfully...", "Information");
                    oMenuitemlist = new ObservableCollection<Product>();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    tran.Rollback();
                }
            }

        }
        private void CatagoryGroupTreeMapping(Product p) {
               
                    foreach (Product pro in p.Children)
                {
                if (!string.IsNullOrEmpty(p.LocationId)) { 
                    pro.LocationId = p.LocationId;
                    pro.RowChanged = 1; }
                if (p.TYPE == "G")
                {
                    CatagoryGroupTreeMapping(pro);
                }
             
                
            }
        }
        public void ExecuteSearch(object obj)
        {

            if (SelectedMGroup == null)
            {
                System.Windows.MessageBox.Show("Please select Product Category First", "Alert");
                return;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    var strSql = string.Format("SELECT MI.MCODE,MI.PARENT,MI.DESCA,MI.MENUCODE,MI.MCAT,MI.MGROUP,MI.TYPE,MI.DISMODE,MI.DISRATE, MI.RATE_A, MI.BASEUNIT, MI.DISAMOUNT,IVL.Id LocationVsItemId,L.LocationName PreviousLocation FROM MENUITEM MI LEFT JOIN TBL_ITEM_DEFAULT_LOCATION IVL ON MI.MCODE=IVL.MCODE LEFT JOIN TBL_LOCATIONS L ON IVL.LID=L.LocationId WHERE MGROUP = '{0}'", SelectedMGroup.MCODE);
                    DataSource = conn.Query<Product>(strSql);
                }
                if (IsTreeFormat == true)
                {
                    var source = new List<Product>();
                    Product prod = new Product { MCODE = SelectedMGroup.MCODE, DESCA = SelectedMGroup.DESCA, TYPE = "G" };
                    LoadTree(prod);
                    source.Add(prod);
                    oMenuitemlist = new ObservableCollection<Product>(source);
                }
                else
                {
                    oMenuitemlist = new ObservableCollection<Product>(DataSource.Where(x => x.MGROUP == SelectedMGroup.MCODE && x.TYPE == "A").Select(x =>
                        new Product
                        {
                            MCODE = x.MCODE,
                            MENUCODE = x.MENUCODE,
                            PARENT = x.PARENT,
                            DESCA = x.DESCA,
                            TYPE = x.TYPE,
                            DISMODE = x.DISMODE,
                            DISRATE = x.DISRATE,
                            MGROUP = x.MGROUP,
                            MCAT = x.MCAT,
                            PDisrate = x.DISRATE,
                            PDisMode = x.DISMODE,
                            PMCAT = x.MCAT,
                            PMCAT1 = x.MCAT1,
                            DISAMOUNT = x.DISAMOUNT,
                            BASEUNIT = x.BASEUNIT,
                            RATE_A = x.RATE_A,
                            PreviousLocation=x.PreviousLocation,
                            LocationVsItemId=x.LocationVsItemId

                        }
                        ));
                    foreach (Product p in oMenuitemlist)
                    {
                        p.PropertyChanged += NewNode_PropertyChanged;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Discount Setting", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void UpdateProduct(Product mi,SqlTransaction tran)
        {

            if (mi.RowChanged > 0)
            {
                if (mi.TYPE != "G")
                {
                    if (string.IsNullOrEmpty(mi.PreviousLocation))
                    {
                        int id = Convert.ToInt32(tran.Connection.ExecuteScalar("SELECT ISNULL(MAX(ISNULL(Id,0)),0)+1 FROM TBL_ITEM_DEFAULT_LOCATION", null, tran));
                        var IL = new ItemVsLocation() { Id = id, LID = mi.LocationId, MCODE = mi.MCODE };
                        tran.Connection.Execute("INSERT INTO TBL_ITEM_DEFAULT_LOCATION VALUES(@Id,@LID,@MCODE)", IL, tran);
                    }
                    else
                    {
                        var IL2 = new ItemVsLocation() { Id = mi.LocationVsItemId, LID = mi.LocationId, MCODE = mi.MCODE };
                        tran.Connection.Execute("UPDATE TBL_ITEM_DEFAULT_LOCATION SET LID = @LID,MCODE=@MCODE WHERE Id = @Id", IL2, tran); 
                    }
                }
              
            }
            foreach (Product p in mi.Children)
            {
                UpdateProduct(p, tran);
            }
        }

private void NewNode_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
{
    if (e.PropertyName == "LocationId")
        (sender as Product).RowChanged = 1;
}
    }
}
