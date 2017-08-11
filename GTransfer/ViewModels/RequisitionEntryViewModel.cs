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
    class RequisitionEntryViewModel : BaseViewModel
    {
        private Requisition _RequisitionObj;
        private Requisition_Detail _Requisition_DetailObj;
        private Requisition_Detail _selectedReqDetails;
        private ObservableCollection<Division> _divisionList;
        private ObservableCollection<Unit> _UnitList;
        private ObservableCollection<Product> _ItemList;
        private string _SelectedItemMcode;

       
        public ObservableCollection<Division> divisionList { get { return _divisionList; } set { _divisionList = value; OnPropertyChanged("divisionList"); } }
        public ObservableCollection<Unit> UnitList { get { return _UnitList; } set { _UnitList = value; OnPropertyChanged("UnitList"); } }
        public ObservableCollection<Product> ItemList { get { return _ItemList; } set { _ItemList = value; OnPropertyChanged("ItemList"); } }
        public Requisition RequisitionObj { get { return _RequisitionObj; } set { _RequisitionObj = value; OnPropertyChanged("RequisitionObj"); } }
        public Requisition_Detail Requisition_DetailObj { get { return _Requisition_DetailObj; } set { _Requisition_DetailObj = value; OnPropertyChanged("Requisition_DetailObj"); } }
        public Requisition_Detail selectedReqDetails { get { return _selectedReqDetails; } set { _selectedReqDetails = value; OnPropertyChanged("selectedReqDetails"); } }
        public string SelectedItemMcode {
            get { return _SelectedItemMcode; }
            set {
                if (_SelectedItemMcode == value) return;
                _SelectedItemMcode = value;
                if (!string.IsNullOrEmpty(_SelectedItemMcode)) {
                    if (Requisition_DetailObj == null) Requisition_DetailObj = new Requisition_Detail();
                    Requisition_DetailObj.Mcode = _SelectedItemMcode;
                    Requisition_DetailObj.Item = new Product(_SelectedItemMcode);
                    Requisition_DetailObj.Unit = Requisition_DetailObj.Item.BASEUNIT;
                }
                OnPropertyChanged("SelectedItemMcode"); } }


        public RelayCommand AddCommand { get { return new RelayCommand(ExecuteAdd); } }
        public RelayCommand GridDoubleClickEvent { get { return new RelayCommand(ExecuteGridDoubleClickEvent); } }


        public RequisitionEntryViewModel()
        {try
            {
                EditVisible = true;
                DeleteVisible = true;
                RequisitionObj = new Requisition();
                divisionList = new ObservableCollection<Division>(GetDivisionList());
                UnitList = new ObservableCollection<Unit>(GetUnitList());
                ItemList = new ObservableCollection<Product>(GetItemList());
                RequisitionObj = new Requisition() { Requisition_Details = new ObservableCollection<Requisition_Detail>() };
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void ExecuteAdd(object obj)
        {
            if (Requisition_DetailObj.Item == null) { return; }
            if (Requisition_DetailObj.Quantity <= 0) { MessageBox.Show("Please provide valid Quantity");return; }
            if (RequisitionObj.Requisition_Details.Any((item) => item.Mcode == Requisition_DetailObj.Mcode))
            {
                    MessageBox.Show("Item  is already in the Table below", "Duplicate Data", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }

            RequisitionObj.Requisition_Details.Add(Requisition_DetailObj);
            Requisition_DetailObj = new Requisition_Detail();
            SelectedItemMcode = null;
        }
        private void ExecuteGridDoubleClickEvent(object obj)
        {
           var RD = GParse.CopyPropertyValues(selectedReqDetails, new Requisition_Detail()) as Requisition_Detail;
            SelectedItemMcode = RD.Mcode;
            Requisition_DetailObj.Unit = RD.Unit;
            Requisition_DetailObj.Quantity = RD.Quantity;
            RequisitionObj.Requisition_Details.Remove(selectedReqDetails);
             }
        public override bool UndoMethod(object obj)
        {
            RequisitionObj = new Requisition() { Requisition_Details = new ObservableCollection<Requisition_Detail>() };
            Requisition_DetailObj = new Requisition_Detail();
            SelectedItemMcode = null;
            return false;
        }
        public override void DeleteMethod(object obj)
        {
            if (MessageBox.Show("You are about to delete current Requisition.Once you Delete you cannot recover it back. Do you want to proceed?", "Delete Data", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
            {
                Conn.Open();
                Conn.Execute("DELETE FROM TBL_REQUISITION_DETAILS WHERE ReqId=" + RequisitionObj.ReqId + "");
                Conn.Execute("DELETE FROM TBL_REQUISITION WHERE ReqId=" + RequisitionObj.ReqId + "");
            }
            MessageBox.Show("Requisition is Deleted Successfully...");
     
            ExecuteUndo(null);
        }
        public override void LoadMethod(object obj)
        {
            if (RequisitionObj == null || RequisitionObj.ReqId == 0) return;
            using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
            {
                Conn.Open();
               var RQ=  Conn.Query<Requisition>("SELECT * FROM TBL_REQUISITION WHERE ReqId=" + RequisitionObj.ReqId + "");
                if (RQ != null) {
                    var RQD = Conn.Query<Requisition_Detail>("SELECT * FROM TBL_REQUISITION_DETAILS WHERE ReqId=" + RequisitionObj.ReqId + "");
                    RequisitionObj = RQ.FirstOrDefault();
                    RequisitionObj.Requisition_Details = new ObservableCollection<Requisition_Detail>(RQD);
                    foreach (var r in RequisitionObj.Requisition_Details) {
                        r.Item = ItemList.FirstOrDefault(x => x.MCODE == r.Mcode);
                    }
                    SetAction(ButtonAction.Selected);
                }

               
            }

        }
        public override void NewMethod(object obj)
        {
            RequisitionObj = new Requisition() { Requisition_Details = new ObservableCollection<Requisition_Detail>(),ReqId=GetNewReqId(),TDate=DateTime.Today,Exp_DeliveryDate=DateTime.Today };

        }
        public override void SaveMethod(object obj)
        {
            if (MessageBox.Show("You are about to Save the Requisition. Do you want to proceed?", "Save Data", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            if (RequisitionObj == null || RequisitionObj.Requisition_Details == null || RequisitionObj.Requisition_Details.Count <= 0) return;
            if (string.IsNullOrEmpty(RequisitionObj.Division)) { MessageBox.Show("Please select a Division"); return; }
            try {
                if (_action == ButtonAction.New)
                {
                    RequisitionObj.ReqId = GetNewReqId();
                    RequisitionObj.TUser = GlobalClass.CurrentUser.UNAME;
                    foreach (var rd in RequisitionObj.Requisition_Details)
                    {
                        rd.ReqId = RequisitionObj.ReqId;
                        rd.ApprovedQty = rd.Quantity;
                    }

                    using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
                    {
                        Conn.Open();
                        SqlTransaction tran = Conn.BeginTransaction();
                        try
                        {
                            Conn.Execute("INSERT INTO TBL_REQUISITION(ReqId, Division, TDate, TUser, Exp_DeliveryDate, IsApproved) VALUES(@ReqId,@Division,@TDate,@TUser,@Exp_DeliveryDate, @IsApproved)", RequisitionObj, tran);
                            Conn.Execute("INSERT INTO TBL_REQUISITION_DETAILS(ReqId, Mcode, Quantity, ApprovedQty, Unit) VALUES(@ReqId,@Mcode,@Quantity,@ApprovedQty,@Unit)", RequisitionObj.Requisition_Details, tran);
                            tran.Commit();
                            MessageBox.Show("Requisition is Saved Sucessfully...");
                        }
                        catch(Exception ex) { tran.Rollback();MessageBox.Show(ex.Message);return; }

                    }
                }
                else if (_action == ButtonAction.Edit) {
                    RequisitionObj.TUser = GlobalClass.CurrentUser.UNAME;
                    foreach (var rd in RequisitionObj.Requisition_Details)
                    {
                        rd.ReqId = RequisitionObj.ReqId;
                        rd.ApprovedQty = rd.Quantity;
                    }

                    using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
                    {
                        Conn.Open();
                        SqlTransaction tran = Conn.BeginTransaction();
                        try
                        {
                            Conn.Execute("UPDATE TBL_REQUISITION SET Division=@Division, TDate=@TDate, TUser=@TUser, Exp_DeliveryDate=@Exp_DeliveryDate, IsApproved=@IsApproved WHERE ReqId=@ReqId", RequisitionObj, tran);
                            Conn.Execute("DELETE FROM TBL_REQUISITION_DETAILS WHERE ReqId=" + RequisitionObj.ReqId + "",null,tran);
                            Conn.Execute("INSERT INTO TBL_REQUISITION_DETAILS(ReqId, Mcode, Quantity, ApprovedQty, Unit) VALUES(@ReqId,@Mcode,@Quantity,@ApprovedQty,@Unit)", RequisitionObj.Requisition_Details, tran);
                            tran.Commit();
                            MessageBox.Show("Requisition is Updated Sucessfully...");
                        }
                        catch(Exception ex) { tran.Rollback();MessageBox.Show(ex.Message);return; }

                    }
                }
                ExecuteUndo(null);
                } catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private int GetNewReqId() {
            using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
            {
                              
                 return Convert.ToInt32(Conn.ExecuteScalar("SELECT ISNULL(MAX(ReqId),0)+1 FROM TBL_REQUISITION"));
            }
        }

        private IEnumerable<Product> GetItemList()
        {
            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                return con.Query<Product>("SELECT * FROM MENUITEM WHERE TYPE='A'");
            }
        }
        private IEnumerable<Unit> GetUnitList()
        {
            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                return con.Query<Unit>("SELECT UNITS FROM UNIT");
            }
        }
        private IEnumerable<Division> GetDivisionList()
        {
            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString))
            {
                return con.Query<Division>("SELECT * FROM DIVISION");
            }
        }
    }
}
