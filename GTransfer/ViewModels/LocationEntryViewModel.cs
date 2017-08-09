using GTransfer.Library;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dapper;
using System.Collections.ObjectModel;
using GTransfer.Models;
using GTransfer.Interfaces;

namespace GTransfer.ViewModels
{
    class LocationEntryViewModel : BaseViewModelWithTree
    {
        private Location _LocationObj;
        private Location _selectedLocation;
        private bool _editMode;
        private string _InformationTest;

        public string InformationTest { get { return _InformationTest; } set { _InformationTest = value; OnPropertyChanged("InformationTest"); } }
        public bool editMode { get { return _editMode; } set { _editMode = value; OnPropertyChanged("editMode"); } }
        public Location selectedLocation { get { return _selectedLocation; } set { _selectedLocation = value; OnPropertyChanged("selectedLocation"); } }
        public Location LocationObj { get { return _LocationObj; } set { _LocationObj = value; OnPropertyChanged("LocationObj"); } }
        private ObservableCollection<ITreeNode> _LocationTreeList;
        public ObservableCollection<ITreeNode> LocationTreeList { get { return _LocationTreeList; } set { _LocationTreeList = value; OnPropertyChanged("LocationTreeList"); } }

        public LocationEntryViewModel()
        {
            NewEnabled = false;
            EditVisible = true;
            DeleteVisible = true;
            LocationTreeMaker();
        }

        private void LocationTreeMaker()
        {
            try
            {
                var WList = new ObservableCollection<dynamic>();
                using (SqlConnection conn = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    WList = new ObservableCollection<dynamic>(conn.Query<dynamic>("SELECT * FROM RMD_WAREHOUSE"));
                    TreeSource = conn.Query<Location>("SELECT * FROM TBL_LOCATIONS");
                }

                LocationTreeList = new ObservableCollection<ITreeNode>();
                foreach (var W in WList)
                {
                    Root = new Location() { LocationId = W.NAME, LocationName = W.NAME, Path = W.NAME, IsExpanded = true, IsMatch = true, Level = 0, Warehouse = W.NAME, Children = new ObservableCollection<ITreeNode>(), Parent = new Location() { LocationId = "Root", LocationName = "Root" } };
                    (Root as Location).PropertyChanged += ATree_PropertyChanged;
                    LoadChild(Root as Location);
                    LocationTreeList.Add(Root as Location);
                }


                //  LoadChild(Root as Location);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void LoadChild(Location Parent, Location _LocationType = null)
        {
            try
            {
                foreach (Location l in TreeSource.Where(x => x.ParentID == Parent.LocationId).OrderBy(x => x.NodeName))
                {
                    Location NewNode = new Location() { LocationId = l.LocationId, LocationName = l.LocationName, Parent = Parent, Path = l.Path, IsMatch = true, Level = l.Level, ParentLocation = Parent.LocationId, Warehouse = Parent.Warehouse, Children = new ObservableCollection<ITreeNode>() };

                    Parent.Children.Add(NewNode);
                    NewNode.PropertyChanged += ATree_PropertyChanged;
                    _TotalNodes.Add(NewNode);
                    // if (NewNode) {
                    LoadChild(NewNode);
                    // LocationTreeList.Add(NewNode);
                    //  }

                }
            }
            catch (Exception ex)
            {
                GlobalClass.ProcessError(ex, "Ledger Account");
            }
        }
        public void ATree_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected" && (sender as ITreeNode).IsSelected)
            {
                try
                {
                    InformationTest = "";

                    selectedLocation = (Location)sender;
                    LocationObj = selectedLocation;
                    editMode = true;
                    SetAction(ButtonAction.Selected);
                    if (selectedLocation.Level == 0)
                    {
                        EditEnabled = false;
                        DeleteEnabled = false;
                    }
                    if (selectedLocation.Level == Settings.LocationLevelLimit)
                    {
                        InformationTest = "Location Level Limit Reached.Can't add further sub Location.";
                        NewEnabled = false;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public override bool UndoMethod(object obj)
        {
            editMode = false;
            selectedLocation = null;
            LocationObj = null;
            InformationTest = "";
            return false;
        }
        public override void DeleteMethod(object obj)
        {
            if (MessageBox.Show("You are about to delete current Location.Once you Delete you cannot recover it back. Do you want to proceed?", "Delete Data", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
            {
                Conn.Open();
                var checkChild = Conn.ExecuteScalar("SELECT LocationId FROM TBL_LOCATIONS WHERE ParentLocation='" + selectedLocation.LocationId + "'");
                if (checkChild != null) { MessageBox.Show("Location contains another sub location so it can't be deleted"); return; }
                Conn.Query("DELETE FROM TBL_LOCATIONS WHERE LocationId='" + selectedLocation.LocationId + "'");
            }
            MessageBox.Show("Selected Location is Deleted Successfully...");
            LocationTreeMaker();
            ExecuteUndo(null);
        }
        public override void NewMethod(object obj)
        {
            if (selectedLocation == null) { MessageBox.Show("Please select the location in Tree to insert sub Location"); return; }
            LocationObj = new Location();
            LocationObj.Parent = selectedLocation;
            LocationObj.ParentLocation = selectedLocation.LocationId;
            LocationObj.Warehouse = selectedLocation.Warehouse;
            LocationObj.Level = selectedLocation.Level + 1;            
            editMode = false;
        }
        public override void SaveMethod(object obj)
        {
            if (selectedLocation == null) { MessageBox.Show("Invalid Data."); return; }
            if (LocationObj.Level > Settings.LocationLevelLimit) { MessageBox.Show("Location Level exceed the Level Limits"); return; }

            try
            {
                //if (MessageBox.Show("You are about to save current Location. Do you want to proceed?", "Save Data", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                //    return;
                if (_action == ButtonAction.New)
                {
                    LocationObj.Path = selectedLocation.Path + "\\" + LocationObj.LocationName;
                    using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
                    {
                        Conn.Open();
                        Conn.Query("INSERT INTO TBL_LOCATIONS(LocationId, LocationName, ParentLocation, Level, Warehouse, Path) VALUES(@LocationId,@LocationName,@ParentLocation,@Level,@Warehouse, @Path)", LocationObj);
                        MessageBox.Show("Location is Saved Sucessfully...");
                    }
                }
                else if (_action == ButtonAction.Edit)
                {
                    using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
                    {
                        Conn.Open();
                        Conn.Query("UPDATE TBL_LOCATIONS SET LocationName=@LocationName,ParentLocation=@ParentLocation,Level=@Level,Warehouse=@Warehouse WHERE LocationId=@LocationId", LocationObj);
                        MessageBox.Show("Selected Location is Updated Sucessfully...");
                    }
                }
                LocationTreeMaker();
                ExecuteUndo(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error On Saving..." + ex.Message);
            }
        }

    }
}
